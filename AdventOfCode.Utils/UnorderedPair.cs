using JetBrains.Annotations;

namespace AdventOfCode.Utils;

/// <summary>
/// Unordered pair, such as (a, b) == (b, a) on all checks
/// </summary>
/// <param name="first">First element</param>
/// <param name="second">Second element</param>
/// <typeparam name="T">Element type</typeparam>
[PublicAPI]
public readonly struct UnorderedPair<T>(T? first, T? second) : IEquatable<UnorderedPair<T>>
    where T : IEquatable<T>
{
    private static readonly EqualityComparer<T> Comparer = EqualityComparer<T>.Default;

    /// <summary>
    /// First element
    /// </summary>
    public T? First { get; } = first;

    /// <summary>
    /// Second element
    /// </summary>
    public T? Second { get; } = second;

    /// <summary>
    /// Deconstructs this pair into it's components
    /// </summary>
    /// <param name="first">First element</param>
    /// <param name="second">Second element</param>
    public void Deconstruct(out T? first, out T? second)
    {
        first = this.First;
        second = this.Second;
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)" />
    public bool Equals(in UnorderedPair<T> other) => (Comparer.Equals(this.First, other.First) && Comparer.Equals(this.Second, other.Second))
                                                  || (Comparer.Equals(this.First, other.Second) && Comparer.Equals(this.Second, other.First));

    /// <inheritdoc />
    bool IEquatable<UnorderedPair<T>>.Equals(UnorderedPair<T> other) => Equals(in other);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is UnorderedPair<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        int first = this.First?.GetHashCode() ?? 0;
        int second = this.Second?.GetHashCode() ?? 0;
        return first <= second
                   ? HashCode.Combine(first, second)
                   : HashCode.Combine(second, first);
    }

    /// <inheritdoc />
    public override string ToString() => $"({this.First?.ToString() ?? "null"}, {this.Second?.ToString() ?? "null"}";

    /// <summary>
    /// Converts a tuple to an unordered pair
    /// </summary>
    /// <param name="tuple">Tuple to create the pair from</param>
    /// <returns>The equivalent unordered pair</returns>
    public static implicit operator UnorderedPair<T>((T?, T?) tuple) => new(tuple.Item1, tuple.Item2);

    /// <summary>
    /// Converts an unordered pair to a tuple
    /// </summary>
    /// <param name="pair">Pair to create the tuple from</param>
    /// <returns>The equivalent unordered pair</returns>
    public static implicit operator (T? first, T? second)(in UnorderedPair<T> pair) => (pair.First, pair.Second);

    /// <summary>
    /// Equality check
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns><see langword="true"/> if both operands are equal, otherwise <see langword="false"/></returns>
    public static bool operator ==(in UnorderedPair<T> left, in UnorderedPair<T> right) => left.Equals(right);

    /// <summary>
    /// Inequality check
    /// </summary>
    /// <param name="left">Left operand</param>
    /// <param name="right">Right operand</param>
    /// <returns><see langword="true"/> if both operands are unequal, otherwise <see langword="false"/></returns>
    public static bool operator !=(in UnorderedPair<T> left, in UnorderedPair<T> right) => !left.Equals(right);
}
