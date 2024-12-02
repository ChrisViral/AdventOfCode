using System;
using System.Collections.Generic;
using AdventOfCode.Extensions.Ranges;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Extensions.Arrays;

/// <summary>
/// Array extension methods
/// </summary>
[PublicAPI]
public static class ArrayExtensions
{
    #region Extension methods
    /// <inheritdoc cref="Array.BinarySearch{T}(T[], T)"/>
    public static int BinarySearch<T>(this T[] array, T value) => Array.BinarySearch(array, value);

    /// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T})"/>
    public static int BinarySearch<T>(this T[] array, T value, IComparer<T>? comparer) => Array.BinarySearch(array, value, comparer);

    /// <inheritdoc cref="Array.Clear(Array)"/>
    public static void Clear<T>(this T[] array) => Array.Clear(array);

    /// <inheritdoc cref="Array.ConvertAll{T, TOutput}"/>
    public static TOutput[] ConvertAll<T, TOutput>(this T[] array, [InstantHandle] Converter<T, TOutput> converter) => Array.ConvertAll(array, converter);

    /// <summary>
    /// Creates a shallow copy of the specified array
    /// </summary>
    /// <typeparam name="T">Type of element in the array</typeparam>
    /// <param name="array">The array to copy</param>
    /// <returns>The copy of <paramref name="array"/></returns>
    public static T[] Copy<T>(this T[] array)
    {
        T[] copy = new T[array.Length];
        Array.Copy(array, copy, array.Length);
        return copy;
    }

    /// <inheritdoc cref="Array.Exists{T}"/>
    public static bool Exists<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.Exists(array, predicate);

    /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
    public static void Fill<T>(this T[] array, T value) => Array.Fill(array, value);

    /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
    public static void Fill<T>(this ArraySegment<T> array, T value) => array.AsSpan().Fill(value);

    /// <summary>
    /// Fills the array with new values
    /// </summary>
    /// <typeparam name="T">Type of values in the array</typeparam>
    /// <param name="array">Array to fill</param>
    /// <param name="getValue">Value getter function</param>
    public static void Fill<T>(this T[] array, [InstantHandle] Func<T> getValue)
    {
        foreach (int i in ..array.Length)
        {
            array[i] = getValue();
        }
    }

    /// <inheritdoc cref="Array.Find{T}"/>
    public static T? Find<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.Find(array, predicate);

    /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
    public static int FindIndex<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.FindIndex(array, predicate);

    /// <inheritdoc cref="Array.FindLast{T}"/>
    public static T? FindLast<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.FindLast(array, predicate);

    /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
    public static int FindLastIndex<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.FindLastIndex(array, predicate);

    /// <inheritdoc cref="Array.ForEach{T}"/>
    public static void ForEach<T>(this T[] array, [InstantHandle] Action<T> action) => Array.ForEach(array, action);

    /// <inheritdoc cref="Array.IndexOf{T}(T[], T)"/>
    public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);

    /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T)"/>
    public static int LastIndexOf<T>(this T[] array, T value) => Array.LastIndexOf(array, value);

    /// <summary>
    /// Iterates over all the permutations of the given array
    /// </summary>
    /// <typeparam name="T">Type of element in the array</typeparam>
    /// <param name="array">Array to get the permutations for</param>
    /// <returns>An enumerable returning all the permutations of the original array</returns>
    public static IEnumerable<T[]> Permutations<T>(this T[] array)
    {
        static IEnumerable<T[]> GetPermutations(T[] working, int k)
        {
            if (k == working.Length - 1)
            {
                T[] perm = working.Copy();
                yield return perm;
                yield break;
            }

            for (int i = k; i < working.Length; i++)
            {
                (working[k], working[i]) = (working[i], working[k]);
                foreach (T[] perm in GetPermutations(working, k + 1))
                {
                    yield return perm;
                }
                (working[k], working[i]) = (working[i], working[k]);
            }
        }

        return GetPermutations(array.Copy(), 0);
    }

    /// <inheritdoc cref="Array.Reverse{T}(T[])"/>
    public static void Reversed<T>(this T[] array) => Array.Reverse(array);

    /// <inheritdoc cref="Array.Sort{T}(T[])"/>
    public static void Sort<T>(this T[] array) => Array.Sort(array);

    /// <inheritdoc cref="Array.Sort{T}(T[], IComparer{T})"/>
    public static void Sort<T>(this T[] array, IComparer<T> comparer) => Array.Sort(array, comparer);

    /// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
    public static void Sort<T>(this T[] array, [InstantHandle] Comparison<T> comparison) => Array.Sort(array, comparison);

    /// <inheritdoc cref="Array.TrueForAll{T}"/>
    public static bool TrueForAll<T>(this T[] array, [InstantHandle] Predicate<T> predicate) => Array.TrueForAll(array, predicate);
    #endregion
}