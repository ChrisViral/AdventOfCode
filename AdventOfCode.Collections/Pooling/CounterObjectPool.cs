using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Collections.Pooling;

/// <summary>
/// Counter object pool
/// </summary>
/// <typeparam name="TKey">Counter key type</typeparam>
/// <typeparam name="TCount">Counter count type</typeparam>
[PublicAPI]
public sealed class CounterObjectPool<TKey, TCount> : CollectionObjectPool<Counter<TKey, TCount>, TKey>
    where TKey : notnull
    where TCount : struct, IBinaryInteger<TCount>
{
    /// <summary>
    /// List pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Counter<TKey, TCount> CreateWithCapacity(int capacity) => new(capacity);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override int GetObjectCapacity(Counter<TKey, TCount> obj) => obj.Capacity;
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static CounterObjectPool<TKey, TCount> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public CounterObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public CounterObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
