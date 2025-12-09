using System.Collections.Generic;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Dictionary object pool
/// </summary>
/// <typeparam name="TKey">Pool object key type</typeparam>
/// <typeparam name="TValue">Pool object value type</typeparam>
[PublicAPI]
public sealed class DictionaryObjectPool<TKey, TValue> : CollectionObjectPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>> where TKey : notnull
{
    /// <summary>
    /// Dictionary pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : CollectionPolicy
    {
        /// <summary>
        /// Comparer used for the dictionary
        /// </summary>
        public IEqualityComparer<TKey> Comparer { get; init; } = EqualityComparer<TKey>.Default;

        /// <inheritdoc />
        protected override Dictionary<TKey, TValue> CreateWithCapacity(int capacity) => new(capacity, this.Comparer);

        /// <inheritdoc />
        protected override int GetObjectCapacity(Dictionary<TKey, TValue> obj) => obj.Capacity;
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static DictionaryObjectPool<TKey, TValue> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public DictionaryObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public DictionaryObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
