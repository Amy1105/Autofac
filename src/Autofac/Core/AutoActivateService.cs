﻿// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Autofac.Core;

/// <summary>
/// 服务用作“标志”，表示特定组件应在容器构建时自动激活。
/// Service used as a "flag" to indicate a particular component should be
/// automatically activated on container build.
/// </summary>
internal class AutoActivateService : Service
{
    /// <summary>
    /// Gets the service description.
    /// </summary>
    /// <value>
    /// Always returns <c>AutoActivate</c>.
    /// </value>
    public override string Description => "AutoActivate";

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
    /// <returns>
    /// <see langword="true" /> if the specified <see cref="object"/> is not <see langword="null" />
    /// and is an <see cref="AutoActivateService"/>; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// <para>
    /// All services of this type are considered "equal".
    /// </para>
    /// </remarks>
    public override bool Equals(object? obj)
    {
        var that = obj as AutoActivateService;
        return that is not null;
    }

    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="object"/>. Always <c>0</c> for this type.
    /// </returns>
    /// <remarks>
    /// <para>
    /// All services of this type are considered "equal" and use the same hash code.
    /// </para>
    /// </remarks>
    public override int GetHashCode()
    {
        return 0;
    }
}
