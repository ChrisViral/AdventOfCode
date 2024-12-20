﻿using System;
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
    #region Extension methods
    /// <summary>
    /// Adds a set of values to the collection
    /// </summary>
    /// <typeparam name="T">Type of values in the collection</typeparam>
    /// <param name="collection">Collection to add to</param>
    /// <param name="values">Values to add</param>
    public static void AddRange<T>(this ICollection<T> collection, [InstantHandle] IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            collection.Add(value);
        }
    }

    /// <summary>
    /// Applies the given function to all members of the array
    /// </summary>
    /// <typeparam name="T">Type of element in the array</typeparam>
    /// <param name="array">Array to apply to</param>
    /// <param name="modification">Modification function</param>
    public static void Apply<T>(this IList<T> array, [InstantHandle] Func<T, T> modification)
    {
        foreach (int i in ..array.Count)
        {
            array[i] = modification(array[i]);
        }
    }

    /// <summary>
    /// Creates a copy of the given stack, preserving order
    /// </summary>
    /// <typeparam name="T">Type of element in the stack</typeparam>
    /// <param name="stack">Stack to reverse</param>
    /// <returns>A shallow copy of the stack</returns>
    public static Stack<T> CreateCopy<T>(this Stack<T> stack)
    {
        T[] array = new T[stack.Count];
        stack.CopyTo(array, 0);
        array.Reversed();
        return new Stack<T>(array);
    }

    /// <summary>
    /// Enumerates all the elements in the passed enumerable, along with the enumeration index
    /// </summary>
    /// <typeparam name="T">Type of element to enumerate</typeparam>
    /// <param name="enumerable">Enumerable</param>
    /// <returns></returns>
    [Pure, LinqTunnel]
    public static IEnumerable<(int index, T value)> Enumerate<T>([InstantHandle] this IEnumerable<T> enumerable)
    {
        int index = 0;
        foreach (T value in enumerable)
        {
            yield return (index++, value);
        }
    }

    /// <summary>
    /// Enumerate pairs of items in the given list
    /// </summary>
    /// <param name="list">List to enumerate pairs from</param>
    /// <typeparam name="T">List element type</typeparam>
    /// <returns>An exhaustive list of all item pairs in <paramref name="list"/></returns>
    public static IEnumerable<(T, T)> EnumeratePairs<T>(this IList<T> list)
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

    /// <summary>
    /// Applies an action to every member of the enumerable
    /// </summary>
    /// <param name="e">Enumerable to iterate over</param>
    /// <param name="action">Action to apply</param>
    public static void ForEach<T>([InstantHandle] this IEnumerable<T> e, [InstantHandle] Action<T> action)
    {
        foreach (T t in e)
        {
            action(t);
        }
    }

    /// <summary>
    /// Checks if a stack is empty
    /// </summary>
    /// <param name="stack">Stack to check</param>
    /// <returns>True if the stack is empty, false otherwise</returns>
    public static bool IsEmpty<T>(this Stack<T> stack) => stack.Count is 0;

    /// <summary>
    /// Checks if a queue is empty
    /// </summary>
    /// <param name="stack">Queue to check</param>
    /// <returns>True if the queue is empty, false otherwise</returns>
    public static bool IsEmpty<T>(this Queue<T> stack) => stack.Count is 0;

    /// <summary>
    /// Checks if a collection is empty
    /// </summary>
    /// <param name="collection">Collection to check</param>
    /// <returns>True if the collection is empty, false otherwise</returns>
    public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count is 0;

    /// <summary>
    /// Loops an enumerator over itself until the total length is reached
    /// </summary>
    /// <typeparam name="T">Type of element within the enumerator</typeparam>
    /// <param name="e">Enumerable to loop over</param>
    /// <param name="length">Total length to reach</param>
    /// <param name="copy">If a local copy of the enumerator should be cached, defaults to true</param>
    /// <returns>A sequence looping over the input sequence and of the specified length</returns>
    [Pure, LinqTunnel]
    public static IEnumerable<T> Loop<T>([InstantHandle] this IEnumerable<T> e, int length, bool copy = true)
    {
        //Make sure the length is valid
        if (length < 1) yield break;

        //Caching
        if (copy)
        {
            //Create cache over first iteration
            List<T> cache = new();
            foreach (T t in e)
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
            foreach (T t in e)
            {
                yield return t;
                if (--length is 0) yield break;
            }
        }
    }

    /// <summary>
    /// Returns the sequence where every element is repeated a given amount of times
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence</typeparam>
    /// <param name="e">Enumerable from which to repeat the elements</param>
    /// <param name="count">Amount of times to repeat each element</param>
    /// <returns>An enumerable where all the elements of the original sequence are repeated the specified amount of times</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="count"/> is less than or equal to zero</exception>
    [Pure, LinqTunnel]
    public static IEnumerable<T> RepeatElements<T>([InstantHandle] this IEnumerable<T> e, int count)
    {
        switch (count)
        {
            case <= 0:
                throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero");

            case 1:
                //For one simply loop through the elements
                foreach (T t in e)
                {
                    yield return t;
                }
                break;

            default:
                //Otherwise repeat each element the right amount of times
                foreach (T t in e)
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
    /// Returns the next node in a linked list in a circular fashion, wrapping back to the start after getting to the end
    /// </summary>
    /// <typeparam name="T">Type of element in the list node</typeparam>
    /// <param name="node">Current node</param>
    /// <returns>The next node in the list, or the first one if at the end</returns>
    public static LinkedListNode<T> NextCircular<T>(this LinkedListNode<T> node) => node.Next ?? node.List!.First!;

    /// <summary>
    /// Returns the previous node in a linked list in a circular fashion, wrapping back to the end after getting to the start
    /// </summary>
    /// <typeparam name="T">Type of element in the list node</typeparam>
    /// <param name="node">Current node</param>
    /// <returns>The previous node in the list, or the last one if at the start</returns>
    public static LinkedListNode<T> PreviousCircular<T>(this LinkedListNode<T> node) => node.Previous ?? node.List!.Last!;

    /// <summary>
    /// Creates an array containing all the nodes of the LinkedList
    /// </summary>
    /// <typeparam name="T">Type of element in the LinkedList</typeparam>
    /// <param name="list">List to get the nodes array for</param>
    /// <returns>An array containing all the <see cref="LinkedListNode{T}"/> contained within <paramref name="list"/></returns>
    public static LinkedListNode<T>[] ToNodeArray<T>(this LinkedList<T> list)
    {
        LinkedListNode<T>[] nodes = new LinkedListNode<T>[list.Count];
        LinkedListNode<T> current = list.First!;
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = current;
            current = current.Next!;
        }

        return nodes;
    }
    #endregion

    #region LINQ extensions
    /// <summary>
    /// Sums the given values and returns the result
    /// </summary>
    /// <typeparam name="T">Type of value to sum</typeparam>
    /// <param name="e">Enumerator to sum</param>
    /// <returns>The sum of all the values</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
    public static T Sum<T>([InstantHandle] this IEnumerable<T> e) where T : IAdditionOperators<T, T, T>
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

    /// <summary>
    /// Sums the given values and returns the result
    /// </summary>
    /// <typeparam name="TValue">Type of value to enumerator</typeparam>
    /// <typeparam name="TResult">Type of value to sum</typeparam>
    /// <param name="e">Enumerator to sum</param>
    /// <param name="selector">Selector function for the enumerated values</param>
    /// <returns>The sum of all the values</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="e"/> or <paramref name="selector"/> is null</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
    public static TResult Sum<TValue, TResult>([InstantHandle] this IEnumerable<TValue> e, [InstantHandle] Func<TValue, TResult> selector) where TResult : IAdditionOperators<TResult, TResult, TResult>
    {
        if (e is null) throw new ArgumentNullException(nameof(e), "Enumerable to sum cannot be null");
        if (selector is null) throw new ArgumentNullException(nameof(selector), "Selector function cannot be null");

        using IEnumerator<TValue> enumerator = e.GetEnumerator();
        if (!enumerator.MoveNext()) throw new InvalidOperationException("Cannot sum an empty collection");

        TResult result = selector(enumerator.Current);
        while (enumerator.MoveNext())
        {
            result += selector(enumerator.Current);
        }

        return result;
    }

    /// <summary>
    /// Multiplies the given values and returns the result
    /// </summary>
    /// <typeparam name="T">Type of value to multiply</typeparam>
    /// <param name="e">Enumerator to multiply</param>
    /// <returns>The product of all the values</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="e"/> is null</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
    public static T Multiply<T>([InstantHandle] this IEnumerable<T> e) where T : IMultiplyOperators<T, T, T>
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

    /// <summary>
    /// Multiplied the given values and returns the result
    /// </summary>
    /// <typeparam name="TValue">Type of value to enumerator</typeparam>
    /// <typeparam name="TResult">Type of value to multiply</typeparam>
    /// <param name="e">Enumerator to multiply</param>
    /// <param name="selector">Selector function for the enumerated values</param>
    /// <returns>The product of all the values</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="e"/> or <paramref name="selector"/> is null</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="e"/> is empty</exception>
    public static TResult Multiply<TValue, TResult>([InstantHandle] this IEnumerable<TValue> e, [InstantHandle] Func<TValue, TResult> selector) where TResult : IMultiplyOperators<TResult, TResult, TResult>
    {
        if (e is null) throw new ArgumentNullException(nameof(e), "Enumerable to sum cannot be null");
        if (selector is null) throw new ArgumentNullException(nameof(selector), "Selector function cannot be null");

        using IEnumerator<TValue> enumerator = e.GetEnumerator();
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
    /// <param name="e">An <see cref="IEnumerable{T}"/> to filter</param>
    /// <param name="predicate">A function to test each element for a condition</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that do not satisfy the condition.</returns>
    [Pure, LinqTunnel]
    public static IEnumerable<TSource> WhereNot<TSource>([InstantHandle] this IEnumerable<TSource> e, [InstantHandle] Func<TSource, bool> predicate)
    {
        return e.Where(predicate.Invert());
    }

    /// <summary>
    /// Filters a sequence of values based on a negated predicate. Each element's index is used in the logic of the predicate function.
    /// </summary>
    /// <param name="e">An <see cref="IEnumerable{T}"/> to filter</param>
    /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element.</param>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that do not satisfy the condition.</returns>
    [Pure, LinqTunnel]
    public static IEnumerable<TSource> WhereNot<TSource>([InstantHandle] this IEnumerable<TSource> e, [InstantHandle] Func<TSource, int, bool> predicate)
    {
        return e.Where(predicate.Invert());
    }
    #endregion
}
