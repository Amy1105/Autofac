// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;
using Autofac.Features.Decorators;

namespace Autofac.Core.Resolving.Pipeline;

/// <inheritdoc />
public abstract class ResolveRequestContext : IComponentContext
{
    /// <summary>
    /// 获取对所属解析操作的引用（该操作可能包含多个嵌套请求）。
    /// Gets a reference to the owning resolve operation (which might encompass multiple nested requests).
    /// </summary>
    public abstract IResolveOperation Operation { get; }

    /// <summary>
    /// 获取或设置生命周期范围，该范围将用于稍后激活管道中的任何组件。
    /// 避免直接从该范围解析实例；它们不会作为同一操作的一部分被追踪。
    /// Gets or sets the lifetime scope that will be used for the activation of any components later in the pipeline.
    /// Avoid resolving instances directly from this scope; they will not be traced as part of the same operation.
    /// </summary>
    public abstract ISharingLifetimeScope ActivationScope { get; protected set; }

    /// <summary>
    ///获取当前请求中正在解析的组件注册。
    /// Gets the component registration that is being resolved in the current request.
    /// </summary>
    public abstract IComponentRegistration Registration { get; }

    /// <summary>
    /// 获取当前请求中正在解析的服务。
    /// Gets the service that is being resolved in the current request.
    /// </summary>
    public abstract Service Service { get; }

    /// <summary>
    /// 获取装饰器请求的目标注册。
    /// Gets the target registration for decorator requests.
    /// </summary>
    public abstract IComponentRegistration? DecoratorTarget { get; }

    /// <summary>
    /// 获取或设置将作为解析请求的结果返回的实例。在管道备份过程中，调用链中的下一个中间件后，此值将填充已解析的实例。
    /// 检查<see cref='NewInstanceActivated'/> 属性，以确定此处的对象是新激活的实例，还是之前激活的共享实例。
    /// Gets or sets the instance that will be returned as the result of the
    /// resolve request. On the way back up the pipeline, after calling the next
    /// middleware in the chain, this value will be populated with the resolved
    /// instance. Check the <see cref="NewInstanceActivated"/> property to
    /// determine whether the object here was a newly activated instance, or a
    /// shared instance previously activated.
    /// </summary>
    [DisallowNull]
    public abstract object? Instance { get; set; }

    /// <summary>
    /// 获取一个值，该值指示解析的<see-cref='Instance'/>是在此请求期间激活的组件的新实例，还是已检索到的现有共享实例。
    /// Gets a value indicating whether the resolved <see cref="Instance"/> is a new instance of a component has been activated during this request,
    /// or an existing shared instance that has been retrieved.
    /// </summary>
    public abstract bool NewInstanceActivated { get; }

    /// <summary>
    /// 获取跟踪事件应写入的<see-cref=“DiagnosticListener”/>。
    /// Gets the <see cref="DiagnosticListener"/> to which trace events should be written.
    /// </summary>
    public abstract DiagnosticListener DiagnosticSource { get; }

    /// <summary>
    /// 获取或设置当前解析参数。可以使用<see cref='ChangeParameters(IEnumerable{Parameter})'/> 方法更改这些参数。
    /// Gets or sets the current resolve parameters.
    /// These can be changed using the <see cref="ChangeParameters(IEnumerable{Parameter})"/> method.
    /// </summary>
    public abstract IEnumerable<Parameter> Parameters { get; protected set; }

    /// <summary>
    /// 获取或设置此请求到达的管道的阶段。
    /// Gets or sets the phase of the pipeline reached by this request.
    /// </summary>
    public abstract PipelinePhase PhaseReached { get; set; }

    /// <summary>
    /// 获取或设置请求的活动装饰器上下文。
    /// Gets or sets the active decorator context for the request.
    /// </summary>
    public abstract DecoratorContext? DecoratorContext { get; set; }

    /// <summary>
    /// 提供一个在当前请求完成时触发的事件。只有当整个<see cref='IResolveOperation'/> 完成时，请求才会被视为“完成”。
    /// Provides an event that will fire when the current request completes.
    /// Requests will only be considered 'complete' when the overall <see cref="IResolveOperation"/> is completing.
    /// </summary>
    public abstract event EventHandler<ResolveRequestCompletingEventArgs>? RequestCompleting;

    /// <summary>
    /// 使用此方法更改此请求中使用的<see cref='ISharingLifetimeScope'/>。
    /// 更改此范围也将更改此上下文中可用的<see cref='IComponentRegistry'/>。
    /// Use this method to change the <see cref="ISharingLifetimeScope"/> that is used in this request. Changing this scope will
    /// also change the <see cref="IComponentRegistry"/> available in this context.
    /// </summary>
    /// <param name="newScope">The new lifetime scope.</param>
    public abstract void ChangeScope(ISharingLifetimeScope newScope);

    /// <summary>
    /// 更改处理此请求时使用的参数集。
    /// Change the set of parameters being used in the processing of this request.
    /// </summary>
    /// <param name="newParameters">The new set of parameters.</param>
    public abstract void ChangeParameters(IEnumerable<Parameter> newParameters);

    /// <inheritdoc/>
    public abstract IComponentRegistry ComponentRegistry { get; }

    /// <inheritdoc/>
    public abstract object ResolveComponent(in ResolveRequest request);
}
