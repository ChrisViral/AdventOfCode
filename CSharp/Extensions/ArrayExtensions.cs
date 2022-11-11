using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AdventOfCode.Extensions;

/// <summary>
/// Array extension methods
/// </summary>
public static class ArrayExtensions
{
    #region Extension methods
    /// <inheritdoc cref="Array.AsReadOnly{T}"/>
    public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array) => Array.AsReadOnly(array);

    /// <inheritdoc cref="Array.BinarySearch{T}(T[], T)"/>
    public static int BinarySearch<T>(this T[] array, T value) => Array.BinarySearch(array, value);

    /// <inheritdoc cref="Array.BinarySearch{T}(T[], T, IComparer{T})"/>
    public static int BinarySearch<T>(this T[] array, T value, IComparer<T>? comparer) => Array.BinarySearch(array, value, comparer);

    /// <inheritdoc cref="Array.Clear(Array)"/>
    public static void Clear<T>(this T[] array) => Array.Clear(array);

    /// <inheritdoc cref="Array.ConvertAll{T, TOutput}"/>
    public static TOutput[] ConvertAll<T, TOutput>(this T[] array, Converter<T, TOutput> converter) => Array.ConvertAll(array, converter);

    /// <summary>
    /// Creates a shallow copy of the specified array
    /// </summary>
    /// <typeparam name="T">Type of element in the array</typeparam>
    /// <param name="array">The array to copy</param>
    /// <returns>The copy of <paramref name="array"/></returns>
    public static T[] Copy<T>(this T[] array)
    {
        T[] copy = new T[array.Length];
        array.CopyTo(copy, 0);
        return copy;
    }

    /// <inheritdoc cref="Array.Copy(Array, Array, int)"/>
    public static void CopyTo(this Array array, Array destination, int length) => Array.Copy(array, destination, length);

    /// <inheritdoc cref="Array.Exists{T}"/>
    public static bool Exists<T>(this T[] array, Predicate<T> predicate) => Array.Exists(array, predicate);

    /// <inheritdoc cref="Array.Fill{T}(T[], T)"/>
    public static void Fill<T>(this T[] array, T value) => Array.Fill(array, value);

    /// <summary>
    /// Fills the array with new values
    /// </summary>
    /// <typeparam name="T">Type of values in the array</typeparam>
    /// <param name="array">Array to fill</param>
    /// <param name="getValue">Value getter function</param>
    public static void Fill<T>(this T[] array, Func<T> getValue)
    {
        foreach (int i in ..array.Length)
        {
            array[i] = getValue();
        }
    }

    /// <inheritdoc cref="Array.Find{T}"/>
    public static T? Find<T>(this T[] array, Predicate<T> predicate) => Array.Find(array, predicate);

    /// <inheritdoc cref="Array.FindIndex{T}(T[], Predicate{T})"/>
    public static int FindIndex<T>(this T[] array, Predicate<T> predicate) => Array.FindIndex(array, predicate);

    /// <inheritdoc cref="Array.FindLast{T}"/>
    public static T? FindLast<T>(this T[] array, Predicate<T> predicate) => Array.FindLast(array, predicate);

    /// <inheritdoc cref="Array.FindLastIndex{T}(T[], Predicate{T})"/>
    public static int FindLastIndex<T>(this T[] array, Predicate<T> predicate) => Array.FindLastIndex(array, predicate);

    /// <inheritdoc cref="Array.ForEach{T}"/>
    public static void ForEach<T>(this T[] array, Action<T> action) => Array.ForEach(array, action);

    /// <inheritdoc cref="Array.IndexOf{T}(T[], T)"/>
    public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);

    /// <inheritdoc cref="Array.LastIndexOf{T}(T[], T)"/>
    public static int LastIndexOf<T>(this T[] array, T value) => Array.LastIndexOf(array, value);

    /// <inheritdoc cref="Array.Reverse{T}(T[])"/>
    public static void Reverse<T>(this T[] array) => Array.Reverse(array);

    /// <inheritdoc cref="Array.Sort{T}(T[])"/>
    public static void Sort<T>(this T[] array) => Array.Sort(array);

    /// <inheritdoc cref="Array.Sort{T}(T[], IComparer{T})"/>
    public static void Sort<T>(this T[] array, IComparer<T> comparer) => Array.Sort(array, comparer);

    /// <inheritdoc cref="Array.Sort{T}(T[], Comparison{T})"/>
    public static void Sort<T>(this T[] array, Comparison<T> comparison) => Array.Sort(array, comparison);

    /// <inheritdoc cref="Array.TrueForAll{T}"/>
    public static bool TrueForAll<T>(this T[] array, Predicate<T> predicate) => Array.TrueForAll(array, predicate);
    #endregion
}