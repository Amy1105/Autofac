// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Core;

namespace Autofac.Features.LazyDependencies;

/// <summary>
/// Support the <see cref="Lazy{T}"/>
/// type automatically whenever type T is registered with the container.
/// When a dependency of a lazy type is used, the instantiation of the underlying
/// component will be delayed until the Value property is first accessed.
/// 每当类型T在容器中注册时，就会自动键入。当使用懒惰类型的依赖关系时，底层组件的实例化将被延迟，直到首次访问Value属性。
/// </summary>
internal class LazyRegistrationSource : ImplicitRegistrationSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyRegistrationSource"/> class.
    /// </summary>
    public LazyRegistrationSource()
        : base(typeof(Lazy<>))
    {
    }

    /// <inheritdoc/>
    public override string Description => LazyRegistrationSourceResources.LazyRegistrationSourceDescription;

    /// <inheritdoc/>
    protected override object ResolveInstance<T>(IComponentContext context, in ResolveRequest request)
    {
        var capturedContext = context.Resolve<IComponentContext>();
        ResolveRequest requestCopy = request;
        return new Lazy<T>(() => (T)capturedContext.ResolveComponent(requestCopy));
    }
}
