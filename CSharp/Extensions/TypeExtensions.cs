using System;
using System.Linq;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Types;

/// <summary>
/// Type extensions
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Checks if a given type is an implementation of the specified interface type
        /// </summary>
        /// <param name="interfaceType">Interface type to check against</param>
        /// <returns><see langword="true"/> if <paramref name="type"/> implements <paramref name="interfaceType"/>, otherwise <see langword="false"/></returns>
        public bool IsImplementationOf(Type interfaceType)
        {
            return type.GetInterfaces()
                       .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
        }

        /// <summary>
        /// Checks if a given type is an implementation of the specified interface type
        /// </summary>
        /// <typeparam name="TInterface">Interface type to check against</typeparam>
        /// <returns><see langword="true"/> if <paramref name="type"/> implements <typeparamref name="TInterface"/>, otherwise <see langword="false"/></returns>
        public bool IsImplementationOf<TInterface>() => IsImplementationOf(type, typeof(TInterface));
    }
}
