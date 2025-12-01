using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Delegates;
using AdventOfCode.Extensions.Ranges;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Enumerables;

/// <summary>
/// Enumerable extension methods
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    /// <typeparam name="T">Type of element in the collection</typeparam>
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// Checks if a collection is empty
        /// </summary>
        /// <value>True if the collection is empty, false otherwise</value>
        public bool IsEmpty => collection.Count is 0;

        /// <summary>
        /// Adds a set of values to the collection
        /// </summary>
        /// <param name="values">Values to add</param>
        public void AddRange([InstantHandle] IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                collection.Add(value);
            }
        }
    }

    /// <typeparam name="T">Type of element in the list</typeparam>
    extension<T>(IList<T> list)
    {
        /// <summary>
        /// Applies the given function to all members of the array
        /// </summary>
        /// <param name="modification">Modification function</param>
        public void Apply([InstantHandle] Func<T, T> modification)
        {
            foreach (int i in ..list.Count)
            {
                list[i] = modification(list[i]);
            }
        }

        /// <summary>
        /// Enumerate pairs of items in the given list
        /// </summary>
        /// <returns>An exhaustive list of all item pairs in <paramref name="list"/></returns>
        public IEnumerable<(T, T)> EnumeratePairs()
        {
            if (list.Count <= 1) yield break;

            int end = list.Count - 1;
            for (int i = 0; i < end; i++)
            {
                T first = list[i];
                for (int j = i + 1; j < list.Count; j++)
                {
                    yield return (first, list[j]);
                }
            }
        }
    }

    /// <typeparam name="T">Type of element in the stack</typeparam>
    extension<T>(Stack<T> stack)
    {
        /// <summary>
        /// Checks if a stack is empty
        /// </summary>
        /// <value>True if the stack is empty, false otherwise</value>
        public bool IsEmpty => stack.Count is 0;

        /// <summary>
        /// Creates a copy of the given stack, preserving order
        /// </summary>
        /// <returns>A shallow copy of the stack</returns>
        public Stack<T> CreateCopy()
        {
            T[] array = new T[stack.Count];
            stack.CopyTo(array, 0);
            return new Stack<T>(array.Reversed());
        }
    }

    /// <typeparam name="T">Type of element in the queue</typeparam>
    extension<T>(Queue<T> stack)
    {
        /// <summary>
        /// Checks if a queue is empty
        /// </summary>
        /// <value>True if the queue is empty, false otherwise</value>
        public bool IsEmpty => stack.Count is 0;
    }

    /// <typeparam name="T">Type of element in the list node</typeparam>
    extension<T>(LinkedListNode<T> node)
    {
        /// <summary>
        /// Returns the next node in a linked list in a circular fashion, wrapping back to the start after getting to the end
        /// </summary>
        /// <returns>The next node in the list, or the first one if at the end</returns>
        public LinkedListNode<T> NextCircular() => node.Next ?? node.List!.First!;

        /// <summary>
        /// Returns the previous node in a linked list in a circular fashion, wrapping back to the end after getting to the start
        /// </summary>
        /// <returns>The previous node in the list, or the last one if at the start</returns>
        public LinkedListNode<T> PreviousCircular() => node.Previous ?? node.List!.Last!;
    }

    /// <typeparam name="T">Type of element in the LinkedList</typeparam>
    extension<T>(LinkedList<T> list)
    {
        /// <summary>
        /// Creates an array containing all the nodes of the LinkedList
        /// </summary>
        /// <returns>An array containing all the <see cref="LinkedListNode{T}"/> contained within <paramref name="list"/></returns>
        public LinkedListNode<T>[] ToNodeArray()
        {
            LinkedListNode<T>[] nodes = new LinkedListNode<T>[list.Count];
            LinkedListNode<T> current = list.First!;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = current;
                current  = current.Next!;
            }

            return nodes;
        }
    }

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
        /// <param name="copy">If a local copy of the enumerator should be cached, defaults to true</param>
        /// <returns>A sequence looping over the input sequence and of the specified length</returns>
        [Pure, LinqTunnel]
        public IEnumerable<T> Loop(int length, bool copy = true)
        {
            //Make sure the length is valid
            if (length < 1) yield break;

            //Caching
            if (copy)
            {
                //Create cache over first iteration
                List<T> cache = [];
                foreach (T t in enumerable)
                {
                    yield return t;
                    if (--length is 0) yield break;

                    cache.Add(t);
                }

                while (true)
                {
                    //Loop forever
                    foreach (T t in cache)
                    {
                        yield return t;
                        if (--length is 0) yield break;
                    }
                }
            }

            //Not caching
            while (true)
            {
                //ReSharper disable once PossibleMultipleEnumeration - intended
                foreach (T t in enumerable)
                {
                    yield return t;
                    if (--length is 0) yield break;
                }
            }
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
        /// Sums the given values and returns the result
        /// </summary>
        /// <typeparam name="TResult">Type of value to sum</typeparam>
        /// <param name="selector">Selector function for the enumerated values</param>
        /// <returns>The sum of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="enumerable"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="enumerable"/> is empty</exception>
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

    /// <param name="e">Enumerator to sum</param>
    /// <typeparam name="T">Type of value to sum</typeparam>
    extension<T>([InstantHandle] IEnumerable<T> e) where T : IAdditionOperators<T, T, T>
    {
        /// <summary>
        /// Sums the given values and returns the result
        /// </summary>
        /// <returns>The sum of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
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

    /// <param name="e">Enumerator to multiply</param>
    /// <typeparam name="T">Type of value to multiply</typeparam>
    extension<T>([InstantHandle] IEnumerable<T> e) where T : IMultiplyOperators<T, T, T>
    {
        /// <summary>
        /// Multiplies the given values and returns the result
        /// </summary>
        /// <returns>The product of all the values</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
        /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
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
}
