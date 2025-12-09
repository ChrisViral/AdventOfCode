using System.Collections.Generic;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// HashSet object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class HashSetObjectPool<T> : CollectionObjectPool<HashSet<T>, T>
{
    /// <summary>
    /// HashSet pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <summary>
        /// Comparer used for the dictionary
        /// </summary>
        public IEqualityComparer<T> Comparer { get; init; } = EqualityComparer<T>.Default;

        /// <inheritdoc />
        protected override HashSet<T> CreateWithCapacity(int capacity) => new(capacity, this.Comparer);

        /// <inheritdoc />
        protected override int GetObjectCapacity(HashSet<T> obj) => obj.Capacity;
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static HashSetObjectPool<T> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public HashSetObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public HashSetObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
