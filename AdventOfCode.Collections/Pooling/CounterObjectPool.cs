using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Collections.Pooling;

/// <summary>
/// Counter object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class CounterObjectPool<T> : CollectionObjectPool<Counter<T>, T> where T : notnull
{
    /// <summary>
    /// List pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Counter<T> CreateWithCapacity(int capacity) => new(capacity);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override int GetObjectCapacity(Counter<T> obj) => obj.Capacity;
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static CounterObjectPool<T> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public CounterObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public CounterObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
