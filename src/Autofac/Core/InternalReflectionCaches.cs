﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;
using Autofac.Core.Activators.Reflection;
using Autofac.Util;
using Autofac.Util.Cache;

namespace Autofac.Core;

/// <summary>
/// 定义了在相对热的路径中使用的仅限内部的已知缓存，
/// 因此我们希望避免在<see cref="ReflectionCoacheSet.GetOrCreateCache(string)"/>中进行额外的字典查找。
/// Defines known, internal-only caches that are used in relatively hot paths, so we want to
/// avoid the additional dictionary lookup in <see cref="ReflectionCacheSet.GetOrCreateCache(string)"/>.
/// </summary>
internal class InternalReflectionCaches
{
    /// <summary>
    /// Gets the cache used by <see cref="Features.Scanning.AssemblyExtensions.GetPermittedTypesForAssemblyScanning"/>.
    /// </summary>
    public ReflectionCacheAssemblyDictionary<Assembly, IEnumerable<Type>> AssemblyScanAllowedTypes { get; }

    /// <summary>
    /// Gets the cache used by <see cref="InternalTypeExtensions.IsGenericEnumerableInterfaceType"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, bool> IsGenericEnumerableInterface { get; }

    /// <summary>
    /// Gets the cache used by <see cref="InternalTypeExtensions.IsGenericListOrCollectionInterfaceType"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, bool> IsGenericListOrCollectionInterfaceType { get; }

    /// <summary>
    /// Gets the cache used by <see cref="InternalTypeExtensions.IsGenericTypeDefinedBy"/>.
    /// </summary>
    public ReflectionCacheTupleDictionary<Type, bool> IsGenericTypeDefinedBy { get; }

    /// <summary>
    /// Gets the cache used by <see cref="ConstructorBinder"/>.
    /// </summary>
    public ReflectionCacheDictionary<ConstructorInfo, Func<object?[], object>> ConstructorBinderFactory { get; }

    /// <summary>
    /// Gets a cache used by <see cref="AutowiringPropertyInjector.InjectProperties"/>.
    /// </summary>
    public ReflectionCacheDictionary<PropertyInfo, Action<object, object?>> AutowiringPropertySetters { get; }

    /// <summary>
    /// Gets a cache used by <see cref="AutowiringPropertyInjector.InjectProperties"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, IReadOnlyList<PropertyInfo>> AutowiringInjectableProperties { get; }

    /// <summary>
    /// Gets a cache used by <see cref="DefaultConstructorFinder"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, ConstructorInfo[]> DefaultPublicConstructors { get; }

    /// <summary>
    /// Gets a cache of memoized <see cref="Type.GetGenericTypeDefinition"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, Type> GenericTypeDefinitionByType { get; }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets a cache used by <see cref="ReflectionActivator"/>.
    /// </summary>
    public ReflectionCacheDictionary<Type, bool> HasRequiredMemberAttribute { get; }
#endif

    /// <summary>
    /// Initializes a new instance of the <see cref="InternalReflectionCaches"/> class.
    /// </summary>
    /// <param name="set">The cache set used to retrieve the required caches.</param>
    public InternalReflectionCaches(ReflectionCacheSet set)
    {
        AssemblyScanAllowedTypes = set.GetOrCreateCache(nameof(AssemblyScanAllowedTypes), _ => new ReflectionCacheAssemblyDictionary<Assembly, IEnumerable<Type>>
        {
            Usage = ReflectionCacheUsage.Registration,
        });

        IsGenericEnumerableInterface = set.GetOrCreateCache<ReflectionCacheDictionary<Type, bool>>(nameof(IsGenericEnumerableInterface));
        IsGenericListOrCollectionInterfaceType = set.GetOrCreateCache<ReflectionCacheDictionary<Type, bool>>(nameof(IsGenericListOrCollectionInterfaceType));
        IsGenericTypeDefinedBy = set.GetOrCreateCache<ReflectionCacheTupleDictionary<Type, bool>>(nameof(IsGenericTypeDefinedBy));
        ConstructorBinderFactory = set.GetOrCreateCache<ReflectionCacheDictionary<ConstructorInfo, Func<object?[], object>>>(nameof(ConstructorBinderFactory));
        AutowiringPropertySetters = set.GetOrCreateCache<ReflectionCacheDictionary<PropertyInfo, Action<object, object?>>>(nameof(AutowiringPropertySetters));
        AutowiringInjectableProperties = set.GetOrCreateCache<ReflectionCacheDictionary<Type, IReadOnlyList<PropertyInfo>>>(nameof(AutowiringInjectableProperties));
        DefaultPublicConstructors = set.GetOrCreateCache<ReflectionCacheDictionary<Type, ConstructorInfo[]>>(nameof(DefaultPublicConstructors));
        GenericTypeDefinitionByType = set.GetOrCreateCache<ReflectionCacheDictionary<Type, Type>>(nameof(GenericTypeDefinitionByType));
#if NET7_0_OR_GREATER
        HasRequiredMemberAttribute = set.GetOrCreateCache<ReflectionCacheDictionary<Type, bool>>(nameof(HasRequiredMemberAttribute));
#endif
    }
}
