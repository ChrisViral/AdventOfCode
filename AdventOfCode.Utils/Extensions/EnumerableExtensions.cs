using System.Numerics;
using AdventOfCode.Utils.Extensions.Delegates;
using AdventOfCode.Utils.ValueEnumerators;
using CommunityToolkit.HighPerformance.Enumerables;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Enumerables;

/// <summary>
/// Enumerable extension methods
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    /// <param name="enumerable">Enumerable</param>
    /// <typeparam name="T">Type of element to enumerate</typeparam>
    extension<T>([InstantHandle] IEnumerable<T> enumerable)
    {
        /// <summary>
        /// Enumerates all the elements in the passed enumerable, along with the enumeration index
        /// </summary>
        /// <returns></returns>
        [Pure, LinqTunnel]
        public IEnumerable<(int index, T value)> Enumerate()
        {
            int index = 0;
            foreach (T value in enumerable)
            {
                yield return (index++, value);
            }
        }

        /// <summary>
        /// Applies an action to every member of the enumerable
        /// </summary>
        /// <param name="action">Action to apply</param>
        public void ForEach([InstantHandle] Action<T> action)
        {
            foreach (T t in enumerable)
            {
                action(t);
            }
        }

        /// <summary>
        /// Loops an enumerator over itself until the total length is reached
        /// </summary>
        /// <param name="length">Total length to reach</param>
        /// <param name="cache">If a local copy of the enumerator should be cached, defaults to true</param>
        /// <returns>A sequence looping over the input sequence and of the specified length</returns>
        [Pure, LinqTunnel]
        public IEnumerable<T> Loop(int length, bool cache = true)
        {
            //Make sure the length is valid
            if (length < 1) return Enumerable.Empty<T>();

            if (!cache)
            {
                return enumerable.LoopWithoutCache(length);
            }

            if (enumerable.TryGetNonEnumeratedCount(out int count))
            {
                return count > length ? enumerable.LoopWithCache(length, count) : enumerable.LoopWithoutCache(length);
            }

            return enumerable.LoopWithCache(length, null);

        }

        /// <summary>
        /// Loops an enumerator over itself while caching the results until the total length is reached
        /// </summary>
        /// <param name="length">Total length to reach</param>
        /// <param name="count">Required cache size, if available</param>
        /// <returns>A sequence looping over the input sequence and of the specified length</returns>
        [Pure, LinqTunnel]
        private IEnumerable<T> LoopWithCache(int length, int? count)
        {
            //Create cache over first iteration
            List<T> cache = count.HasValue ? new List<T>(count.Value) : [];
            foreach (T t in enumerable)
            {
                yield return t;
                if (length --> 0) yield break;

                cache.Add(t);
            }

            while (true)
            {
                //Loop forever
                foreach (T t in cache)
                {
                    yield return t;
                    if (length --> 0) yield break;
                }
            }
        }

        /// <summary>
        /// Loops an enumerator over itself whithout cachings until the total length is reached
        /// </summary>
        /// <param name="length">Total length to reach</param>
        /// <returns>A sequence looping over the input sequence and of the specified length</returns>
        [Pure, LinqTunnel]
        private IEnumerable<T> LoopWithoutCache(int length)
        {
            //Not caching
            while (true)
            {
                //ReSharper disable once PossibleMultipleEnumeration - intended
                foreach (T t in enumerable)
                {
                    yield return t;
                    if (length --> 0) yield break;
                }
            }
        }

        /// <summary>
        /// Creates an enumerable of pairs of elements
        /// </summary>
        /// <param name="isCircular">If the list is circular (last element is paired with the first element)</param>
        /// <returns>Pairs of subsequent elements in the enumerable</returns>
        [Pure, LinqTunnel]
        public IEnumerable<(T first, T second)> Pairwise(bool isCircular = false)
        {
            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext()) yield break;

            T first = enumerator.Current;
            T previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                yield return (previous, current);
                previous = current;
            }

            if (isCircular) yield return (previous, first);
        }

        /// <summary>
        /// Returns the sequence where every element is repeated a given amount of times
        /// </summary>
        /// <param name="count">Amount of times to repeat each element</param>
        /// <returns>An enumerable where all the elements of the original sequence are repeated the specified amount of times</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="count"/> is less than or equal to zero</exception>
        [Pure, LinqTunnel]
        public IEnumerable<T> RepeatElements(int count)
        {
            switch (count)
            {
                case <= 0:
                    throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero");

                case 1:
                    //For one simply loop through the elements
                    foreach (T t in enumerable)
                    {
                        yield return t;
                    }
                    break;

                default:
                    //Otherwise repeat each element the right amount of times
                    foreach (T t in enumerable)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            yield return t;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Counts the instance of a value in the enumerable
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <returns>The amount of times <paramref name="value"/> is found in the enumerable</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="enumerable"/> is <see langword="null"/></exception>
        [Pure]
        public int Count(T value) => enumerable.Count(value, EqualityComparer<T>.Default);

        /// <summary>
        /// Counts the instance of a value in the enumerable
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <param name="comparer">Equality comparer</param>
        /// <returns>The amount of times <paramref name="value"/> is found in the enumerable</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="enumerable"/> is <see langword="null"/></exception>
        [Pure]
        public int Count(T value, IEqualityComparer<T> comparer)
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable), "Enumerable to sum cannot be null");

            int count = 0;
            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (comparer.Equals(value, enumerator.Current))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Sums the given values and returns the result
        /// </summary>
        /// <typeparam name="TResult">Type of value to sum</typeparam>
        /// <param name="selector">Selector function for the enumerated values</param>
        /// <returns>The sum of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enumerable"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="enumerable"/> is empty</exception>
        [Pure]
        public TResult Sum<TResult>([InstantHandle] Func<T, TResult> selector) where TResult : IAdditionOperators<TResult, TResult, TResult>
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable), "Enumerable to sum cannot be null");
            if (selector is null) throw new ArgumentNullException(nameof(selector), "Selector function cannot be null");

            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot sum an empty collection");

            TResult result = selector(enumerator.Current);
            while (enumerator.MoveNext())
            {
                result += selector(enumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Multiplied the given values and returns the result
        /// </summary>
        /// <typeparam name="TResult">Type of value to multiply</typeparam>
        /// <param name="selector">Selector function for the enumerated values</param>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException">If the enumerable is empty</exception>
        [Pure]
        public TResult Multiply<TResult>([InstantHandle] Func<T, TResult> selector) where TResult : IMultiplyOperators<TResult, TResult, TResult>
        {
            if (enumerable is null) throw new ArgumentNullException(nameof(enumerable), "Enumerable to sum cannot be null");
            if (selector is null) throw new ArgumentNullException(nameof(selector), "Selector function cannot be null");

            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot multiply an empty collection");

            TResult result = selector(enumerator.Current);
            while (enumerator.MoveNext())
            {
                result *= selector(enumerator.Current);
            }

            return result;
        }

        /// <summary>
        /// Filters a sequence of values based on a negated predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that do not satisfy the condition.</returns>
        [Pure, LinqTunnel]
        public IEnumerable<T> WhereNot([InstantHandle] Func<T, bool> predicate) => enumerable.Where(predicate.Inverted);

        /// <summary>
        /// Filters a sequence of values based on a negated predicate. Each element's index is used in the logic of the predicate function.
        /// </summary>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that do not satisfy the condition.</returns>
        [Pure, LinqTunnel]
        public IEnumerable<T> WhereNot([InstantHandle] Func<T, int, bool> predicate) => enumerable.Where(predicate.Inverted);
    }

    /// <param name="e">Enumerable to sum</param>
    /// <typeparam name="T">Type of value to sum</typeparam>
    extension<T>([InstantHandle] IEnumerable<T> e) where T : IAdditionOperators<T, T, T>
    {
        /// <summary>
        /// Sums the given values and returns the result
        /// </summary>
        /// <returns>The sum of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
        [Pure]
        public T Sum()
        {
            if (e is null) throw new ArgumentNullException(nameof(e), "Enumerable to sum cannot be null");

            using IEnumerator<T> enumerator = e.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot sum an empty collection");

            T result = enumerator.Current;
            while (enumerator.MoveNext())
            {
                result += enumerator.Current;
            }

            return result;
        }
    }

    /// <param name="e">Enumerable to multiply</param>
    /// <typeparam name="T">Type of value to multiply</typeparam>
    extension<T>([InstantHandle] IEnumerable<T> e) where T : IMultiplyOperators<T, T, T>
    {
        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
        [Pure]
        public T Multiply()
        {
            if (e is null) throw new ArgumentNullException(nameof(e), "Enumerable to sum cannot be null");

            using IEnumerator<T> enumerator = e.GetEnumerator();
            if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot multiply an empty collection");

            T result = enumerator.Current;
            while (enumerator.MoveNext())
            {
                result *= enumerator.Current;
            }

            return result;
        }
    }

    /// <param name="enumerable">Enumerable to multiply</param>
    /// <typeparam name="TEnumerator">Enumerator type</typeparam>
    /// <typeparam name="TSource">Type of value to multiply</typeparam>
    extension<TEnumerator, TSource>(ValueEnumerable<TEnumerator, TSource> enumerable)
        where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
    {
        /// <summary>
        /// Applies an action to each member of the enumerable
        /// </summary>
        /// <param name="action">Action to apply</param>
        public void ForEach([InstantHandle] Action<TSource> action)
        {
            using TEnumerator enumerator = enumerable.Enumerator;
            while (enumerator.TryGetNext(out TSource element))
            {
                action(element);
            }
        }

        /// <summary>
        /// Counts the instance of a value in the enumerable
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <returns>The amount of times <paramref name="value"/> is found in the enumerable</returns>
        [Pure]
        public int Count(TSource value) => enumerable.Count(value, EqualityComparer<TSource>.Default);

        /// <summary>
        /// Counts the instance of a value in the enumerable
        /// </summary>
        /// <param name="value">Value to count</param>
        /// <param name="comparer">Equality comparer</param>
        /// <returns>The amount of times <paramref name="value"/> is found in the enumerable</returns>
        [Pure]
        public int Count(TSource value, IEqualityComparer<TSource> comparer)
        {
            int count = 0;
            using TEnumerator enumerator = enumerable.Enumerator;
            while (enumerator.TryGetNext(out TSource current))
            {
                if (comparer.Equals(value, current))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enumerable"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="enumerable"/> is empty</exception>
        [Pure]
        public TResult Sum<TResult>([InstantHandle] Func<TSource, TResult> selector)
            where TResult : IAdditionOperators<TResult, TResult, TResult>
        {
            using TEnumerator enumerator = enumerable.Enumerator;
            if (!enumerator.TryGetNext(out TSource element)) throw new InvalidOperationException("Cannot multiply an empty collection");

            TResult result = selector(element);
            while (enumerator.TryGetNext(out element))
            {
                result += selector(element);
            }
            return result;
        }

        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enumerable"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="enumerable"/> is empty</exception>
        [Pure]
        public TResult Multiply<TResult>([InstantHandle] Func<TSource, TResult> selector)
            where TResult : IMultiplyOperators<TResult, TResult, TResult>
        {
            using TEnumerator enumerator = enumerable.Enumerator;
            if (!enumerator.TryGetNext(out TSource element)) throw new InvalidOperationException("Cannot multiply an empty collection");

            TResult result = selector(element);
            while (enumerator.TryGetNext(out element))
            {
                result *= selector(element);
            }
            return result;
        }
    }

    /// <param name="e">Enumerable to multiply</param>
    /// <typeparam name="TEnumerator">Enumerator type</typeparam>
    /// <typeparam name="TSource">Type of value to multiply</typeparam>
    extension<TEnumerator, TSource>(ValueEnumerable<TEnumerator, TSource> e)
        where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
        where TSource : IAdditionOperators<TSource, TSource, TSource>
    {
        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
        [Pure]
        public TSource Sum()
        {
            using TEnumerator enumerator = e.Enumerator;
            if (!enumerator.TryGetNext(out TSource result)) throw new InvalidOperationException("Cannot multiply an empty collection");

            while (enumerator.TryGetNext(out TSource current))
            {
                result += current;
            }
            return result;
        }
    }

    /// <param name="e">Enumerable to multiply</param>
    /// <typeparam name="TEnumerator">Enumerator type</typeparam>
    /// <typeparam name="TSource">Type of value to multiply</typeparam>
    extension<TEnumerator, TSource>(ValueEnumerable<TEnumerator, TSource> e)
        where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
        where TSource : IMultiplyOperators<TSource, TSource, TSource>
    {
        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
        [Pure]
        public TSource Multiply()
        {
            using TEnumerator enumerator = e.Enumerator;
            if (!enumerator.TryGetNext(out TSource result)) throw new InvalidOperationException("Cannot multiply an empty collection");

            while (enumerator.TryGetNext(out TSource current))
            {
                result *= current;
            }
            return result;
        }
    }

    /// <param name="enumerable">Ref enumerable</param>
    /// <typeparam name="T">Enumerable value type</typeparam>
    extension<T>(RefEnumerable<T> enumerable)
    {
        /// <summary>
        /// Returns this RefEnumerable as a ValueEnumerable
        /// </summary>
        /// <returns>ValueEnumerable instance</returns>
        public ValueEnumerable<FromRefEnumerable<T>, T> AsValueEnumerable()
        {
            return new ValueEnumerable<FromRefEnumerable<T>, T>(new FromRefEnumerable<T>(enumerable));
        }
    }
}
