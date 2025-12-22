using System.Numerics;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using SpanLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Spans;

[PublicAPI]
public static class SpanExtensions
{
    /// <param name="span">Span</param>
    /// <typeparam name="T">Span value type</typeparam>
    extension<T>(ReadOnlySpan<T> span) where T : IMultiplyOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply() => span.Aggregate((a, b) => a * b);
    }

    /// <param name="span">Span</param>
    /// <typeparam name="TIn">Input value type</typeparam>
    /// <typeparam name="TOut">Output value type</typeparam>
    extension<TIn, TOut>(ReadOnlySpan<TIn> span) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply(Func<TIn, TOut> selector) => span.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
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

    /// <param name="span">Span</param>
    /// <typeparam name="T">Span value type</typeparam>
    extension<T>(ReadOnlySpan2D<T?> span) where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        /// <summary>
        /// Sums the members of a ReadOnlySpan2D
        /// </summary>
        /// <returns>The sum of all the members in the span</returns>
        public T Sum()
        {
            T result = T.AdditiveIdentity;
            foreach (T? value in span)
            {
                if (value is not null)
                {
                    result += value;
                }
            }
            return result;
        }
    }

    /// <param name="span">Span</param>
    /// <typeparam name="TValue">Span value type</typeparam>
    /// <typeparam name="TSum">Summation value type</typeparam>
    extension<TValue, TSum>(ReadOnlySpan2D<TValue?> span) where TSum : IAdditionOperators<TSum, TSum, TSum>, IAdditiveIdentity<TSum, TSum>
    {
        /// <summary>
        /// Sums the members of a ReadOnlySpan2D
        /// </summary>
        /// <returns>The sum of all the members in the span</returns>
        public TSum Sum([InstantHandle] Func<TValue, TSum> selector)
        {
            TSum result = TSum.AdditiveIdentity;
            foreach (TValue? value in span)
            {
                if (value is not null)
                {
                    result += selector(value);
                }
            }
            return result;
        }
    }

    /// <param name="spanEnumerator">Span enumerator</param>
    /// <typeparam name="TSource">Enumerator source value type</typeparam>
    /// <typeparam name="TOut">Enumerator output value type</typeparam>
    /// <typeparam name="TOperator">Operation value type</typeparam>
    extension<TSource, TOut, TOperator>(SpanEnumerator<TSource, TOut, TOperator> spanEnumerator) where TOut : IMultiplyOperators<TOut, TOut, TOut>
                                                                                                 where TOperator : ISpanOperator<TSource, TOut>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply() => spanEnumerator.Aggregate((a, b) => a * b);
    }

    /// <param name="spanEnumerator">Span enumerator</param>
    /// <typeparam name="TSource">Enumerator source value type</typeparam>
    /// <typeparam name="TIn">Enumerator input value type</typeparam>
    /// <typeparam name="TOut">Enumerator output value type</typeparam>
    /// <typeparam name="TOperator">Operation value type</typeparam>
    extension<TSource, TIn, TOut, TOperator>(SpanEnumerator<TSource, TIn, TOperator> spanEnumerator) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
                                                                                                     where TOperator : ISpanOperator<TSource, TIn>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply(Func<TIn, TOut> selector) => spanEnumerator.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
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
