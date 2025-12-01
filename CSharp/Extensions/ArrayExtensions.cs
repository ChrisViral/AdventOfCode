using System;
using System.Collections;
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
    /// <typeparam name="T">Array element type</typeparam>
    extension<T>(T[] array)
    {
        /// <inheritdoc cref="Array.BinarySearch{T}(T[], T)"/>
        [PublicAPI]
        public int BinarySearch(T value) => Array.BinarySearch(array, value);

        /// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T})"/>
        public int BinarySearch(T value, IComparer<T>? comparer) => Array.BinarySearch(array, value, comparer);

        /// <inheritdoc cref="Array.Clear(Array)"/>
        public void Clear() => Array.Clear(array);

        /// <inheritdoc cref="Array.ConvertAll{T, TOutput}"/>
        public TOutput[] ConvertAll<TOutput>([InstantHandle] Converter<T, TOutput> converter) => Array.ConvertAll(array, converter);

        /// <summary>
        /// Creates a shallow copy of the specified array
        /// </summary>
        /// <typeparam name="T">Type of element in the array</typeparam>
        /// <returns>The copy of <paramref name="array"/></returns>
        public T[] Copy()
        {
            T[] copy = new T[array.Length];
            Array.Copy(array, copy, array.Length);
            return copy;
        }

        /// <inheritdoc cref="Array.Exists{T}"/>
        public bool Exists([InstantHandle] Predicate<T> predicate) => Array.Exists(array, predicate);

        /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
        public void Fill(T value) => Array.Fill(array, value);

        /// <summary>
        /// Fills the array with new values
        /// </summary>
        /// <typeparam name="T">Type of values in the array</typeparam>
        /// <param name="getValue">Value getter function</param>
        public void Fill([InstantHandle] Func<T> getValue)
        {
            foreach (int i in ..array.Length)
            {
                array[i] = getValue();
            }
        }

        /// <inheritdoc cref="Array.Find{T}"/>
        public T? Find([InstantHandle] Predicate<T> predicate) => Array.Find(array, predicate);

        /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
        public int FindIndex([InstantHandle] Predicate<T> predicate) => Array.FindIndex(array, predicate);

        /// <inheritdoc cref="Array.FindLast{T}"/>
        public T? FindLast([InstantHandle] Predicate<T> predicate) => Array.FindLast(array, predicate);

        /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
        public int FindLastIndex([InstantHandle] Predicate<T> predicate) => Array.FindLastIndex(array, predicate);

        /// <inheritdoc cref="Array.ForEach{T}"/>
        public void ForEach([InstantHandle] Action<T> action) => Array.ForEach(array, action);

        /// <inheritdoc cref="Array.IndexOf{T}(T[], T)"/>
        public int IndexOf(T value) => Array.IndexOf(array, value);

        /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T)"/>
        public int LastIndexOf(T value) => Array.LastIndexOf(array, value);

        /// <summary>
        /// Iterates over all the permutations of the given array
        /// </summary>
        /// <typeparam name="T">Type of element in the array</typeparam>
        /// <returns>An enumerable returning all the permutations of the original array</returns>
        public IEnumerable<T[]> Permutations()
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
        public void Reverse() => Array.Reverse(array);

        /// <inheritdoc cref="Array.Reverse{T}(T[])"/>
        /// <returns>The array reversed in place</returns>
        public T[] Reversed()
        {
            Array.Reverse(array);
            return array;
        }

        /// <summary>
        /// Iterates over all the permutations of the given array without allocating new memory for each permutation
        /// </summary>
        /// <returns>An enumerable returning all the permutations of the original array</returns>
        public IEnumerable<T[]> PermutationsInPlace()
        {
            static IEnumerable<T[]> GetPermutations(T[] output, int k)
            {
                if (k == output.Length - 1)
                {
                    yield return output;
                    yield break;
                }

                for (int i = k; i < output.Length; i++)
                {
                    (output[k], output[i]) = (output[i], output[k]);
                    foreach (T[] perm in GetPermutations(output, k + 1))
                    {
                        yield return perm;
                    }
                    (output[k], output[i]) = (output[i], output[k]);
                }
            }

            T[] output = new T[array.Length];
            array.CopyTo(output, 0);
            return GetPermutations(output, 0);
        }

        /// <inheritdoc cref="Array.Sort{T}(T[])"/>
        public void Sort() => Array.Sort(array);

        /// <inheritdoc cref="Array.Sort{T}(T[], IComparer{T})"/>
        public void Sort(IComparer<T> comparer) => Array.Sort(array, comparer);

        /// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
        public void Sort([InstantHandle] Comparison<T> comparison) => Array.Sort(array, comparison);

        /// <inheritdoc cref="Array.TrueForAll{T}"/>
        public bool TrueForAll([InstantHandle] Predicate<T> predicate) => Array.TrueForAll(array, predicate);

    }

    /// <typeparam name="T">Array element type</typeparam>
    extension<T>(ArraySegment<T> array)
    {
        /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
        public void Fill(T value) => array.AsSpan().Fill(value);
    }
}
