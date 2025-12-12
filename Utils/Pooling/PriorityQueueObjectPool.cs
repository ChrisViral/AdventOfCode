using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// PriorityQueue object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class PriorityQueueObjectPool<T> : CollectionObjectPool<PriorityQueue<T>, T>
    where T : notnull
{
    /// <summary>
    /// PriorityQueue pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <summary>
        /// Comparer used for the dictionary
        /// </summary>
        public IComparer<T> Comparer { get; init; } = Comparer<T>.Default;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override PriorityQueue<T> CreateWithCapacity(int capacity) => new(capacity, this.Comparer);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override int GetObjectCapacity(PriorityQueue<T> obj) => obj.Capacity;
    }

    /// <summary>
    /// Pool cache for comparer specific pools
    /// </summary>
    private static readonly ConcurrentDictionary<IComparer<T>, PriorityQueueObjectPool<T>> ComparerPoolCache = new();

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static PriorityQueueObjectPool<T> Shared { get; } = new(new Policy());

    /// <summary>
    /// Gets the pool for a given comparer
    /// </summary>
    /// <param name="comparer">Comparer to get the pool for</param>
    /// <returns>A pool with the given comparer as it's factory object</returns>
    public static PriorityQueueObjectPool<T> PoolForComparer(IComparer<T> comparer)
    {
        if (!ComparerPoolCache.TryGetValue(comparer, out PriorityQueueObjectPool<T>? pool))
        {
            pool = new PriorityQueueObjectPool<T>(new Policy { Comparer = comparer });
            ComparerPoolCache.TryAdd(comparer, pool);
        }
        return pool;
    }

    /// <inheritdoc />
    public PriorityQueueObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public PriorityQueueObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
