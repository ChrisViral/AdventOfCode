using JetBrains.Annotations;

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
    public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.SequenceEqual(y);
    }

    /// <inheritdoc/>
    public int GetHashCode(IEnumerable<T> obj)
    {
        return obj.Aggregate(0, HashCode.Combine);
    }
}
