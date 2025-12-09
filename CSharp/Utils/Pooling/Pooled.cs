using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.ObjectPool;

namespace AdventOfCode.Utils.Pooling;

/// <summary>
/// Pooled object wrapper
/// </summary>
/// <param name="obj">Object to wrap</param>
/// <param name="pool">Pool the object came from</param>
/// <typeparam name="T">Object type</typeparam>
[PublicAPI]
public readonly ref struct Pooled<T>(T obj, ObjectPool<T> pool) : IDisposable where T : class
{
    /// <summary>
    /// Pool this object came from
    /// </summary>
    private readonly ObjectPool<T>? pool = pool;

    /// <summary>
    /// Pooled object reference
    /// </summary>
    public T Ref
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    } = obj;

    /// <summary>
    /// Returns the object to the pool
    /// </summary>
    public void Dispose() => this.pool?.Return(this.Ref);

    /// <summary>
    /// Gets the pooled object
    /// </summary>
    /// <param name="pooled">Pooled object to get the refrence from</param>
    /// <returns>The unwrapped object reference</returns>
    public static explicit operator T(in Pooled<T> pooled) => pooled.Ref;
}
