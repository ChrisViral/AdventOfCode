using System;
using System.Linq;
using JetBrains.Annotations;

namespace AdventOfCode.Extensions;

/// <summary>
/// Type extensions
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    /// <summary>
    /// Checks if a given type is an implementation of the specified interface type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <param name="interfaceType">Interface type to check against</param>
    /// <returns><see langword="true"/> if <paramref name="type"/> implements <paramref name="interfaceType"/>, otherwise <see langword="false"/></returns>
    public static bool IsImplementationOf(this Type type, Type interfaceType)
    {
        return type.GetInterfaces()
                   .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
    }

    /// <summary>
    /// Checks if a given type is an implementation of the specified interface type
    /// </summary>
    /// <param name="type">Type to check</param>
    /// <typeparam name="TInterface">Interface type to check against</typeparam>
    /// <returns><see langword="true"/> if <paramref name="type"/> implements <typeparamref name="TInterface"/>, otherwise <see langword="false"/></returns>
    public static bool IsImplementationOf<TInterface>(this Type type) => IsImplementationOf(type, typeof(TInterface));
}