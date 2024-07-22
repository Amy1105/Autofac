// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Autofac.Core.Registration;

namespace Autofac.Core;

/// <summary>
/// 表示打包在一起的一组组件和相关功能。
/// Represents a set of components and related functionality packaged together.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Apply the module to the component registry.
    /// </summary>
    /// <param name="componentRegistry">Component registry to apply configuration to.</param>
    void Configure(IComponentRegistryBuilder componentRegistry);
}
