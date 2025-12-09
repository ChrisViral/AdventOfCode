using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Collection object pool base
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
/// <typeparam name="TElement">Pool collection element type</typeparam>
[PublicAPI]
public abstract class CollectionObjectPool<T, TElement> : WrappedObjectPool<T> where T : class, ICollection<TElement>, new()
{
    /// <summary>
    /// Collection pool policy
    /// </summary>
    [PublicAPI]
    public abstract class CollectionPolicy : DefaultPooledObjectPolicy<T>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled <see cref="ICollection{T}"/> instances.
        /// </summary>
        /// <value>Defaults to <c>100</c>.</value>
        public int InitialCapacity { get; init; } = 100;

        /// <summary>
        /// Gets or sets the maximum value for <see cref="ICollection{T}"/> capacity that is allowed to be
        /// retained, CollectionPolicysee cref="Policy.Return"/> is invoked.
        /// </summary>
        /// <value>Defaults to <c>4096</c>.</value>
        public int MaximumRetainedCapacity { get; init; } = 4096;

        /// <inheritdoc />
        public override T Create() => CreateWithCapacity(InitialCapacity);

        /// <summary>
        /// Creates the collection with the specified capacity
        /// </summary>
        /// <param name="capacity">Base capacity to give the object</param>
        /// <returns>The newly created object with the specified capacity</returns>
        protected abstract T CreateWithCapacity(int capacity);

        /// <inheritdoc />
        public override bool Return(T obj)
        {
            if (GetObjectCapacity(obj) > MaximumRetainedCapacity)
            {
                // Too big. Discard this one.
                return false;
            }

            obj.Clear();
            return true;
        }

        /// <summary>
        /// Gets the given object's capacity
        /// </summary>
        /// <param name="obj">Object to get the capacity for</param>
        /// <returns>The capacity of the object</returns>
        protected abstract int GetObjectCapacity(T obj);
    }

    /// <inheritdoc />
    protected CollectionObjectPool(CollectionPolicy policy) : base(policy) { }

    /// <inheritdoc />
    protected CollectionObjectPool(CollectionPolicy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
