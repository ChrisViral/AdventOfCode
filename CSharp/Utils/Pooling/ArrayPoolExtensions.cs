using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Pooled array wrapper
/// </summary>
/// <param name="array">Array to wrap</param>
/// <param name="pool">Pool the array came from</param>
/// <typeparam name="T">Array element type</typeparam>
[PublicAPI]
public readonly ref struct PooledArray<T>(T[] array, ArrayPool<T> pool) : IDisposable
{
    /// <summary>
    /// Pool this array came from
    /// </summary>
    private readonly ArrayPool<T>? pool = pool;

    /// <summary>
    /// Pooled array reference
    /// </summary>
    public T[] Ref
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = array;

    /// <summary>
    /// Returns the array to the pool
    /// </summary>
    public void Dispose() => this.pool?.Return(this.Ref);

    /// <summary>
    /// Gets the pooled array
    /// </summary>
    /// <param name="pooled">Pooled array to get the refrence from</param>
    /// <returns>The unwrapped array reference</returns>
    public static explicit operator T[](in PooledArray<T> pooled) => pooled.Ref;
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
        public PooledArray<T> RentTracked(int minimumLength) => new(pool.Rent(minimumLength), pool);
    }
}
