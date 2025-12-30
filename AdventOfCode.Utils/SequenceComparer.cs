using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ZLinq;

namespace AdventOfCode.Utils;

/// <summary>
/// Sequence based equality comparer
/// </summary>
/// <typeparam name="T">Sequence element</typeparam>
[PublicAPI]
public class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static SequenceComparer<T> Instance { get; } = new();

    /// <summary>
    /// Prevents extenal instantiation
    /// </summary>
    private SequenceComparer() { }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.AsValueEnumerable().SequenceEqual(y.AsValueEnumerable());
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(IEnumerable<T> obj)
    {
        return obj.AsValueEnumerable().Aggregate(0, HashCode.Combine);
    }
}

/// <summary>
/// Sequence based equality comparer
/// </summary>
/// <typeparam name="TEnumerator">Enumerator type</typeparam>
/// <typeparam name="T">Sequence element</typeparam>
[PublicAPI]
public class ValueSequenceComparer<TEnumerator, T> : IEqualityComparer<IValueEnumerable<TEnumerator, T>>
    where TEnumerator : struct, IValueEnumerator<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static ValueSequenceComparer<TEnumerator, T> Instance { get; } = new();

    /// <summary>
    /// Prevents extenal instantiation
    /// </summary>
    private ValueSequenceComparer() { }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(IValueEnumerable<TEnumerator, T>? x, IValueEnumerable<TEnumerator, T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.AsValueEnumerable().SequenceEqual(y.AsValueEnumerable());
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode(IValueEnumerable<TEnumerator, T> obj)
    {
        return obj.AsValueEnumerable().Aggregate(0, HashCode.Combine);
    }
}
