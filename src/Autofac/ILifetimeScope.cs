﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if NET5_0_OR_GREATER
using System.Runtime.Loader;
#endif

using Autofac.Builder;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;

namespace Autofac;

/// <summary>
/// <para>
/// <see cref="ILifetimeScope"/>跟踪组件实例的实例化。它定义了一个共享和配置实例的边界。
/// An <see cref="ILifetimeScope"/> tracks the instantiation of component instances.
/// It defines a boundary in which instances are shared and configured.
/// </para>
/// <para>
/// Disposing an <see cref="ILifetimeScope"/> will dispose any components that were
/// newly created when resolving from it, provided those components were not registered as
/// <see cref="IRegistrationBuilder{TLimit, TActivatorData, TRegistrationStyle}.ExternallyOwned"/>.
/// </para>
/// </summary>
/// <example>
/// <code>
/// // See IContainer for definition of the container variable
/// using (var requestScope = container.BeginLifetimeScope())
/// {
///     // Note that handler is resolved from requestScope, not
///     // from the container:
///
///     var handler = requestScope.Resolve&lt;IRequestHandler&gt;();
///     handler.Handle(request);
///
///     // When requestScope is disposed, all resources used in processing
///     // the request will be released.
/// }
/// </code>
/// </example>
/// <remarks>
/// <para>
/// All long-running applications should resolve components via an
/// <see cref="ILifetimeScope"/>. Choosing the duration of the lifetime is application-
/// specific.
/// </para>
/// <para>
/// The standard Autofac integrations are already configured
/// to create and dispose <see cref="ILifetimeScope"/>s as appropriate. For example, the
/// integration with ASP.NET Core (via the .NET DI library) will create and dispose
/// one <see cref="ILifetimeScope"/> per HTTP request.
/// </para>
/// <para>
/// Most <see cref="ILifetimeScope"/> functionality is provided by extension methods
/// on the inherited <see cref="IComponentContext"/> interface.
/// </para>
/// </remarks>
/// <seealso cref="IContainer"/>
/// <seealso cref="IComponentContext"/>
/// <seealso cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerMatchingLifetimeScope"/>
/// <seealso cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerLifetimeScope"/>
/// <seealso cref="InstanceSharing"/>
/// <seealso cref="IComponentLifetime"/>
public interface ILifetimeScope : IComponentContext, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Begin a new nested scope. Component instances created via the new scope
    /// will be disposed along with it.
    /// </summary>
    /// <returns>A new lifetime scope.</returns>
    ILifetimeScope BeginLifetimeScope();

    /// <summary>
    /// Begin a new nested scope. Component instances created via the new scope
    /// will be disposed along with it.
    /// </summary>
    /// <param name="tag">The tag applied to the <see cref="ILifetimeScope"/>.</param>
    /// <returns>A new lifetime scope.</returns>
    ILifetimeScope BeginLifetimeScope(object tag);

    /// <summary>
    /// 开始一个新的嵌套作用域，其中包含可用的其他组件。通过新作用域创建的组件实例将与其一起处置。
    /// Begin a new nested scope, with additional components available to it.
    /// Component instances created via the new scope will be disposed along with it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any components registered in the sub-scope will only be resolvable from this new scope,
    /// or additional child scopes subsequently resolved from it.
    /// </para>
    /// <para>
    /// Components registered in the sub-scope that provide services already registered
    /// in a parent <see cref="ILifetimeScope"/> (or root <see cref="IContainer"/>) will
    /// override that same service registration when resolving instances.
    /// </para>
    /// <para>
    /// If you resolve an <see cref="IEnumerable{TService}"/> in the new scope, the returned set of components will
    /// include both components registered in the new sub-scope, plus all registered components all the way back
    /// up to the root <see cref="IContainer"/>.
    /// </para>
    /// <para>
    /// Resolving a service from this scope that was only defined in a parent <see cref="ILifetimeScope"/> (or root <see cref="IContainer"/>)
    /// will result in a component 'owned' by the scope it was registered in (rather than this new scope).
    /// For example, if you registered a <c>SingleInstance()</c> component in the root container, but resolved it in the new sub-scope, the instance you get will
    /// be attached to the root container, not your scope, so the same instance will be shared by all other <see cref="ILifetimeScope"/> instances.
    /// </para>
    /// <para>
    /// <c>SingleInstance()</c> components registered in this scope will be held as long as this scope exists, but will be discarded
    /// when the scope is disposed.
    /// </para>
    /// </remarks>
    /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/>
    /// that adds component registrations visible only in the new scope.</param>
    /// <returns>A new lifetime scope.</returns>
    ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction);

    /// <summary>
    /// Begin a new nested scope, with additional components available to it.
    /// Component instances created via the new scope
    /// will be disposed along with it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any components registered in the sub-scope will only be resolvable from this new scope,
    /// or additional child scopes subsequently resolved from it.
    /// </para>
    /// <para>
    /// Components registered in the sub-scope that provide services already registered
    /// in a parent <see cref="ILifetimeScope"/> (or root <see cref="IContainer"/>) will
    /// override that same service registration when resolving instances.
    /// </para>
    /// <para>
    /// If you resolve an <see cref="IEnumerable{TService}"/> in the new scope, the returned set of components will
    /// include both components registered in the new sub-scope, plus all registered components all the way back
    /// up to the root <see cref="IContainer"/>.
    /// </para>
    /// <para>
    /// Resolving a service from this scope that was only defined in a parent <see cref="ILifetimeScope"/> (or root <see cref="IContainer"/>)
    /// will result in a component 'owned' by the scope it was registered in (rather than this new scope).
    /// For example, if you registered a <c>SingleInstance()</c> component in the root container, but resolved it in the new sub-scope, the instance you get will
    /// be attached to the root container, not your scope, so the same instance will be shared by all other <see cref="ILifetimeScope"/> instances.
    /// </para>
    /// <para>
    /// <c>SingleInstance()</c> components registered in this scope will be held as long as this scope exists, but will be discarded
    /// when the scope is disposed.
    /// </para>
    /// </remarks>
    /// <param name="tag">The tag applied to the <see cref="ILifetimeScope"/>.</param>
    /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/>
    /// that adds component registrations visible only in the new scope.</param>
    /// <returns>A new lifetime scope.</returns>
    ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction);

#if NET5_0_OR_GREATER
    /// <summary>
    /// Begin a new anonymous sub-scope, with additional components available to it that may be dynamically
    /// loaded from the provided <see cref="AssemblyLoadContext"/>.
    /// Component instances created via the new scope
    /// will be disposed along with it.
    /// </summary>
    /// <param name="loadContext">
    /// A <see cref="AssemblyLoadContext"/> to associate to the created <see cref="ILifetimeScope"/>.
    /// </param>
    /// <param name="configurationAction">
    /// Action on a <see cref="ContainerBuilder"/>
    /// that adds component registrations visible only in the new scope.
    /// </param>
    /// <returns>A new lifetime scope.</returns>
    /// <example>
    /// <code>
    /// IContainer cr = // ...
    /// AssemblyLoadContext pluginContext = // ...
    /// using (var lifetime = cr.BeginLoadContextLifetimeScope(pluginContext, builder =&gt; {
    ///         var assembly = pluginContext.LoadFromAssemblyPath("Plugins/plugin.dll");
    ///         builder.RegisterType(assembly.GetType("PluginEntryPoint")).As&lt;IPlugin&gt;();
    /// {
    ///     var plugin = lifetime.Resolve&lt;IPlugin&gt;();
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// When the returned lifetime scope is disposed, the provided
    /// <paramref name="loadContext"/> can be unloaded, in that
    /// Autofac will no longer have any references to types loaded from <paramref name="loadContext"/>.
    /// However if you have captured references to types from the loaded assemblies manually, you still may not be able
    /// to unload.
    /// </remarks>
    ILifetimeScope BeginLoadContextLifetimeScope(AssemblyLoadContext loadContext, Action<ContainerBuilder> configurationAction);

    /// <summary>
    /// Begin a new tagged sub-scope, with additional components available to it that may be dynamically
    /// loaded from the provided <see cref="AssemblyLoadContext"/>.
    /// Component instances created via the new scope
    /// will be disposed along with it.
    /// </summary>
    /// <param name="tag">The tag applied to the <see cref="ILifetimeScope"/>.</param>
    /// <param name="loadContext">
    /// A <see cref="AssemblyLoadContext"/> to associate to the created <see cref="ILifetimeScope"/>.
    /// </param>
    /// <param name="configurationAction">
    /// Action on a <see cref="ContainerBuilder"/>
    /// that adds component registrations visible only in the new scope.
    /// </param>
    /// <returns>A new lifetime scope.</returns>
    /// <example>
    /// <code>
    /// IContainer cr = // ...
    /// AssemblyLoadContext pluginContext = // ...
    /// using (var lifetime = cr.BeginLoadContextLifetimeScope(pluginContext, builder =&gt; {
    ///         var assembly = pluginContext.LoadFromAssemblyPath("Plugins/plugin.dll");
    ///         builder.RegisterType(assembly.GetType("PluginEntryPoint")).As&lt;IPlugin&gt;();
    /// {
    ///     var plugin = lifetime.Resolve&lt;IPlugin&gt;();
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// When the returned lifetime scope is disposed, the provided
    /// <paramref name="loadContext"/> can be unloaded, in that
    /// Autofac will no longer have any references to types loaded from <paramref name="loadContext"/>.
    /// However if you have captured references to types from the loaded assemblies manually, you still may not be able
    /// to unload.
    /// </remarks>
    ILifetimeScope BeginLoadContextLifetimeScope(object tag, AssemblyLoadContext loadContext, Action<ContainerBuilder> configurationAction);
#endif

    /// <summary>
    /// Gets the disposer associated with this <see cref="ILifetimeScope"/>.
    /// Component instances can be associated with it manually if required.
    /// </summary>
    /// <remarks>Typical usage does not require interaction with this member- it
    /// is used when extending the container.</remarks>
    IDisposer Disposer { get; }

    /// <summary>
    /// Gets the tag applied to the <see cref="ILifetimeScope"/>.
    /// </summary>
    /// <remarks>Tags allow a level in the lifetime hierarchy to be identified.
    /// In most applications, tags are not necessary.</remarks>
    /// <seealso cref="IRegistrationBuilder{TLimit,TActivatorData,TRegistrationStyle}.InstancePerMatchingLifetimeScope"/>
    object Tag { get; }

    /// <summary>
    /// Fired when a new scope based on the current scope is beginning.
    /// </summary>
    event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;

    /// <summary>
    /// Fired when this scope is ending.
    /// </summary>
    event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;

    /// <summary>
    /// Fired when a resolve operation is beginning in this scope.
    /// </summary>
    event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;
}
