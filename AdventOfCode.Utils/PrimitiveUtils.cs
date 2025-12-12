using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Primitive type utils
/// </summary>
/// <typeparam name="T">Primitive type</typeparam>
[PublicAPI]
public static class PrimitiveUtils<T>
{
    /// <summary>
    /// Bytes size for <typeparamref name="T"/>. If <typeparamref name="T"/> is not a primitive, then this is <c>0</c>
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static int BufferSize { get; }

    /// <summary>
    /// Initializes the primitive buffer size
    /// </summary>
    static PrimitiveUtils() => BufferSize = typeof(T) switch
    {
        { } t when t == typeof(bool)  => 1,
        { } t when t == typeof(char)  => 2,
        { IsPrimitive: true }         => Marshal.SizeOf<T>(),
        _                             => 0
    };
}
