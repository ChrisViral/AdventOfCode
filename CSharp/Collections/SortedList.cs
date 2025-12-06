using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Sorted list where the value is it's own key
/// </summary>
/// <typeparam name="T">List element type</typeparam>
[PublicAPI]
public sealed class SortedList<T> : ICollection<T>, IReadOnlyCollection<T> where T : notnull
{
    private readonly SortedList<T, T> list;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    public int Count => this.list.Count;

    /// <inheritdoc cref="ICollection{T}.Count"/>
    int IReadOnlyCollection<T>.Count => this.list.Count;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Gets the value at the given index in the list
    /// </summary>
    /// <param name="index">Index to get at</param>
    /// <returns>The value at the given index</returns>
    public T this[int index] => this.list.Keys[index];

    /// <summary>
    /// Gets the value at the given index in the list
    /// </summary>
    /// <param name="index">Index to get at</param>
    /// <returns>The value at the given index</returns>
    public T this[Index index] => this.list.Keys[index];

    /// <summary>
    /// Gets a slice from the <see cref="SortedList{T}"/>
    /// </summary>
    /// <param name="range">Range to get the values from</param>
    /// <returns>An enumerable of the values in the given range</returns>
    public IEnumerable<T> this[Range range] => range.AsEnumerable().Select(index => this.list.Keys[index]);

    /// <inheritdoc cref="SortedList{TKey,TValue}()"/>
    public SortedList() => this.list = new SortedList<T, T>();

    /// <inheritdoc cref="SortedList{TKey,TValue}(IComparer{TKey})"/>
    public SortedList(IComparer<T>? comparer) => this.list = new SortedList<T, T>(comparer);

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedList{T}"/> class that contains elements copied from
    /// the specific <see cref="IEnumerable{T}"/>, has sufficient capacity to accomodate the number of elements copied,
    /// and uses the default <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="range">Range of elements to add to the list</param>
    public SortedList(IEnumerable<T> range) => this.list = new SortedList<T, T>(range.ToDictionary(key => key, _ => default(T)!));

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedList{T}"/> class that contains elements copied from
    /// the specific <see cref="IEnumerable{T}"/>, has sufficient capacity to accomodate the number of elements copied,
    /// and uses the specified <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="range">Range of elements to add to the list</param>
    /// <param name="comparer">Element comparer to sort the list</param>
    public SortedList(IEnumerable<T> range, IComparer<T>? comparer) => this.list = new SortedList<T, T>(range.ToDictionary(key => key, _ => default(T)!), comparer);

    /// <inheritdoc cref="SortedList{TKey,TValue}(int)"/>
    public SortedList(int capacity) => this.list = new SortedList<T, T>(capacity);

    /// <inheritdoc cref="SortedList{TKey,TValue}(int, IComparer{TKey})"/>
    public SortedList(int capacity, IComparer<T>? comparer) => this.list = new SortedList<T, T>(capacity, comparer);

    /// <summary>
    /// Adds the given value to the sorted list
    /// </summary>
    /// <param name="value">Value to add</param>
    public void Add(T value) => this.list.Add(value, default!);

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    public void Clear() => this.list.Clear();

    /// <summary>
    /// Checks if the value is present within the list
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>True if the value is in the list, false otherwise</returns>
    public bool Contains(T value) => list.ContainsKey(value);

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    public void CopyTo(T[] array, int arrayIndex) => this.list.Keys.CopyTo(array, arrayIndex);

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    public bool Remove(T item) => this.list.Remove(item);

    /// <summary>
    /// Index of the value in the list
    /// </summary>
    /// <param name="value">Value to get the index for</param>
    /// <returns>The index of the value, or -1 if the value is not within the list</returns>
    public int IndexOf(T value) => this.list.IndexOfKey(value);

    /// <summary>
    /// Iterates over the values of the list, in sorted order
    /// </summary>
    /// <returns>An enumerator over the sorted list</returns>
    public IEnumerator<T> GetEnumerator() => this.list.Keys.GetEnumerator();

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
