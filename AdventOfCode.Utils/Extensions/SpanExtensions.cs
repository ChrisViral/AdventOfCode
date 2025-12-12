using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SpanLinq;

namespace AdventOfCode.Utils.Extensions;

[PublicAPI]
public static class SpanExtensions
{
    extension<T>(Span<T> span) where T : IMultiplyOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply() => span.Aggregate((a, b) => a * b);
    }

    extension<T>(ReadOnlySpan<T> span) where T : IMultiplyOperators<T, T, T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Multiply() => span.Aggregate((a, b) => a * b);
    }

    extension<TIn, TOut>(Span<TIn> span) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply(Func<TIn, TOut> selector) => span.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }

    extension<TIn, TOut>(ReadOnlySpan<TIn> span) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply(Func<TIn, TOut> selector) => span.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }

    extension<TSource, TOut, TOperator>(SpanEnumerator<TSource, TOut, TOperator> spanEnumerator) where TOut : IMultiplyOperators<TOut, TOut, TOut>
                                                                                                 where TOperator : ISpanOperator<TSource, TOut>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply() => spanEnumerator.Aggregate((a, b) => a * b);
    }

    extension<TSource, TIn, TOut, TOperator>(SpanEnumerator<TSource, TIn, TOperator> spanEnumerator) where TOut : IMultiplicativeIdentity<TOut, TOut>, IMultiplyOperators<TOut, TOut, TOut>
                                                                                                     where TOperator : ISpanOperator<TSource, TIn>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TOut Multiply(Func<TIn, TOut> selector) => spanEnumerator.Aggregate(TOut.MultiplicativeIdentity, (a, b) => a * selector(b));
    }
}
