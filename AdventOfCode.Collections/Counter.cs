using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Utils.Extensions.Enumerables;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Counter class - counts how many instances of each value have been added to it
/// </summary>
/// <typeparam name="T">Type of value stored in the counter</typeparam>
[PublicAPI]
public sealed class Counter<T> : Counter<T, int> where T : notnull
{
    /// <inheritdoc />
    public Counter() { }

    /// <inheritdoc />
    public Counter(IDictionary<T, int> source) : base(source) { }

    /// <inheritdoc />
    public Counter(IDictionary<T, int> source, IEqualityComparer<T> comparer) : base(source, comparer) { }

    /// <inheritdoc />
    public Counter(IEnumerable<KeyValuePair<T, int>> source) : base(source) { }

    /// <inheritdoc />
    public Counter(IEnumerable<KeyValuePair<T, int>> source, IEqualityComparer<T> comparer) : base(source, comparer) { }

    /// <inheritdoc />
    public Counter(int capacity) : base(capacity) { }

    /// <inheritdoc />
    public Counter(IEqualityComparer<T> comparer) : base(comparer) { }

    /// <inheritdoc />
    public Counter(int capacity, IEqualityComparer<T> comparer) : base(capacity, comparer) { }

    /// <inheritdoc />
    public Counter(IEnumerable<T> source) : base(source) { }

    /// <inheritdoc />
    public Counter(IEnumerable<T> source, IEqualityComparer<T> comparer) : base(source, comparer) { }
}

/// <summary>
/// Counter class - counts how many instances of each value have been added to it
/// </summary>
/// <typeparam name="TKey">Type of value stored in the counter</typeparam>
/// <typeparam name="TCount">Type integer stored in the counter</typeparam>
[PublicAPI, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public class Counter<TKey, TCount> : IDictionary<TKey, TCount>, IReadOnlyDictionary<TKey, TCount>, ICollection<TKey>
    where TKey : notnull
    where TCount : struct, IBinaryInteger<TCount>
{
    /// <summary>Backing dictionary</summary>
    private readonly Dictionary<TKey, TCount> dictionary;

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Count"/>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Count;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Capacity"/>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Capacity;
    }

    /// <summary>
    /// The keys stored within this counter
    /// </summary>
    public Dictionary<TKey, TCount>.KeyCollection Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <summary>
    /// The count values stored within this counter
    /// </summary>
    public Dictionary<TKey, TCount>.ValueCollection Counts
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <summary>
    /// Gets the count for a given key in the dictionary. If the key is not present, 0 is returned
    /// </summary>
    /// <param name="key">Value to find in the Counter</param>
    /// <returns>The amount of that value stored in the counter, or 0 if none is</returns>
    public TCount this[TKey key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.GetValueOrDefault(key, TCount.Zero);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.dictionary[key] = value;
    }

    /// <summary>
    /// Creates a new Counter
    /// </summary>
    public Counter() => this.dictionary = new Dictionary<TKey, TCount>();

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    public Counter(IDictionary<TKey, TCount> source) => this.dictionary = new Dictionary<TKey, TCount>(source);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IDictionary<TKey, TCount> source, IEqualityComparer<TKey> comparer) => this.dictionary = new Dictionary<TKey, TCount>(source, comparer);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    public Counter(IEnumerable<KeyValuePair<TKey, TCount>> source) => this.dictionary = new Dictionary<TKey, TCount>(source);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEnumerable<KeyValuePair<TKey, TCount>> source, IEqualityComparer<TKey> comparer) => this.dictionary = new Dictionary<TKey, TCount>(source, comparer);

    /// <summary>
    /// Creates a new Counter with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    public Counter(int capacity) => this.dictionary = new Dictionary<TKey, TCount>(capacity);

    /// <summary>
    /// Creates a new Counter with a specific <see cref="EqualityComparer{T}"/>
    /// </summary>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEqualityComparer<TKey> comparer) => this.dictionary = new Dictionary<TKey, TCount>(comparer);

    /// <summary>
    /// Creates a new Counter with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(int capacity, IEqualityComparer<TKey> comparer) => this.dictionary = new Dictionary<TKey, TCount>(capacity, comparer);

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    public Counter(IEnumerable<TKey> source) : this(source, EqualityComparer<TKey>.Default) { }

    /// <summary>
    /// Creates a new Counter from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    /// <param name="comparer">Match equality comparer</param>
    public Counter(IEnumerable<TKey> source, IEqualityComparer<TKey> comparer)
    {
        this.dictionary = source.TryGetNonEnumeratedCount(out int count)
                              ? new Dictionary<TKey, TCount>(count, comparer)
                              : new Dictionary<TKey, TCount>(comparer);
        AddRange(source);
    }

    /// <summary>
    /// Adds a new value to the Counter
    /// </summary>
    /// <param name="value">Value to add</param>
    /// <returns><see langword="true"/> if the value was already in the counter and was incremented, otherwise <see langword="false"/></returns>
    public bool Add(TKey value)
    {
        if (this.dictionary.TryAdd(value, TCount.One)) return true;

        this.dictionary[value]++;
        return false;

    }

    /// <summary>
    /// Adds a range of values to the Counter
    /// </summary>
    /// <param name="values">Values to add</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRange(IEnumerable<TKey> values) => values.ForEach(v => Add(v));

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.dictionary.Clear();

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(TKey value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(TKey[] array, int arrayIndex) => this.dictionary.Keys.CopyTo(array, arrayIndex);

    /// <inheritdoc cref="ICollection{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<TKey> GetEnumerator() => this.dictionary.Keys.GetEnumerator();

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    public bool Remove(TKey value)
    {
        if (!this.dictionary.TryGetValue(value, out TCount count)) return false;
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
    public bool TryGetCount(TKey value, out TCount count) => this.dictionary.TryGetValue(value, out count);

    /// <summary>
    /// Returns this counter as a dictionary specific implementation
    /// </summary>
    /// <returns>Dictionary implementation of the Counter</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDictionary<TKey, TCount> AsDictionary() => this;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    bool ICollection<KeyValuePair<TKey, TCount>>.IsReadOnly => false;

    /// <inheritdoc cref="ICollection{T}.IsReadOnly"/>
    bool ICollection<TKey>.IsReadOnly => false;

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Keys"/>
    ICollection<TKey> IDictionary<TKey, TCount>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Values"/>
    ICollection<TCount> IDictionary<TKey, TCount>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <inheritdoc ref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TCount>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <inheritdoc ref="IReadOnlyDictionary{TKey, TValue}.Values"/>
    IEnumerable<TCount> IReadOnlyDictionary<TKey, TCount>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Values"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void IDictionary<TKey, TCount>.Add(TKey key, TCount value) => this.dictionary.Add(key, value);

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TCount>>.Add(KeyValuePair<TKey, TCount> item)
    {
        ((ICollection<KeyValuePair<TKey, TCount>>)this.dictionary).Add(item);
    }

    /// <inheritdoc cref="ICollection{T}.Add"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<TKey>.Add(TKey value) => Add(value);

    /// <inheritdoc cref="ICollection{T}.Contains"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TCount>>.Contains(KeyValuePair<TKey, TCount> item)
    {
        return ((ICollection<KeyValuePair<TKey, TCount>>)this.dictionary).Contains(item);
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IDictionary<TKey, TCount>.ContainsKey(TKey value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="ICollection{T}.CopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TCount>>.CopyTo(KeyValuePair<TKey, TCount>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TCount>>)this.dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc cref="ICollection{T}.Remove"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TCount>>.Remove(KeyValuePair<TKey, TCount> item)
    {
        return ((ICollection<KeyValuePair<TKey, TCount>>)this.dictionary).Remove(item);
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IReadOnlyDictionary<TKey, TCount>.ContainsKey(TKey value) => this.dictionary.ContainsKey(value);

    /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IDictionary<TKey, TCount>.TryGetValue(TKey key, out TCount value) => this.dictionary.TryGetValue(key, out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IReadOnlyDictionary<TKey, TCount>.TryGetValue(TKey key, out TCount value) => this.dictionary.TryGetValue(key, out value);

    /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<KeyValuePair<TKey, TCount>> IEnumerable<KeyValuePair<TKey, TCount>>.GetEnumerator() => this.dictionary.GetEnumerator();
}
