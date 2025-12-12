using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Collections.Pooling;

/// <summary>
/// Stack object pool
/// </summary>
/// <typeparam name="T">Pool object type</typeparam>
[PublicAPI]
public sealed class StackObjectPool<T> : WrappedObjectPool<Stack<T>>
{
    /// <summary>
    /// Collection pool policy
    /// </summary>
    [PublicAPI]
    public sealed class Policy : DefaultPooledObjectPolicy<Stack<T>>
    {
        /// <summary>
        /// Gets or sets the initial capacity of pooled <see cref="Stack{T}"/> instances.
        /// </summary>
        /// <value>Defaults to <c>100</c>.</value>
        public int InitialCapacity { get; init; } = 100;

        /// <summary>
        /// Gets or sets the maximum value for <see cref="Stack{T}"/> capacity that is allowed to be
        /// retained, CollectionPolicysee cref="Policy.Return"/> is invoked.
        /// </summary>
        /// <value>Defaults to <c>4096</c>.</value>
        public int MaximumRetainedCapacity { get; init; } = 4096;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Stack<T> Create() => new(this.InitialCapacity);

        /// <inheritdoc />
        public override bool Return(Stack<T> obj)
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
    public static StackObjectPool<T> Shared { get; } = new(new Policy());

    /// <inheritdoc />
    public StackObjectPool(Policy policy) : base(policy) { }

    /// <inheritdoc />
    public StackObjectPool(Policy policy, int maximumRetained) : base(policy, maximumRetained) { }
}
