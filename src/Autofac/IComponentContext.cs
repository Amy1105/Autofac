// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Core;
using Autofac.Core.Registration;

namespace Autofac;

/// <summary>
/// The context in which a service can be accessed or a component's
/// dependencies resolved. Disposal of a context will dispose any owned
/// components.
/// 可以访问服务或解决组件依赖关系的上下文。上下文的处置将处置所有拥有的组件。
/// </summary>
public interface IComponentContext
{
    /// <summary>
    /// Gets the associated services with the components that provide them.
    /// </summary>
    IComponentRegistry ComponentRegistry { get; }

    /// <summary>
    /// Resolve an instance of the provided registration within the context.
    /// 在上下文中解析所提供注册的实例。
    /// </summary>
    /// <param name="request">The resolve request.</param>
    /// <returns>
    /// The component instance.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    object ResolveComponent(in ResolveRequest request);
}
