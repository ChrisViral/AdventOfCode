using System;
using System.Collections;
using System.Collections.Generic;

namespace AdventOfCode.Utils.Extensions;

/// <summary>
/// Enumerable extension methods
/// </summary>
public static class EnumerableExtensions
{
    #region Extension methods
    /// <summary>
    /// Adds a set of values to the collection
    /// </summary>
    /// <typeparam name="T">Type of values in the collection</typeparam>
    /// <param name="collection">Collection to add to</param>
    /// <param name="values">Values to add</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            collection.Add(value);
        }
    }

    /// <summary>
    /// Applies an action to every member of the enumerable
    /// </summary>
    /// <param name="e">Enumerable to iterate over</param>
    /// <param name="action">Action to apply</param>
    public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
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
    public static IEnumerable<T> Loop<T>(this IEnumerable<T> e, int length, bool copy = true)
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
    public static IEnumerable<T> RepeatElements<T>(this IEnumerable<T> e, int count)
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
                    foreach (int _ in ..count)
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
    public static T Sum<T>(this IEnumerable<T> e) where T : IAdditionOperators<T, T, T>
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
    public static TResult Sum<TValue, TResult>(this IEnumerable<TValue> e, Func<TValue, TResult> selector) where TResult : IAdditionOperators<TResult, TResult, TResult>
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
    public static T Multiply<T>(this IEnumerable<T> e) where T : IMultiplyOperators<T, T, T>
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
    public static TResult Multiply<TValue, TResult>(this IEnumerable<TValue> e, Func<TValue, TResult> selector) where TResult : IMultiplyOperators<TResult, TResult, TResult>
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
    #endregion
}
