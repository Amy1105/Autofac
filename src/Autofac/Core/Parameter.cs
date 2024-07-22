// Copyright (c) Autofac Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;

namespace Autofac.Core;

/// <summary>
/// 用于为容器创建的实例上的构造函数参数或属性提供值。
/// Used in order to provide a value to a constructor parameter or property on an instance
/// being created by the container.
/// </summary>
/// <remarks>
/// Not all parameters can be applied to all sites.
/// </remarks>
public abstract class Parameter
{
    /// <summary>
    /// 如果参数能够为特定站点提供值，则返回true。
    /// Returns true if the parameter is able to provide a value to a particular site.
    /// </summary>
    /// <param name="pi">Constructor, method, or property-mutator parameter.</param>
    /// <param name="context">The component context in which the value is being provided.</param>
    /// <param name="valueProvider">If the result is true, the valueProvider parameter will
    /// be set to a function that will lazily retrieve the parameter value. If the result is false,
    /// will be set to null.</param>
    /// <returns>True if a value can be supplied; otherwise, false.</returns>
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
    public abstract bool CanSupplyValue(ParameterInfo pi, IComponentContext context, [NotNullWhen(returnValue: true)] out Func<object?>? valueProvider);
}
