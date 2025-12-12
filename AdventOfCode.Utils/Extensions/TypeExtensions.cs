using System.Runtime.CompilerServices;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsImplementationOf(Type interfaceType)
        {
            return type.GetInterfaces()
                       .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
        }
    }
}
