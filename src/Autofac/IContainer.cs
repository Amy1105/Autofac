﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Diagnostics;

namespace Autofac;

/// <summary>
/// 创建、连接依赖关系并管理一组组件的生命周期。大多数<see-cref="IContainer"/>实例
/// 都是由<see-cref="ContainerBuilder"/>创建的。
/// Creates, wires dependencies and manages lifetime for a set of components.
/// Most instances of <see cref="IContainer"/> are created by a <see cref="ContainerBuilder"/>.
/// </summary>
/// <example>
/// <code>
/// // See ContainerBuilder for the definition of the builder variable
/// using (var container = builder.Build())
/// {
///     var program = container.Resolve&lt;Program&gt;();
///     program.Run();
/// }
/// </code>
/// </example>
/// <remarks>
/// Most <see cref="IContainer"/> functionality is provided by extension methods
/// on the inherited <see cref="IComponentContext"/> interface.
/// </remarks>
/// <seealso cref="ILifetimeScope"/>
/// <seealso cref="IComponentContext"/>
/// <seealso cref="ResolutionExtensions"/>
/// <seealso cref="ContainerBuilder"/>
public interface IContainer : ILifetimeScope
{
    /// <summary>
    /// Gets the <see cref="DiagnosticListener"/> to which
    /// trace events should be written.
    /// 获取跟踪事件应写入的<see-cref='DiagnosticListener'/>。
    /// </summary>
    DiagnosticListener DiagnosticSource { get; }
}
