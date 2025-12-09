using System.Collections.Generic;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// List object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class ListObjectPool<T> : CollectionObjectPool<List<T>, T>
{
    /// <summary>
    /// List pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <inheritdoc />
        protected override List<T> CreateWithCapacity(int capacity) => new(capacity);

        /// <inheritdoc />
        protected override int GetObjectCapacity(List<T> obj) => obj.Capacity;
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static ListObjectPool<T> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public ListObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public ListObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
