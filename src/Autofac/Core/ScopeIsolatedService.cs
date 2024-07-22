// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Core;

/// <summary>
/// <see cref=“Service”/>的装饰器，表示服务应该与当前范围隔离开来，因此对它的引用不会保留。允许稍后卸载隔离的服务。
/// Decorator for a <see cref="Service"/> that indicates the service should
/// be isolated from the current scope so references to it are not
/// retained. Enables isolated services to be later unloaded.
/// </summary>
internal class ScopeIsolatedService : Service
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeIsolatedService"/> class.
    /// </summary>
    /// <param name="service">The service to wrap for isolation.</param>
    public ScopeIsolatedService(Service service)
    {
        Service = service;
    }

    /// <summary>
    /// Gets the actual service that has been isolated.
    /// </summary>
    public Service Service { get; }

    /// <inheritdoc />
    public override string Description => Service.Description;
}
