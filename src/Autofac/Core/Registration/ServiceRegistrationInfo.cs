﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.CompilerServices;
using Autofac.Core.Resolving.Pipeline;

namespace Autofac.Core.Registration;

/// <summary>
/// 跟踪注册表已知的服务。
/// Tracks the services known to the registry.
/// </summary>
internal class ServiceRegistrationInfo : IResolvePipelineBuilder
{
    private volatile bool _isInitialized;

    private readonly Service _service;

    private IComponentRegistration? _fixedRegistration;

    /// <summary>
    /// 隐式默认服务实现列表。覆盖默认实现被附加到末尾，因此枚举也应该从末尾开始，最后是最默认的实现。
    ///  List of implicit default service implementations. Overriding default implementations are appended to the end,
    ///  so the enumeration should begin from the end too, and the most default implementation comes last.
    /// </summary>
    private readonly List<IComponentRegistration> _defaultImplementations = new();

    /// <summary>
    /// 来自源代码的服务实现列表。源优先于保留默认实现。源代码中的实现以保留默认顺序枚举，因此最默认的实现排在第一位。
    ///  List of service implementations coming from sources. Sources have priority over preserve-default implementations.
    ///  Implementations from sources are enumerated in preserve-default order, so the most default implementation comes first.
    /// </summary>
    private List<IComponentRegistration>? _sourceImplementations;

    /// <summary>
    /// 使用PreserveExistingDefaults 选项指定的显式服务实现列表。按保留默认值的顺序枚举，因此最默认的实现排在第一位。
    ///  List of explicit service implementations specified with the PreserveExistingDefaults option.
    ///  Enumerated in preserve-defaults order, so the most default implementation comes first.
    /// </summary>
    private List<IComponentRegistration>? _preserveDefaultImplementations;

    [SuppressMessage("Microsoft.ApiDesignGuidelines", "CA2213", Justification = "The creator of the component registration is responsible for disposal.")]
    private IComponentRegistration? _defaultImplementation;

    /// <summary>
    /// 用于记账，使同一来源不会被查询两次（可能为空）。
    /// Used for bookkeeping so that the same source is not queried twice (may be null).
    /// </summary>
    private Queue<IRegistrationSource>? _sourcesToQuery;

    private IResolvePipeline? _resolvePipeline;

    private IResolvePipelineBuilder? _customPipelineBuilder;

    /// <summary>
    /// 初始化<see cref="ServiceRegistrationInfo"/>类的新实例。
    /// Initializes a new instance of the <see cref="ServiceRegistrationInfo"/> class.
    /// </summary>
    /// <param name="service">跟踪服务。 The tracked service.</param>
    public ServiceRegistrationInfo(Service service) => _service = service;

    /// <summary>
    /// 获取一个值，该值指示是否在首次请求服务时进行初始化（例如从源读取）。然后，该值将设置为true。在初始化之前调用此类型的许多方法是错误的。
    /// Gets a value indicating whether the first time a service is requested, initialization (e.g. reading from sources)
    /// happens. This value will then be set to true. Calling many methods on this type before
    /// initialization is an error.
    /// </summary>
    public bool IsInitialized
    {
        get => _isInitialized;
        private set => _isInitialized = value;
    }

    /// <summary>
    /// 获取或设置表示当前初始化深度的值。初始化的服务块将始终为零。
    /// Gets or sets a value representing the current initialization depth. Will always be zero for initialized service blocks.
    /// </summary>
    public int InitializationDepth { get; set; }

    /// <summary>
    /// 获取已知的实现。第一个实现是默认实现。
    /// Gets the known implementations. The first implementation is a default one.
    /// </summary>
    public IEnumerable<IComponentRegistration> Implementations
    {
        get
        {
            RequiresInitialization();

            if (_fixedRegistration is object)
            {
                yield return _fixedRegistration;
            }

            var defaultImpls = _defaultImplementations;

            for (var defaultReverseIdx = defaultImpls.Count - 1; defaultReverseIdx >= 0; defaultReverseIdx--)
            {
                yield return defaultImpls[defaultReverseIdx];
            }

            if (_sourceImplementations is not null)
            {
                foreach (var item in _sourceImplementations)
                {
                    yield return item;
                }
            }

            if (_preserveDefaultImplementations is not null)
            {
                foreach (var item in _preserveDefaultImplementations)
                {
                    yield return item;
                }
            }
        }
    }

    /// <summary>
    /// 获取服务管道。如果未初始化，将抛出。
    /// Gets the service pipeline. Will throw if not initialized.
    /// </summary>
    public IResolvePipeline ServicePipeline => _resolvePipeline ?? throw new InvalidOperationException(ServiceRegistrationInfoResources.NotInitialized);

    /// <summary>
    /// 获取针对服务注册的所有中间件的集合（不包括默认中间件）。
    /// Gets the set of all middleware registered against the service (excluding the default middleware).
    /// </summary>
    public IEnumerable<IResolveMiddleware> ServiceMiddleware
    {
        get
        {
            if (_customPipelineBuilder is null)
            {
                return Enumerable.Empty<IResolveMiddleware>();
            }

            return _customPipelineBuilder.Middleware.Where(t => !ServicePipelines.IsDefaultMiddleware(t));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RequiresInitialization()
    {
        // Implementations can be read by consumers while we are inside an initialization window,
        // even when the initialization hasn't finished yet.
        // The InitializationDepth property is always 0 outside of the lock-protected initialization block.
        if (InitializationDepth == 0 && !IsInitialized)
        {
            throw new InvalidOperationException(ServiceRegistrationInfoResources.NotInitialized);
        }
    }

    /// <summary>
    /// 获取一个值，该值指示是否已知任何实现。
    /// Gets a value indicating whether any implementations are known.
    /// </summary>
    public bool IsRegistered
    {
        get
        {
            RequiresInitialization();
            return Any;
        }
    }

    /// <summary>
    /// 获取一个值，该值指示此注册信息是否注册了任何自定义服务中间件。
    /// Gets a value indicating whether this registration info has any custom service middleware registered.
    /// </summary>
    public bool HasCustomServiceMiddleware
    {
        get
        {
            RequiresInitialization();
            return _customPipelineBuilder is not null;
        }
    }

    private bool Any =>
        _defaultImplementations.Count > 0 ||
        _sourceImplementations is not null ||
        _preserveDefaultImplementations is not null;

    /// <summary>
    /// 为服务添加实现。
    /// Add an implementation for the service.
    /// </summary>
    /// <param name="registration">The component registration.</param>
    /// <param name="preserveDefaults">Whether to preserve the defaults.</param>
    /// <param name="originatedFromSource">Whether the registration originated from a dynamic source.</param>
    public void AddImplementation(IComponentRegistration registration, bool preserveDefaults, bool originatedFromSource)
    {
        if (registration.Options.HasOption(RegistrationOptions.Fixed))
        {
            if (_fixedRegistration is null || !originatedFromSource)
            {
                _fixedRegistration = registration;
            }
        }
        else if (preserveDefaults)
        {
            if (originatedFromSource)
            {
                _sourceImplementations ??= new List<IComponentRegistration>();
                _sourceImplementations.Add(registration);
            }
            else
            {
                _preserveDefaultImplementations ??= new List<IComponentRegistration>();
                _preserveDefaultImplementations.Add(registration);
            }
        }
        else
        {
            if (originatedFromSource)
            {
                throw new ArgumentOutOfRangeException(nameof(originatedFromSource));
            }

            _defaultImplementations.Add(registration);
        }

        _defaultImplementation = null;
    }

    /// <summary>
    /// Use the specified piece of middleware in the service pipeline.
    /// </summary>
    /// <param name="middleware">The middleware.</param>
    /// <param name="insertionMode">The insertion mode for the pipeline.</param>
    public void UseServiceMiddleware(IResolveMiddleware middleware, MiddlewareInsertionMode insertionMode = MiddlewareInsertionMode.EndOfPhase)
    {
        _customPipelineBuilder ??= new ResolvePipelineBuilder(PipelineType.Service);
        _customPipelineBuilder.Use(middleware, insertionMode);
    }

    /// <summary>
    /// Use the multiple specified pieces of middleware in the service pipeline.
    /// </summary>
    /// <param name="middleware">The set of middleware.</param>
    /// <param name="insertionMode">The insertion mode.</param>
    public void UseServiceMiddlewareRange(IEnumerable<IResolveMiddleware> middleware, MiddlewareInsertionMode insertionMode = MiddlewareInsertionMode.EndOfPhase)
    {
        if (!middleware.Any())
        {
            return;
        }

        _customPipelineBuilder ??= new ResolvePipelineBuilder(PipelineType.Service);
        _customPipelineBuilder.UseRange(middleware, insertionMode);
    }

    /// <summary>
    /// Attempts to access the implementing registration for this service, selecting the correct one based on defaults and all known registrations.
    /// </summary>
    /// <param name="registration">The output registration.</param>
    /// <returns>True if a registration was found; false otherwise.</returns>
    public bool TryGetRegistration([NotNullWhen(returnValue: true)] out IComponentRegistration? registration)
    {
        RequiresInitialization();

        registration = _defaultImplementation ??= _fixedRegistration ??
                                                  _defaultImplementations.LastOrDefault() ??
                                                  _sourceImplementations?.First() ??
                                                  _preserveDefaultImplementations?.First();

        return registration is not null;
    }

    /// <summary>
    /// Gets a value indicating whether this service info is initializing.
    /// </summary>
    public bool IsInitializing => !IsInitialized && _sourcesToQuery is not null;

    /// <summary>
    /// Gets a value indicating whether there are any sources left to query.
    /// </summary>
    public bool HasSourcesToQuery => IsInitializing && _sourcesToQuery!.Count != 0;

    /// <summary>
    /// Begin the initialization process for this service info, given the set of dynamic sources.
    /// </summary>
    /// <param name="sources">The set of sources.</param>
    public void BeginInitialization(IEnumerable<IRegistrationSource> sources)
    {
        IsInitialized = false;
        _sourcesToQuery = new Queue<IRegistrationSource>(sources);

        // Build the pipeline during service info initialization, so that sources can access it
        // while getting a registration recursively.
        _resolvePipeline ??= BuildPipeline();
    }

    /// <summary>
    /// Skip a given source in the set of dynamic sources.
    /// </summary>
    /// <param name="source">The source to skip.</param>
    public void SkipSource(IRegistrationSource source)
    {
        EnforceDuringInitialization();

        // _sourcesToQuery always non-null during Initialization.
        _sourcesToQuery = new Queue<IRegistrationSource>(_sourcesToQuery!.Where(rs => rs != source));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnforceDuringInitialization()
    {
        if (!IsInitializing)
        {
            throw new InvalidOperationException(ServiceRegistrationInfoResources.NotDuringInitialization);
        }
    }

    /// <summary>
    /// Dequeue the next registration source.
    /// </summary>
    /// <returns>The source.</returns>
    public IRegistrationSource DequeueNextSource()
    {
        EnforceDuringInitialization();

        // _sourcesToQuery always non-null during initialization
        return _sourcesToQuery!.Dequeue();
    }

    /// <summary>
    /// Complete initialization of the service info.
    /// </summary>
    public void CompleteInitialization()
    {
        EnforceDuringInitialization();

        IsInitialized = true;
        _sourcesToQuery = null;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return _service.ToString();
    }

    private IResolvePipeline BuildPipeline()
    {
        // Build the custom service pipeline (if we need to).
        if (_customPipelineBuilder is object)
        {
            // Add the default stages.
            _customPipelineBuilder.UseRange(ServicePipelines.DefaultMiddleware);

            // Add the default.
            return _customPipelineBuilder.Build();
        }
        else
        {
            // Nothing custom, use an empty pipeline.
            return ServicePipelines.DefaultServicePipeline;
        }
    }

    /// <summary>
    /// 创建未初始化的<see cref="ServiceRegistrationInfo"/>的副本，保留现有注册和自定义中间件
    /// Creates a copy of an uninitialized <see cref="ServiceRegistrationInfo"/>, preserving existing registrations and custom middleware.
    /// </summary>
    /// <returns>A new service registration info block.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service registration has been initialized already.</exception>
    public ServiceRegistrationInfo CloneUninitialized()
    {
        if (InitializationDepth != 0 || IsInitializing || IsInitialized)
        {
            throw new InvalidOperationException(ServiceRegistrationInfoResources.NotAfterInitialization);
        }

        var copy = new ServiceRegistrationInfo(_service)
        {
            _fixedRegistration = _fixedRegistration,
            _defaultImplementation = _defaultImplementation,
        };

        if (_sourceImplementations is not null)
        {
            copy._sourceImplementations = new List<IComponentRegistration>(_sourceImplementations);
        }

        if (_preserveDefaultImplementations is not null)
        {
            copy._preserveDefaultImplementations = new List<IComponentRegistration>(_preserveDefaultImplementations);
        }

        copy._defaultImplementations.AddRange(_defaultImplementations);

        if (_customPipelineBuilder is object)
        {
            copy._customPipelineBuilder = _customPipelineBuilder.Clone();
        }

        return copy;
    }

    /// <inheritdoc/>
    IEnumerable<IResolveMiddleware> IResolvePipelineBuilder.Middleware => ServiceMiddleware;

    /// <inheritdoc/>
    PipelineType IResolvePipelineBuilder.Type => PipelineType.Service;

    /// <inheritdoc/>
    IResolvePipeline IResolvePipelineBuilder.Build()
    {
        throw new InvalidOperationException(ServiceRegistrationInfoResources.ServicePipelineCannotBeBuilt);
    }

    /// <inheritdoc/>
    IResolvePipelineBuilder IResolvePipelineBuilder.Use(IResolveMiddleware middleware, MiddlewareInsertionMode insertionMode)
    {
        UseServiceMiddleware(middleware, insertionMode);
        return this;
    }

    /// <inheritdoc/>
    IResolvePipelineBuilder IResolvePipelineBuilder.UseRange(IEnumerable<IResolveMiddleware> middleware, MiddlewareInsertionMode insertionMode)
    {
        UseServiceMiddlewareRange(middleware, insertionMode);
        return this;
    }

    /// <inheritdoc/>
    IResolvePipelineBuilder IResolvePipelineBuilder.Clone()
    {
        if (_customPipelineBuilder is null)
        {
            return new ResolvePipelineBuilder(PipelineType.Service);
        }

        return _customPipelineBuilder.Clone();
    }
}
