using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Stack object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class QueueObjectPool<T> : WrappedObjectPool<Queue<T>>
{
    /// <summary>
    /// Collection pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : DefaultPooledObjectPolicy<Queue<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled <see cref="Queue{T}"/> instances.
        /// </summary>
        /// <value>Defaults to <c>100</c>.</value>
        public int InitialCapacity { get; init; } = 100;

        /// <summary>
        /// Gets or sets the maximum value for <see cref="Queue{T}"/> capacity that is allowed to be
        /// retained, CollectionPolicysee cref="Policy.Return"/> is invoked.
        /// </summary>
        /// <value>Defaults to <c>4096</c>.</value>
        public int MaximumRetainedCapacity { get; init; } = 4096;

        /// <inheritdoc />
        public override Queue<T> Create() => new(this.InitialCapacity);

        /// <inheritdoc />
        public override bool Return(Queue<T> obj)
        {
            if (obj.Capacity > this.MaximumRetainedCapacity)
            {
                // Too big. Discard this one.
                return false;
            }

            obj.Clear();
            return true;
        }
    }

    /// <summary>
    /// Shared pool instance
    /// </summary>
    public static QueueObjectPool<T> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public QueueObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public QueueObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
