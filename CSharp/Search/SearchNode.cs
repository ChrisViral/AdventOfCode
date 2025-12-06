using System;
using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Search;

/// <summary>
/// Search node basic implementation interface
/// </summary>
/// <typeparam name="TCost">Cost type</typeparam>
[PublicAPI]
public interface ISearchNode<out TCost>
{
    /// <summary>
    /// Cost of the node
    /// </summary>
    public TCost Cost { get; }
}

/// <summary>
/// Generic search node
/// </summary>
/// <typeparam name="TValue">Type of element stored by the node</typeparam>
/// <typeparam name="TCost">Cost type</typeparam>
[PublicAPI]
public class SearchNode<TValue, TCost> : ISearchNode<TCost>, IEquatable<SearchNode<TValue, TCost>>, IComparable<SearchNode<TValue, TCost>>
    where TValue : IEquatable<TValue>
    where TCost : INumber<TCost>
{
    /// <summary>
    /// Search heuristic function
    /// </summary>
    /// <param name="value">Value to apply the heuristic function onto</param>
    /// <returns>Heuristic value</returns>
    public delegate TCost Heuristic(TValue value);

    /// <summary>Heuristic function of the node</summary>
    private readonly Heuristic? heuristic;

    /// <summary>
    /// Cost to reach the node so far
    /// </summary>
    public TCost CostSoFar { get; }
    /// <summary>
    /// Value of the node
    /// </summary>
    public TValue Value { get; }
    /// <summary>
    /// Parent node
    /// </summary>
    public SearchNode<TValue, TCost>? Parent { get; }
    /// <summary>
    /// Cost of this node
    /// </summary>
    public virtual TCost Cost => this.CostSoFar + (this.heuristic is not null ? this.heuristic(this.Value) : TCost.Zero);

    /// <summary>
    /// Creates a new root node
    /// </summary>
    /// <param name="value">Value of the node</param>
    public SearchNode(TValue value)
    {
        this.CostSoFar = TCost.Zero;
        this.Value = value;
    }

    /// <summary>
    /// Creates a new SearchNode for the specified value
    /// </summary>
    /// <param name="cost">Cost of the node so far</param>
    /// <param name="value">Value of the node</param>
    /// <param name="heuristic">Heuristic function for this node</param>
    /// <param name="parent">Parent node</param>
    public SearchNode(TCost cost, TValue value, Heuristic? heuristic, SearchNode<TValue, TCost> parent)
    {
        this.CostSoFar = cost;
        this.Value = value;
        this.heuristic = heuristic;
        this.Parent = parent;
    }

    /// <summary>
    /// Check if the parent node or any of its parents has the given search value
    /// </summary>
    /// <param name="value">Search value to find</param>
    /// <returns><see langword="true"/> true if any of the parents contain the given value, else <see langword="false"/></returns>
    public bool HasParent(TValue value) => this.Parent is not null && (this.Parent == value || this.Parent.HasParent(value));

    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? obj) => obj is SearchNode<TValue, TCost> other && this.Value.Equals(other.Value);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(SearchNode<TValue, TCost>? other) => other is not null && this.Value.Equals(other.Value);

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"{{Node: {this.Value}, Cost: {this.Cost}}}";

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    public int CompareTo(SearchNode<TValue, TCost>? other) => other is not null ? this.Cost.CompareTo(other.Cost) : -1;

    /// <summary>
    /// Equality operator between two search nodes
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <returns>True if both nodes are equal, false otherwise</returns>
    public static bool operator ==(SearchNode<TValue, TCost> a, SearchNode<TValue, TCost> b) => a.Equals(b);

    /// <summary>
    /// Inequality operator between two search nodes
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <returns>True if both nodes are unequal, false otherwise</returns>
    public static bool operator !=(SearchNode<TValue, TCost> a, SearchNode<TValue, TCost> b) => !a.Equals(b);

    /// <summary>
    /// Equality operator between a search node and a value
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Value</param>
    /// <returns>True if the value of the node equals the other value, false otherwise</returns>
    public static bool operator ==(SearchNode<TValue, TCost> a, TValue b) => a.Value.Equals(b);

    /// <summary>
    /// Inequality operator between a search node and a value
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Value</param>
    /// <returns>True if the value of the node is not equals the other value, false otherwise</returns>
    public static bool operator !=(SearchNode<TValue, TCost> a, TValue b) => !a.Value.Equals(b);
}

/// <summary>
/// Generic search node
/// </summary>
/// <typeparam name="T">Type of element stored by the node</typeparam>
public sealed class SearchNode<T> : SearchNode<T, int> where T : IEquatable<T>
{
    public override int Cost => this.CostSoFar;

    public new SearchNode<T>? Parent => base.Parent as SearchNode<T>;

    /// <inheritdoc />
    public SearchNode(T value) : base(value) { }

    /// <inheritdoc />
    public SearchNode(int cost, T value, SearchNode<T, int> parent) : base(cost, value, null, parent) { }
}
