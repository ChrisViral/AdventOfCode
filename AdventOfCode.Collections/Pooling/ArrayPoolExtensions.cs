using System.Buffers;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Collections.Pooling.Arrays;

/// <summary>
/// Pooled array wrapper
/// </summary>
/// <param name="array">Array to wrap</param>
/// <param name="pool">Pool the array came from</param>
/// <typeparam name="T">Array element type</typeparam>
[PublicAPI]
public readonly ref struct FromArrayPool<T>(T[] array, ArrayPool<T> pool, int requestedLength) : IDisposable
{
    /// <summary>
    /// Pool this array came from
    /// </summary>
    private readonly ArrayPool<T>? pool = pool;

    /// <summary>
    /// Pooled array reference
    /// </summary>
    public T[] Ref { get; } = array;

    /// <summary>
    /// The length of array that was actually requested
    /// </summary>
    public int RequestedLength { get; } = requestedLength;

    /// <summary>
    /// The requested array as a span
    /// </summary>
    public Span<T> AsSpan => this.Ref.AsSpan(this.RequestedLength);

    /// <summary>
    /// The requested array as an ArraySegment
    /// </summary>
    public ArraySegment<T> AsArraySegement => new ArraySegment<T>(this.Ref, 0, this.RequestedLength);

    /// <summary>
    /// Returns the array to the pool
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => this.pool?.Return(this.Ref);

    /// <summary>
    /// Gets the pooled array
    /// </summary>
    /// <param name="fromArrayPool">Pooled array to get the refrence from</param>
    /// <returns>The unwrapped array reference</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator T[](in FromArrayPool<T> fromArrayPool) => fromArrayPool.Ref;
}

/// <summary>
/// ArrayPool extensions
/// </summary>
[PublicAPI]
public static class ArrayPoolExtensions
{
    /// <summary>
    /// ArrayPool extensions
    /// </summary>
    /// <param name="pool">Pool instance</param>
    /// <typeparam name="T">Array elementy type</typeparam>
    extension<T>(ArrayPool<T> pool)
    {
        /// <summary>
        /// Rents an array in a tracked wrapper
        /// </summary>
        /// <param name="minimumLength">Array minimum length</param>
        /// <returns>The wrapped rented array reference</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FromArrayPool<T> RentTracked(int minimumLength) => new(pool.Rent(minimumLength), pool, minimumLength);
    }
}
