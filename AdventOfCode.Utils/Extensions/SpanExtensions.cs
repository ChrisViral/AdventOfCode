using AdventOfCode.Utils.ValueEnumerators;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Spans;

/// <summary>
/// Span extensions
/// </summary>
[PublicAPI]
public static class SpanExtensions
{
    /// <param name="span">Span instance</param>
    /// <typeparam name="T">Value contained in the span</typeparam>
    extension<T>(Span2D<T> span)
    {
        public ValueEnumerable<FromSpan2D<T>, T> AsValueEnumerable()
        {
            return new ValueEnumerable<FromSpan2D<T>, T>(new FromSpan2D<T>(span));
        }
    }

    /// <param name="span">Span instance</param>
    /// <typeparam name="T">Value contained in the span</typeparam>
    extension<T>(ReadOnlySpan2D<T> span)
    {
        public ValueEnumerable<FromSpan2D<T>, T> AsValueEnumerable()
        {
            return new ValueEnumerable<FromSpan2D<T>, T>(new FromSpan2D<T>(span));
        }
    }

    /// <param name="span">Span</param>
    /// <typeparam name="T">Span value type</typeparam>
    extension<T>(ReadOnlySpan2D<T?> span) where T : IEquatable<T>
    {
        /// <summary>
        /// Coutns the occurence of a value in a ReadOnlySpan2D
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <returns>The amount of times <paramref name="value"/> appears in the span</returns>
        /// ReSharper disable once CognitiveComplexity
        public int Count(T? value)
        {
            int count = 0;
            if (value is null)
            {
                foreach (T? element in span)
                {
                    if (element is null)
                    {
                        count++;
                    }
                }
            }
            else
            {
                foreach (T? element in span)
                {
                    if (value.Equals(element))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }

    /// <param name="e">Ref enumerable</param>
    /// <typeparam name="T">Enumerable value type</typeparam>
    extension<T>(RefEnumerable<T?> e) where T : IEquatable<T>
    {
        /// <summary>
        /// Coutns the occurence of a value in a RefEnumerable
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <returns>The amount of times <paramref name="value"/> appears in the enumerable</returns>
        /// ReSharper disable once CognitiveComplexity
        public int Count(T? value)
        {
            int count = 0;
            if (value is null)
            {
                foreach (T? element in e)
                {
                    if (element is null)
                    {
                        count++;
                    }
                }
            }
            else
            {
                foreach (T? element in e)
                {
                    if (value.Equals(element))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
