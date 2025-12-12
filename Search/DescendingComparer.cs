using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Search;

/// <summary>
/// A descending order comparer, using the default comparer of <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Type to compare</typeparam>
[PublicAPI]
public sealed class DescendingComparer<T> : IComparer<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static DescendingComparer<T> Comparer { get; } = new();

    /// <inheritdoc cref="IComparer{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Compare(T? x, T? y) => -Comparer<T>.Default.Compare(x, y);
}
