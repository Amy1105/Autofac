// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac;

/// <summary>
/// When implemented by a component, an instance of the component will be resolved
/// and started as soon as the container is built. Autofac will not call the Start()
/// method when subsequent instances are resolved. If this behavior is required, use
/// an <c>OnActivated()</c> event handler instead.
/// 当由组件实现时，一旦构建了容器，就会解析并启动组件的实例。解析后续实例时，Autofac不会调用Start（）方法。
/// 如果需要此行为，则使用<c>OnActivated（）</c>事件处理程序。
/// </summary>
/// <remarks>
/// <para>
/// For equivalent "Stop" functionality, implement <see cref="IDisposable"/>. Autofac
/// will always dispose a component before any of its dependencies (except in the presence
/// of circular dependencies, in which case the components in the cycle are disposed in
/// reverse-construction order.)
/// </para>
/// <para>
/// Components that implement <see cref="IStartable"/> and configured with a InstancePerLifetimeScope()
/// lifetime are still only started *once*, in the root <see cref="IContainer"/> or <see cref="ILifetimeScope"/>
/// where the component was registered.
/// </para>
/// </remarks>
public interface IStartable
{
    /// <summary>
    /// Perform once-off startup processing.
    /// </summary>
    void Start();
}
