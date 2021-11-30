using System;

namespace AdventOfCode.Search;

/// <summary>
/// Search node basic implementation interface
/// </summary>
public interface ISearchNode
{
    #region Properties
    /// <summary>
    /// Cost of the node
    /// </summary>
    public double Cost { get; }
    #endregion
}
    
/// <summary>
/// Generic search node
/// </summary>
/// <typeparam name="T">Type of element stored by the node</typeparam>
public class SearchNode<T> : ISearchNode, IEquatable<SearchNode<T>>, IComparable<SearchNode<T>> where T : IEquatable<T>
{
    /// <summary>
    /// Search heuristic function
    /// </summary>
    /// <param name="value">Value to apply the heuristic function onto</param>
    /// <returns>Heuristic value</returns>
    public delegate double Heuristic(T value);

    #region Fields
    /// <summary>Heuristic function of the node</summary>
    private readonly Heuristic? heuristic;
    #endregion
        
    #region Properties
    /// <summary>
    /// Cost to reach the node so far
    /// </summary>
    public double CostSoFar { get; }
    /// <summary>
    /// Value of the node
    /// </summary>
    public T Value { get; }
    /// <summary>
    /// Parent node
    /// </summary>
    public SearchNode<T>? Parent { get; }
    /// <summary>
    /// Cost of this node
    /// </summary>
    public double Cost => this.CostSoFar + this.heuristic?.Invoke(this.Value) ?? 0d;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new root node
    /// </summary>
    /// <param name="value">Value of the node</param>
    public SearchNode(T value)
    {
        this.CostSoFar = 0d;
        this.Value = value;
    }
        
    /// <summary>
    /// Creates a new SearchNode for the specified value
    /// </summary>
    /// <param name="cost">Cost of the node so far</param>
    /// <param name="value">Value of the node</param>
    /// <param name="heuristic">Heuristic function for this node</param>
    /// <param name="parent">Parent node</param>
    public SearchNode(double cost, T value, Heuristic heuristic, SearchNode<T> parent)
    {
        this.CostSoFar = cost;
        this.Value = value;
        this.heuristic = heuristic;
        this.Parent = parent;
    }
    #endregion
        
    #region Methods
    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? obj) => obj is SearchNode<T> other && this.Value.Equals(other.Value);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(SearchNode<T>? other) => other is not null && this.Value.Equals(other.Value);

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"{{Node: {this.Value}, Cost: {this.Cost}}}";

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    public int CompareTo(SearchNode<T>? other) => other is not null ? this.Cost.CompareTo(other.Cost) : -1;
    #endregion

    #region Operators
    /// <summary>
    /// Equality operator between two search nodes
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <returns>True if both nodes are equal, false otherwise</returns>
    public static bool operator ==(SearchNode<T> a, SearchNode<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality operator between two search nodes
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <returns>True if both nodes are unequal, false otherwise</returns>
    public static bool operator !=(SearchNode<T> a, SearchNode<T> b) => !a.Equals(b);
        
    /// <summary>
    /// Equality operator between a search node and a value
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Value</param>
    /// <returns>True if the value of the node equals the other value, false otherwise</returns>
    public static bool operator ==(SearchNode<T> a, T b) => a.Value.Equals(b);
        
    /// <summary>
    /// Inequality operator between a search node and a value
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Value</param>
    /// <returns>True if the value of the node is not equals the other value, false otherwise</returns>
    public static bool operator !=(SearchNode<T> a, T b) => !a.Value.Equals(b);
    #endregion
}