using System;
using AdventOfCode.Extensions.Ranges;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Spans;

/// <summary>
/// Spans extensions
/// </summary>
[PublicAPI]
public static class SpansExtension
{
    /// <summary>
    /// Aggregates a span into a singular value
    /// </summary>
    /// <param name="values">Values to aggregate</param>
    /// <param name="aggregator">Aggregator function</param>
    /// <typeparam name="TValue">Span value type</typeparam>
    /// <returns>The resulting aggregate value</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="values"/> is empty</exception>
    public static TValue Aggregate<TValue>(this in Span<TValue> values, [InstantHandle] Func<TValue, TValue, TValue> aggregator)
    {
        if (values.IsEmpty) throw new InvalidOperationException("Span to aggregate must contain at least one value");

        return Aggregate(values[1..], values[0], aggregator);
    }

    /// <summary>
    /// Aggregates a span into a singular value
    /// </summary>
    /// <param name="values">Values to aggregate</param>
    /// <param name="seed">Seeded aggregation value</param>
    /// <param name="aggregator">Aggregator function</param>
    /// <typeparam name="TValue">Span value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    /// <returns>The resulting aggregate value</returns>
    public static TResult Aggregate<TValue, TResult>(this in Span<TValue> values, TResult seed, [InstantHandle] Func<TResult, TValue, TResult> aggregator)
    {
        if (values.IsEmpty) return seed;

        TResult result = seed;
        foreach (int i in ..values.Length)
        {
            result = aggregator(result, values[i]);
        }
        return result;
    }

    /// <summary>
    /// Aggregates a span into a singular value
    /// </summary>
    /// <param name="values">Values to aggregate</param>
    /// <param name="aggregator">Aggregator function</param>
    /// <typeparam name="TValue">Span value type</typeparam>
    /// <returns>The resulting aggregate value</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="values"/> is empty</exception>
    public static TValue Aggregate<TValue>(this in ReadOnlySpan<TValue> values, [InstantHandle] Func<TValue, TValue, TValue> aggregator)
    {
        if (values.IsEmpty) throw new InvalidOperationException("Span to aggregate must contain at least one value");

        return Aggregate(values[1..], values[0], aggregator);
    }

    /// <summary>
    /// Aggregates a span into a singular value
    /// </summary>
    /// <param name="values">Values to aggregate</param>
    /// <param name="seed">Seeded aggregation value</param>
    /// <param name="aggregator">Aggregator function</param>
    /// <typeparam name="TValue">Span value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    /// <returns>The resulting aggregate value</returns>
    public static TResult Aggregate<TValue, TResult>(this in ReadOnlySpan<TValue> values, TResult seed, [InstantHandle] Func<TResult, TValue, TResult> aggregator)
    {
        if (values.IsEmpty) return seed;

        TResult result = seed;
        foreach (int i in ..values.Length)
        {
            result = aggregator(result, values[i]);
        }
        return result;
    }
}