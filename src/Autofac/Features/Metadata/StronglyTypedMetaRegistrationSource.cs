﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Util;
using Autofac.Util.Cache;

namespace Autofac.Features.Metadata;

/// <summary>
/// Support the <see cref="Meta{T, TMetadata}"/>
/// types automatically whenever type T is registered with the container.
/// Metadata values come from the component registration's metadata.
/// 支持<see cref="Meta{T，TMetadata}"/>每当类型T在容器中注册时，就会自动类型。元数据值来自组件注册的元数据。
/// </summary>
internal class StronglyTypedMetaRegistrationSource : IRegistrationSource
{
    private const string ReflectionCacheName = $"{nameof(StronglyTypedMetaRegistrationSource)}.Cache";

    private static readonly MethodInfo CreateMetaRegistrationMethod = typeof(StronglyTypedMetaRegistrationSource).GetDeclaredMethod(nameof(CreateMetaRegistration));

    private delegate IComponentRegistration RegistrationCreator(Service providedService, Service valueService, ServiceRegistration valueRegistration);

    /// <inheritdoc/>
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
        if (registrationAccessor == null)
        {
            throw new ArgumentNullException(nameof(registrationAccessor));
        }

        if (service is not IServiceWithType swt ||
            !swt.ServiceType.IsGenericTypeDefinedBy(typeof(Meta<,>)))
        {
            return Enumerable.Empty<IComponentRegistration>();
        }

        var genericArguments = swt.ServiceType.GenericTypeArguments.ToArray();
        var valueType = genericArguments[0];
        var metaType = genericArguments[1];

        if (!metaType.IsClass)
        {
            return Enumerable.Empty<IComponentRegistration>();
        }

        var valueService = swt.ChangeType(valueType);

        var methodCache = ReflectionCacheSet.Shared.GetOrCreateCache<ReflectionCacheTupleDictionary<Type, RegistrationCreator>>(ReflectionCacheName);

        var registrationCreator = methodCache.GetOrAdd((valueType, metaType), t =>
        {
            return CreateMetaRegistrationMethod.MakeGenericMethod(t.Item1, t.Item2).CreateDelegate<RegistrationCreator>(null);
        });

        return registrationAccessor(valueService)
            .Select(v => registrationCreator.Invoke(service, valueService, v));
    }

    /// <inheritdoc/>
    public bool IsAdapterForIndividualComponents => true;

    /// <inheritdoc/>
    public override string ToString()
    {
        return MetaRegistrationSourceResources.StronglyTypedMetaRegistrationSourceDescription;
    }

    private static IComponentRegistration CreateMetaRegistration<T, TMetadata>(Service providedService, Service valueService, ServiceRegistration implementation)
    {
        var metadataProvider = MetadataViewProvider.GetMetadataViewProvider<TMetadata>();

        var rb = RegistrationBuilder
            .ForDelegate((c, p) =>
            {
                var metadata = metadataProvider(implementation.Registration.Target.Metadata);
                return new Meta<T, TMetadata>((T)c.ResolveComponent(new ResolveRequest(valueService, implementation, p)), metadata);
            })
            .As(providedService)
            .Targeting(implementation.Registration);

        return rb.CreateRegistration();
    }
}
