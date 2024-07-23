// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Core.Resolving.Pipeline;

namespace Autofac.Core.Registration;

/// <summary>
/// 跟踪已注册服务的状态。 Keeps track of the status of registered services.
/// </summary>
internal interface IRegisteredServicesTracker : IDisposable, IAsyncDisposable, IComponentRegistryServices
{
    /// <summary>
    /// 将注册添加到已注册服务列表中。 Adds a registration to the list of registered services.
    /// </summary>
    /// <param name="registration">The registration to add.</param>
    /// <param name="preserveDefaults">Indicates whether the defaults should be preserved.</param>
    /// <param name="originatedFromDynamicSource">Indicates whether this is an explicitly added registration or that it has been added by a dynamic registration source.</param>
    void AddRegistration(IComponentRegistration registration, bool preserveDefaults, bool originatedFromDynamicSource = false);

    /// <summary>
    /// 向服务添加一块中间件。 Adds a piece of middleware to a service.
    /// </summary>
    /// <param name="service">The service to add to.</param>
    /// <param name="middleware">The middleware.</param>
    /// <param name="insertionMode">The insertion mode.</param>
    void AddServiceMiddleware(Service service, IResolveMiddleware middleware, MiddlewareInsertionMode insertionMode = MiddlewareInsertionMode.EndOfPhase);

    /// <summary>
    /// 添加一个注册源，提供即时注册。
    /// Add a registration source that will provide registrations on-the-fly.
    /// </summary>
    /// <param name="source">The source to register.</param>
    void AddRegistrationSource(IRegistrationSource source);

    /// <summary>
    /// 添加一个服务中间件源，该源将动态提供服务中间件。
    /// Adds a service middleware source that will provide service middleware on-the-fly.
    /// </summary>
    /// <param name="serviceMiddlewareSource">The source to register.</param>
    void AddServiceMiddlewareSource(IServiceMiddlewareSource serviceMiddlewareSource);

    /// <summary>
    /// 无论何时注册组件，无论是显式注册还是通过<see cref='IRegistrationSource'/>注册，都会触发。
    /// Fired whenever a component is registered - either explicitly or via an <see cref="IRegistrationSource"/>.
    /// </summary>
    event EventHandler<IComponentRegistration> Registered;

    /// <summary>
    /// 当一个<see-cref='IRegicationSource'/>被添加到注册表时触发。
    /// Fired when an <see cref="IRegistrationSource"/> is added to the registry.
    /// </summary>
    event EventHandler<IRegistrationSource> RegistrationSourceAdded;

    /// <summary>
    /// 应在构建<see cref='ComponentRegistry'/>之前调用，以指示跟踪器已完成，并且请求的服务信息不应再是短暂的。
    /// Should be called prior to the construction of a <see cref="ComponentRegistry" /> to
    /// indicate that the tracker is complete, and requested service information should no longer be ephemeral.
    /// </summary>
    void Complete();

    /// <summary>
    /// 获取已注册的组件。 Gets the registered components.
    /// </summary>
    IEnumerable<IComponentRegistration> Registrations { get; }

    /// <summary>
    /// 获取注册表使用的注册源。 Gets the registration sources that are used by the registry.
    /// </summary>
    IEnumerable<IRegistrationSource> Sources { get; }

    /// <summary>
    /// 获取已注册的服务中间件源的集合。 Gets the set of registered service middleware sources.
    /// </summary>
    IEnumerable<IServiceMiddlewareSource> ServiceMiddlewareSources { get; }

    /// <summary>
    /// 获取服务的已配置服务中间件集。 Gets the set of configured service middleware for a service.
    /// </summary>
    /// <param name="service">The service to look up.</param>
    /// <returns>The set of middleware.</returns>
    IEnumerable<IResolveMiddleware> ServiceMiddlewareFor(Service service);

    /// <summary>
    /// 尝试查找指定服务的默认服务注册。 Attempts to find a default service registration for the specified service.
    /// </summary>
    /// <param name="service">The service to look up.</param>
    /// <param name="serviceRegistration">The default registration for the service.</param>
    /// <returns>True if a registration exists.</returns>
    bool TryGetServiceRegistration(Service service, out ServiceRegistration serviceRegistration);

    /// <summary>
    /// 在确保已调用可能提供<paramref name='service'/>的任何动态注册源后，从可用的服务注册中选择。
    /// Selects from the available service registrations after ensuring that any
    /// dynamic registration sources that may provide <paramref name="service"/>
    /// have been invoked.
    /// </summary>
    /// <param name="service">The service for which registrations are sought.</param>
    /// <returns>Service registrations supporting <paramref name="service"/>.</returns>
    IEnumerable<ServiceRegistration> ServiceRegistrationsFor(Service service);
}
