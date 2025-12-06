using System;
using System.Numerics;
using JetBrains.Annotations;
using SpanLinq;

namespace AdventOfCode.Extensions;

[PublicAPI]
public static class SpanExtensions
{
    extension<T>(Span<T> span) where T : IMultiplyOperators<T, T, T>
    {
        public T Multiply() => span.Aggregate((a, b) => a * b);
    }

    extension<T>(ReadOnlySpan<T> span) where T : IMultiplyOperators<T, T, T>
    {
        public T Multiply() => span.Aggregate((a, b) => a * b);
    }

    extension<TIn, TOut>(Span<TIn> span) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
    {
        public TOut Multiply(Func<TIn, TOut> selector) => span.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }

    extension<TIn, TOut>(ReadOnlySpan<TIn> span) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
    {
        public TOut Multiply(Func<TIn, TOut> selector) => span.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }

    extension<T, TOperator>(SpanEnumerator<T, T, TOperator> spanEnumerator) where T : IMultiplyOperators<T, T, T>
                                                                            where TOperator : ISpanOperator<T, T>
    {
        public T Multiply() => spanEnumerator.Aggregate((a, b) => a * b);
    }

    extension<TSource, TIn, TOut, TOperator>(SpanEnumerator<TSource, TIn, TOperator> spanEnumerator) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
                                                                                         where TOperator : ISpanOperator<TSource, TIn>
    {
        public TOut Multiply(Func<TIn, TOut> selector) => spanEnumerator.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }
}
