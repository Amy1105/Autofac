﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Globalization;
using System.Reflection;
using Autofac.Core;
using Autofac.Core.Registration;

namespace Autofac;

/// <summary>
/// 用户定义模块的基类。模块可以将一组相关组件添加到容器中（<see cref="Load"/>）
/// 或将跨领域功能附加到其他组件中（<see cref="AttachToComponent Registry"/>。
/// Base class for user-defined modules. Modules can add a set of related components
/// to a container (<see cref="Load"/>) or attach cross-cutting functionality
/// to other components (<see cref="AttachToComponentRegistration"/>.
/// Modules are given special support in the XML configuration feature - see
/// https://autofac.readthedocs.io/en/latest/configuration/modules.html.
/// </summary>
/// <remarks>Provides a user-friendly way to implement <see cref="IModule"/>
/// via <see cref="ContainerBuilder"/>.</remarks>
/// <example>
/// Defining a module:
/// <code>
/// public class DataAccessModule : Module
/// {
///     public string ConnectionString { get; set; }
///
///     public override void Load(ContainerBuilder moduleBuilder)
///     {
///         moduleBuilder.RegisterGeneric(typeof(MyRepository&lt;&gt;))
///             .As(typeof(IRepository&lt;&gt;))
///             .InstancePerLifetimeScope();
///
///         moduleBuilder.Register(c =&gt; new MyDbConnection(ConnectionString))
///             .As&lt;IDbConnection&gt;()
///             .InstancePerMatchingLifetimeScope(WebLifetime.Request);
///     }
/// }
/// </code>
/// Using the module...
/// <code>
/// var builder = new ContainerBuilder();
/// builder.RegisterModule(new DataAccessModule { ConnectionString = "..." });
/// var container = builder.Build();
/// var customers = container.Resolve&lt;IRepository&lt;Customer&gt;&gt;();
/// </code>
/// </example>
public abstract class Module : IModule
{
    /// <summary>
    /// Apply the module to the component registry.
    /// </summary>
    /// <param name="componentRegistry">Component registry to apply configuration to.</param>
    public void Configure(IComponentRegistryBuilder componentRegistry)
    {
        if (componentRegistry == null)
        {
            throw new ArgumentNullException(nameof(componentRegistry));
        }

        var moduleBuilder = new ContainerBuilder(componentRegistry.Properties);

        Load(moduleBuilder);
        moduleBuilder.UpdateRegistry(componentRegistry);
        AttachToRegistrations(componentRegistry);
        AttachToSources(componentRegistry);
    }

    /// <summary>
    /// Override to add registrations to the container.
    /// </summary>
    /// <remarks>
    /// Note that the ContainerBuilder parameter is unique to this module.
    /// </remarks>
    /// <param name="builder">The builder through which components can be
    /// registered.</param>
    protected virtual void Load(ContainerBuilder builder)
    {
    }

    /// <summary>
    /// Override to attach module-specific functionality to a
    /// component registration.
    /// </summary>
    /// <remarks>This method will be called for all existing <i>and future</i> component
    /// registrations - ordering is not important.</remarks>
    /// <param name="componentRegistry">The component registry.</param>
    /// <param name="registration">The registration to attach functionality to.</param>
    protected virtual void AttachToComponentRegistration(
        IComponentRegistryBuilder componentRegistry,
        IComponentRegistration registration)
    {
    }

    /// <summary>
    /// Override to perform module-specific processing on a registration source.
    /// </summary>
    /// <remarks>This method will be called for all existing <i>and future</i> sources
    /// - ordering is not important.</remarks>
    /// <param name="componentRegistry">The component registry into which the source was added.</param>
    /// <param name="registrationSource">The registration source.</param>
    protected virtual void AttachToRegistrationSource(
        IComponentRegistryBuilder componentRegistry,
        IRegistrationSource registrationSource)
    {
    }

    private void AttachToRegistrations(IComponentRegistryBuilder componentRegistry)
    {
        if (componentRegistry == null)
        {
            throw new ArgumentNullException(nameof(componentRegistry));
        }

        componentRegistry.Registered +=
            (sender, e) => AttachToComponentRegistration(e.ComponentRegistryBuilder, e.ComponentRegistration);
    }

    private void AttachToSources(IComponentRegistryBuilder componentRegistry)
    {
        if (componentRegistry == null)
        {
            throw new ArgumentNullException(nameof(componentRegistry));
        }

        componentRegistry.RegistrationSourceAdded +=
            (sender, e) => AttachToRegistrationSource(e.ComponentRegistry, e.RegistrationSource);
    }

    /// <summary>
    /// Gets the assembly in which the concrete module type is located. To avoid bugs whereby deriving from a module will
    /// change the target assembly, this property can only be used by modules that inherit directly from
    /// <see cref="Module"/>.
    /// </summary>
    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "Prevent breaking change")]
    protected virtual Assembly ThisAssembly
    {
        get
        {
            var thisType = GetType();
            var baseType = thisType.BaseType;
            if (baseType != typeof(Module))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ModuleResources.ThisAssemblyUnavailable, thisType, baseType));
            }

            return thisType.Assembly;
        }
    }
}
