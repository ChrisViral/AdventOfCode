using System.Runtime.InteropServices;

namespace AdventOfCode.Utils;

/// <summary>
/// Primitive type utils
/// </summary>
/// <typeparam name="T">Primitive type</typeparam>
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
    static PrimitiveUtils()
    {
        BufferSize = typeof(T) switch
        {
            var t when t == typeof(bool) => 1,
            var t when t == typeof(char) => 2,
            var t when t.IsPrimitive     => Marshal.SizeOf<T>(),
            _                            => 0
        };
    }
}
