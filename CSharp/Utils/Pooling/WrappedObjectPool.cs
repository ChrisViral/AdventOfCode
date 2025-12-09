using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Object pool which wraps the references it returns to facilitate returns
/// </summary>
/// <typeparam name="T">Pooled object type</typeparam>
public abstract class WrappedObjectPool<T> : DefaultObjectPool<T> where T : class
{
    /// <inheritdoc />
    protected WrappedObjectPool(IPooledObjectPolicy<T> policy) : base(policy) { }

    /// <inheritdoc />
    protected WrappedObjectPool(IPooledObjectPolicy<T> policy, int maximumRetained) : base(policy, maximumRetained) { }

    /// <summary>
    /// Gets a warpped reference to a pooled object
    /// </summary>
    /// <returns>Wrapped pooled object reference</returns>
    public new virtual Pooled<T> Get() => new(base.Get(), this);
}
