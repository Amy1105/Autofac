﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Globalization;
using System.Runtime.CompilerServices;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;

namespace Autofac;

/// <summary>
/// Adds syntactic convenience methods to the <see cref="IComponentContext"/> interface.
/// </summary>
public static class ResolutionExtensions
{   
    /// <summary>
    /// The <see cref="NamedParameter"/> name, provided when properties are injected onto an existing instance.
    /// </summary>
    public const string PropertyInjectedInstanceTypeNamedParameter = AutowiringPropertyInjector.InstanceTypeNamedParameter;

    #region InjectProperties
    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be
    /// resolved in the context.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.OverwriteSetValueInstance, ResolveRequest.NoParameters);
        return instance;
    }

    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be
    /// resolved in the context.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.OverwriteSetValueInstance, parameters);
        return instance;
    }

    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be
    /// resolved in the context.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance, params Parameter[] parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.OverwriteSetValueInstance, parameters);
        return instance;
    }

    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be resolved by service and that satisfy the
    /// constraints imposed by <paramref name="propertySelector"/>.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="propertySelector">Selector to determine with properties should be injected.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance, IPropertySelector propertySelector)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, propertySelector, ResolveRequest.NoParameters);
        return instance;
    }

    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be resolved by service and that satisfy the
    /// constraints imposed by <paramref name="propertySelector"/>.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="propertySelector">Selector to determine with properties should be injected.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance, IPropertySelector propertySelector, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, propertySelector, parameters);
        return instance;
    }

    /// <summary>
    /// Set any properties on <paramref name="instance"/> that can be resolved by service and that satisfy the
    /// constraints imposed by <paramref name="propertySelector"/>.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="propertySelector">Selector to determine with properties should be injected.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectProperties<TService>(this IComponentContext context, TService instance, IPropertySelector propertySelector, params Parameter[] parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, propertySelector, parameters);
        return instance;
    }

    /// <summary>
    /// Set any null-valued properties on <paramref name="instance"/> that can be
    /// resolved by the container.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectUnsetProperties<TService>(this IComponentContext context, TService instance)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.PreserveSetValueInstance, ResolveRequest.NoParameters);
        return instance;
    }

    /// <summary>
    /// Set any null-valued properties on <paramref name="instance"/> that can be
    /// resolved by the container.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectUnsetProperties<TService>(this IComponentContext context, TService instance, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.PreserveSetValueInstance, parameters);
        return instance;
    }

    /// <summary>
    /// Set any null-valued properties on <paramref name="instance"/> that can be
    /// resolved by the container.
    /// </summary>
    /// <typeparam name="TService">Type of instance. Used only to provide method chaining.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The instance to inject properties into.</param>
    /// <param name="parameters">Optional parameters to use during the property injection.</param>
    /// <returns><paramref name="instance"/>.</returns>
    public static TService InjectUnsetProperties<TService>(this IComponentContext context, TService instance, params Parameter[] parameters)
        where TService : notnull
    {
        AutowiringPropertyInjector.InjectProperties(context, instance, DefaultPropertySelector.PreserveSetValueInstance, parameters);
        return instance;
    }

    #endregion

    #region IsRegistered
    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <typeparam name="TService">The service to test for the registration of.</typeparam>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegistered<TService>(this IComponentContext context)
        where TService : notnull
    {
        return IsRegistered(context, typeof(TService));
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceType">The service to test for the registration of.</param>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegistered(this IComponentContext context, Type serviceType)
    {
        return IsRegisteredService(context, new TypedService(serviceType));
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service to test for the registration of.</param>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegisteredService(this IComponentContext context, Service service)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        return context.ComponentRegistry.IsRegistered(service);
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">The key of the service to test for the registration of.</param>
    /// <typeparam name="TService">Type type of the service to test for the registration of.</typeparam>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegisteredWithKey<TService>(this IComponentContext context, object serviceKey)
        where TService : notnull
    {
        return IsRegisteredWithKey(context, serviceKey, typeof(TService));
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">The key of the service to test for the registration of.</param>
    /// <param name="serviceType">Type type of the service to test for the registration of.</param>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegisteredWithKey(this IComponentContext context, object serviceKey, Type serviceType)
    {
        return IsRegisteredService(context, new KeyedService(serviceKey, serviceType));
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The name of the service to test for the registration of.</param>
    /// <typeparam name="TService">Type type of the service to test for the registration of.</typeparam>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegisteredWithName<TService>(this IComponentContext context, string serviceName)
        where TService : notnull
    {
        return IsRegisteredWithKey<TService>(context, serviceName);
    }

    /// <summary>
    /// Determine whether the specified service is available in the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The name of the service to test for the registration of.</param>
    /// <param name="serviceType">Type type of the service to test for the registration of.</param>
    /// <returns>True if the service is registered.</returns>
    public static bool IsRegisteredWithName(this IComponentContext context, string serviceName, Type serviceType)
    {
        return IsRegisteredWithKey(context, serviceName, serviceType);
    }

    #endregion

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The service to retrieve.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <returns>The component instance that provides the service.</returns>
    /// <exception cref="ComponentNotRegisteredException" />
    /// <exception cref="DependencyResolutionException" />
    public static TService Resolve<TService>(this IComponentContext context)  //可以访问服务或解决组件依赖关系的上下文。上下文的处置将处置所有拥有的组件
        where TService : notnull
    {
        return Resolve<TService>(context, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService Resolve<TService>(this IComponentContext context, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        return CastInstance<TService>(Resolve(context, typeof(TService), parameters));
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService Resolve<TService>(this IComponentContext context, params Parameter[] parameters)
        where TService : notnull
    {
        return context.Resolve<TService>((IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object Resolve(this IComponentContext context, Type serviceType)
    {
        return Resolve(context, serviceType, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object Resolve(this IComponentContext context, Type serviceType, IEnumerable<Parameter> parameters)
    {
        return ResolveService(context, new TypedService(serviceType), parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object Resolve(this IComponentContext context, Type serviceType, params Parameter[] parameters)
    {
        return context.Resolve(serviceType, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveKeyed<TService>(this IComponentContext context, object serviceKey)
        where TService : notnull
    {
        return ResolveKeyed<TService>(context, serviceKey, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveKeyed<TService>(this IComponentContext context, object serviceKey, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        return CastInstance<TService>(ResolveService(context, new KeyedService(serviceKey, typeof(TService)), parameters));
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveKeyed<TService>(this IComponentContext context, object serviceKey, params Parameter[] parameters)
        where TService : notnull
    {
        return context.ResolveKeyed<TService>(serviceKey, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveKeyed(this IComponentContext context, object serviceKey, Type serviceType)
    {
        return ResolveKeyed(context, serviceKey, serviceType, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveKeyed(this IComponentContext context, object serviceKey, Type serviceType, IEnumerable<Parameter> parameters)
    {
        return ResolveService(context, new KeyedService(serviceKey, serviceType), parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">Key of the service.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveKeyed(this IComponentContext context, object serviceKey, Type serviceType, params Parameter[] parameters)
    {
        return context.ResolveKeyed(serviceKey, serviceType, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName)
        where TService : notnull
    {
        return ResolveNamed<TService>(context, serviceName, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName, IEnumerable<Parameter> parameters)
        where TService : notnull
    {
        return CastInstance<TService>(ResolveService(context, new KeyedService(serviceName, typeof(TService)), parameters));
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <typeparam name="TService">The type to which the result will be cast.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static TService ResolveNamed<TService>(this IComponentContext context, string serviceName, params Parameter[] parameters)
        where TService : notnull
    {
        return context.ResolveNamed<TService>(serviceName, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The service name.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveNamed(this IComponentContext context, string serviceName, Type serviceType)
    {
        return ResolveNamed(context, serviceName, serviceType, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceName">The service name.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveNamed(this IComponentContext context, string serviceName, Type serviceType, IEnumerable<Parameter> parameters)
    {
        return ResolveService(context, new KeyedService(serviceName, serviceType), parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceName">The service name.</param>
    /// <param name="serviceType">Type of the service.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveNamed(this IComponentContext context, string serviceName, Type serviceType, params Parameter[] parameters)
    {
        return context.ResolveNamed(serviceName, serviceType, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptional<TService>(this IComponentContext context)
        where TService : class
    {
        return ResolveOptional<TService>(context, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptional<TService>(this IComponentContext context, IEnumerable<Parameter> parameters)
        where TService : class
    {
        return CastInstance<TService?>(ResolveOptionalService(context, new TypedService(typeof(TService)), parameters));
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptional<TService>(this IComponentContext context, params Parameter[] parameters)
        where TService : class
    {
        return context.ResolveOptional<TService>((IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptional(this IComponentContext context, Type serviceType)
    {
        return ResolveOptional(context, serviceType, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptional(this IComponentContext context, Type serviceType, IEnumerable<Parameter> parameters)
    {
        return ResolveOptionalService(context, new TypedService(serviceType), parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceType">The type of the service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptional(this IComponentContext context, Type serviceType, params Parameter[] parameters)
    {
        return context.ResolveOptional(serviceType, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// 从上下文中检索服务。
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">The name of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalKeyed<TService>(this IComponentContext context, object serviceKey)
        where TService : class
    {
        return ResolveOptionalKeyed<TService>(context, serviceKey, ResolveRequest.NoParameters);
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceKey">The name of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalKeyed<TService>(this IComponentContext context, object serviceKey, IEnumerable<Parameter> parameters)
        where TService : class
    {
        return CastInstance<TService?>(ResolveOptionalService(context, new KeyedService(serviceKey, typeof(TService)), parameters));
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceKey">The key of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalKeyed<TService>(this IComponentContext context, object serviceKey, params Parameter[] parameters)
        where TService : class
    {
        return context.ResolveOptionalKeyed<TService>(serviceKey, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalNamed<TService>(this IComponentContext context, string serviceName)
        where TService : class
    {
        return ResolveOptionalKeyed<TService>(context, serviceName);
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalNamed<TService>(this IComponentContext context, string serviceName, IEnumerable<Parameter> parameters)
        where TService : class
    {
        return ResolveOptionalKeyed<TService>(context, serviceName, parameters);
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <typeparam name="TService">The service to resolve.</typeparam>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static TService? ResolveOptionalNamed<TService>(this IComponentContext context, string serviceName, params Parameter[] parameters)
        where TService : class
    {
        return context.ResolveOptionalKeyed<TService>(serviceName, parameters);
    }

    /// <summary>
    ///  从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptionalService(this IComponentContext context, Service service)
    {
        return ResolveOptionalService(context, service, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// 从上下文中检索服务，如果服务没有注册，则为null
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptionalService(this IComponentContext context, Service service, IEnumerable<Parameter> parameters)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        context.TryResolveService(service, parameters, out object? instance);
        return instance;
    }

    /// <summary>
    /// Retrieve a service from the context, or null if the service is not
    /// registered.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <returns>
    /// The component instance that provides the service, or null.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static object? ResolveOptionalService(this IComponentContext context, Service service, params Parameter[] parameters)
    {
        return context.ResolveOptionalService(service, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service to resolve.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveService(this IComponentContext context, Service service)
    {
        return ResolveService(context, service, ResolveRequest.NoParameters);
    }

    /// <summary>
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="service">The service to resolve.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveService(this IComponentContext context, Service service, IEnumerable<Parameter> parameters)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        if (!context.TryResolveService(service, parameters, out var instance))
        {
            throw new ComponentNotRegisteredException(service);
        }

        return instance;
    }

    /// <summary>
    /// Retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="parameters">Parameters for the service.</param>
    /// <param name="service">The service to resolve.</param>
    /// <returns>
    /// The component instance that provides the service.
    /// </returns>
    /// <exception cref="ComponentNotRegisteredException"/>
    /// <exception cref="DependencyResolutionException"/>
    public static object ResolveService(this IComponentContext context, Service service, params Parameter[] parameters)
    {
        return context.ResolveService(service, (IEnumerable<Parameter>)parameters);
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="instance">The resulting component instance providing the service, or default(T).</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static bool TryResolve<T>(this IComponentContext context, [NotNullWhen(returnValue: true)] out T? instance)
        where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // Null annotation attributes only work if placed directly in an if statement.
        if (context.TryResolve(typeof(T), out object? component))
        {
            instance = CastInstance<T>(component);

            return true;
        }
        else
        {
            instance = default;

            return false;
        }
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceType">The service type to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public static bool TryResolve(this IComponentContext context, Type serviceType, [NotNullWhen(returnValue: true)] out object? instance)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.TryResolveService(new TypedService(serviceType), ResolveRequest.NoParameters, out instance);
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">The key of the service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static bool TryResolveKeyed<T>(this IComponentContext context, object serviceKey, [NotNullWhen(returnValue: true)] out T? instance)
        where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.TryResolveKeyed(serviceKey, typeof(T), out object? component))
        {
            instance = CastInstance<T>(component);

            return true;
        }
        else
        {
            instance = default;

            return false;
        }
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceKey">The key of the service to resolve.</param>
    /// <param name="serviceType">The type of the service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public static bool TryResolveKeyed(this IComponentContext context, object serviceKey, Type serviceType, [NotNullWhen(returnValue: true)] out object? instance)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.TryResolveService(new KeyedService(serviceKey, serviceType), ResolveRequest.NoParameters, out instance);
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <typeparam name="T">The service type to resolve.</typeparam>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The name of the service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    public static bool TryResolveNamed<T>(this IComponentContext context, string serviceName, [NotNullWhen(returnValue: true)] out T? instance)
        where T : class
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.TryResolveNamed(serviceName, typeof(T), out object? component))
        {
            instance = CastInstance<T>(component);

            return true;
        }
        else
        {
            instance = default;

            return false;
        }
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="serviceName">The name of the service to resolve.</param>
    /// <param name="serviceType">The type of the service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public static bool TryResolveNamed(this IComponentContext context, string serviceName, Type serviceType, [NotNullWhen(returnValue: true)] out object? instance)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.TryResolveService(new KeyedService(serviceName, serviceType), ResolveRequest.NoParameters, out instance);
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public static bool TryResolveService(this IComponentContext context, Service service, [NotNullWhen(returnValue: true)] out object? instance)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return context.TryResolveService(service, ResolveRequest.NoParameters, out instance);
    }

    /// <summary>
    /// Try to retrieve a service from the context.
    /// </summary>
    /// <param name="context">The context from which to resolve the service.</param>
    /// <param name="service">The service to resolve.</param>
    /// <param name="instance">The resulting component instance providing the service, or null.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>
    /// True if a component providing the service is available.
    /// </returns>
    /// <exception cref="DependencyResolutionException"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context" /> is <see langword="null" />.
    /// </exception>
    [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate")]
    public static bool TryResolveService(this IComponentContext context, Service service, IEnumerable<Parameter> parameters, [NotNullWhen(returnValue: true)] out object? instance)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

                                        //获取已初始化的服务信息
        if (!context.ComponentRegistry.TryGetServiceRegistration(service, out var serviceRegistration))
        {
            instance = null;
            return false;
        }

        instance = context.ResolveComponent(new ResolveRequest(service, serviceRegistration, parameters));
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TService CastInstance<TService>(object? instance)
    {
        try
        {
            // Allow a cast from null object to null TService.
            return (TService)instance!;
        }
        catch (InvalidCastException castException)
        {
            throw new DependencyResolutionException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    ResolutionExtensionsResources.CouldNotCastInstance,
                    instance!.GetType(),
                    typeof(TService)),
                castException);
        }
    }
}
