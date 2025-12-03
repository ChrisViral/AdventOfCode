using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Search;

/// <summary>
/// Minimum value search comparer
/// </summary>
[PublicAPI]
public sealed class MinSearchComparer<T> : IComparer<ISearchNode<T>> where T : INumber<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static MinSearchComparer<T> Comparer { get; } = new();

    /// <summary>
    /// Private constructor, prevents instantiation
    /// </summary>
    private MinSearchComparer() { }

    /// <inheritdoc cref="IComparer{T}"/>
    public int Compare(ISearchNode<T>? a, ISearchNode<T>? b) => a?.Cost.CompareTo(b is not null ? b.Cost : T.Zero) ?? 0;
}

/// <summary>
/// Maximum value search comparer
/// </summary>
/// ReSharper disable once UnusedType.Global
[PublicAPI]
public sealed class MaxSearchComparer<T> : IComparer<ISearchNode<T>> where T : INumber<T>
{
    /// <summary>
    /// Comparer instance
    /// </summary>
    public static MaxSearchComparer<T> Comparer { get; } = new();

    /// <summary>
    /// Private constructor, prevents instantiation
    /// </summary>
    private MaxSearchComparer() { }

    /// <inheritdoc cref="IComparer{T}"/>
    public int Compare(ISearchNode<T>? a, ISearchNode<T>? b) => b?.Cost.CompareTo(a is not null ? a.Cost : T.Zero) ?? 0;
}
