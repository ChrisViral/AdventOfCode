using System.Runtime.CompilerServices;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Types;

/// <summary>
/// Type extensions
/// </summary>
[PublicAPI]
public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        /// Checks if a given type is an implementation of a generic interface
        /// </summary>
        /// <param name="interfaceType">Unbounded generic interface type to check against</param>
        /// <returns><see langword="true"/> if <paramref name="type"/> implements <paramref name="interfaceType"/>, otherwise <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsGenericImplementationOf(Type interfaceType)
        {
            return type.GetInterfaces()
                       .Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType);
        }

        /// <summary>
        /// Checks if a given type is an implementation of a generic interface
        /// </summary>
        /// <param name="interfaceType">Unbounded generic interface type to check against</param>
        /// <param name="parameterType">Type of the first generic parameter of the interface</param>
        /// <returns><see langword="true"/> if <paramref name="type"/> implements <paramref name="interfaceType"/>, otherwise <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsGenericImplementationOf(Type interfaceType, Type parameterType)
        {
            return type.GetInterfaces()
                       .Any(t => t.IsGenericType
                              && t.GetGenericTypeDefinition() == interfaceType
                              && t.GenericTypeArguments[0] == parameterType);
        }

        /// <summary>
        /// Checks if a given type is an implementation of the specified interface type
        /// </summary>
        /// <typeparam name="T">Interface type to check against</typeparam>
        /// <returns><see langword="true"/> if <paramref name="type"/> implements <typeparamref name="T"/>, otherwise <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsImplementationOf<T>() => typeof(T).IsAssignableFrom(type);
    }
}
