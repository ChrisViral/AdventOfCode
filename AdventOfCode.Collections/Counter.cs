using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Utils.Extensions.Enumerables;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Counter class - counts how many instances of each value have been added to it
/// </summary>
/// <typeparam name="T">Type of value stored in the counter</typeparam>
[PublicAPI, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public sealed class Counter<T> : IDictionary<T, int>, IReadOnlyDictionary<T, int>, ICollection<T> where T : notnull
{
    /// <summary>Backing dictionary</summary>
    private readonly Dictionary<T, int> dictionary;

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Count"/>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Count;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Capacity"/>
    public int Capacity => this.dictionary.Capacity;

    /// <summary>
    /// The keys stored within this counter
    /// </summary>
    public Dictionary<T, int>.KeyCollection Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <summary>
    /// The count values stored within this counter
    /// </summary>
    public Dictionary<T, int>.ValueCollection Counts
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <summary>
    /// Gets the count for a given key in the dictionary. If the key is not present, 0 is returned
    /// </summary>
    /// <param name="key">Value to find in the Counter</param>
    /// <returns>The amount of that value stored in the counter, or 0 if none is</returns>
    public int this[T key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.GetValueOrDefault(key, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.dictionary[key] = value;
    }

    /// <summary>
    /// Creates a new Counter
    /// </summary>
    public Counter() => this.dictionary = new Dictionary<T, int>();

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    public Counter(IDictionary<T, int> source) => this.dictionary = new Dictionary<T, int>(source);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IDictionary<T, int> source, IEqualityComparer<T> comparer) => this.dictionary = new Dictionary<T, int>(source, comparer);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    public Counter(IEnumerable<KeyValuePair<T, int>> source) => this.dictionary = new Dictionary<T, int>(source);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEnumerable<KeyValuePair<T, int>> source, IEqualityComparer<T> comparer) => this.dictionary = new Dictionary<T, int>(source, comparer);

    /// <summary>
    /// Creates a new Counter with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    public Counter(int capacity) => this.dictionary = new Dictionary<T, int>(capacity);

    /// <summary>
    /// Creates a new Counter with a specific <see cref="EqualityComparer{T}"/>
    /// </summary>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEqualityComparer<T> comparer) => this.dictionary = new Dictionary<T, int>(comparer);

    /// <summary>
    /// Creates a new Counter with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(int capacity, IEqualityComparer<T> comparer) => this.dictionary = new Dictionary<T, int>(capacity, comparer);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    public Counter(IEnumerable<T> source) : this(source, EqualityComparer<T>.Default) { }

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEnumerable<T> source, IEqualityComparer<T> comparer)
    {
        this.dictionary = source.TryGetNonEnumeratedCount(out int count)
                              ? new Dictionary<T, int>(count, comparer)
                              : new Dictionary<T, int>(comparer);
        AddRange(source);
    }

    /// <summary>
    /// Adds a new value to the Counter
    /// </summary>
    /// <param name="value">Value to add</param>
    /// <returns><see langword="true"/> if the value was already in the counter and was incremented, otherwise <see langword="false"/></returns>
    public bool Add(T value)
    {
        if (this.dictionary.TryAdd(value, 1)) return true;

        this.dictionary[value]++;
        return false;

    }

    /// <summary>
    /// Adds a range of values to the Counter
    /// </summary>
    /// <param name="values">Values to add</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(IEnumerable<T> values) => values.ForEach(v => Add(v));

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.dictionary.Clear();

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array, int arrayIndex) => this.dictionary.Keys.CopyTo(array, arrayIndex);

    /// <inheritdoc cref="ICollection{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator() => this.dictionary.Keys.GetEnumerator();

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    public bool Remove(T value)
    {
        if (!this.dictionary.TryGetValue(value, out int count)) return false;
        if (count is 1) return this.dictionary.Remove(value);

        this.dictionary[value]--;
        return false;
    }

    /// <summary>
    /// Tries to get the item count for a given value
    /// </summary>
    /// <param name="value">Value to get the count for</param>
    /// <param name="count">Value count output parameter</param>
    /// <returns><see langword="true"/> if the value was in the Counter and the count was found, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCount(T value, out int count) => this.dictionary.TryGetValue(value, out count);

    /// <summary>
    /// Returns this counter as a dictionary specific implementation
    /// </summary>
    /// <returns>Dictionary implementation of the Counter</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDictionary<T, int> AsDictionary() => this;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    bool ICollection<KeyValuePair<T, int>>.IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Keys"/>
    ICollection<T> IDictionary<T, int>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Values"/>
    ICollection<int> IDictionary<T, int>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <inheritdoc ref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
    IEnumerable<T> IReadOnlyDictionary<T, int>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <inheritdoc ref="IReadOnlyDictionary{TKey, TValue}.Values"/>
    IEnumerable<int> IReadOnlyDictionary<T, int>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Values"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDictionary<T, int>.Add(T key, int value) => this.dictionary.Add(key, value);

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<T, int>>.Add(KeyValuePair<T, int> item)
    {
        ((ICollection<KeyValuePair<T, int>>)this.dictionary).Add(item);
    }

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T value) => Add(value);

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<T, int>>.Contains(KeyValuePair<T, int> item)
    {
        return ((ICollection<KeyValuePair<T, int>>)this.dictionary).Contains(item);
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IDictionary<T, int>.ContainsKey(T value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<T, int>>.CopyTo(KeyValuePair<T, int>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<T, int>>)this.dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<T, int>>.Remove(KeyValuePair<T, int> item)
    {
        return ((ICollection<KeyValuePair<T, int>>)this.dictionary).Remove(item);
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IReadOnlyDictionary<T, int>.ContainsKey(T value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IDictionary<T, int>.TryGetValue(T key, out int value) => this.dictionary.TryGetValue(key, out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IReadOnlyDictionary<T, int>.TryGetValue(T key, out int value) => this.dictionary.TryGetValue(key, out value);

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<KeyValuePair<T, int>> IEnumerable<KeyValuePair<T, int>>.GetEnumerator() => this.dictionary.GetEnumerator();
}
