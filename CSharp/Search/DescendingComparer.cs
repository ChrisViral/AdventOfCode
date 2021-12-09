using System.Collections.Generic;

namespace AdventOfCode.Search;

/// <summary>
/// A descending order comparer, using the default comparer of <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Type to compare</typeparam>
public class DescendingComparer<T> : IComparer<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static DescendingComparer<T> Comparer { get; } = new();

    /// <inheritdoc cref="IComparer{T}"/>
    public int Compare(T? x, T? y) => -Comparer<T>.Default.Compare(x, y);
}
