﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Core;

/// <summary>
/// 由携带类型信息的服务支持的接口。
/// Interface supported by services that carry type information.
/// </summary>
public interface IServiceWithType
{
    /// <summary>
    /// Gets the type of the service.
    /// </summary>
    /// <value>The type of the service.</value>
    Type ServiceType { get; }

    /// <summary>
    /// Return a new service of the same kind, but carrying
    /// <paramref name="newType"/> as the <see cref="ServiceType"/>.
    /// </summary>
    /// <param name="newType">The new service type.</param>
    /// <returns>A new service with the service type.</returns>
    Service ChangeType(Type newType);
}
