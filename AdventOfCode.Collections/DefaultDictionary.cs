using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Dictionary which provides a default value when
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
[PublicAPI, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public sealed class DefaultDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly TValue defaultValue;
    private readonly Dictionary<TKey, TValue> dictionary;

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Count" />
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

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Keys" />
    public Dictionary<TKey, TValue>.KeyCollection Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Keys;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Values" />
    public Dictionary<TKey, TValue>.ValueCollection Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.Values;
    }

    /// <inheritdoc cref="Dictionary{TKey, TValue}.Item"/>
    public TValue this[TKey key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.dictionary.GetValueOrDefault(key, this.defaultValue);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.dictionary[key] = value;
    }

    /// <summary>
    /// Creates a new DefaultDictionary
    /// </summary>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>();
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(IDictionary<TKey, TValue> source, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(source);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(IDictionary<TKey, TValue> source, IEqualityComparer<TKey> comparer, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(source, comparer);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary from existing data
    /// </summary>
    /// <param name="source">Data enumerable</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(source);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary from existing data
    /// </summary>
    /// <param name="source">Data dictionary</param>
    /// <param name="comparer">Match equality comparer</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(source, comparer);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(int capacity, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(capacity);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary with a specific <see cref="EqualityComparer{T}"/>
    /// </summary>
    /// <param name="comparer">Match equality comparer</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(IEqualityComparer<TKey> comparer, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(comparer);
        this.defaultValue = defaultValue;
    }

    /// <summary>
    /// Creates a new DefaultDictionary with the given capacity
    /// </summary>
    /// <param name="capacity">Counter capacity</param>
    /// <param name="comparer">Match equality comparer</param>
    /// <param name="defaultValue">Default value emmited by the dictionary when no value exists</param>
    public DefaultDictionary(int capacity, IEqualityComparer<TKey> comparer, TValue defaultValue)
    {
        this.dictionary   = new Dictionary<TKey, TValue>(capacity, comparer);
        this.defaultValue = defaultValue;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value) => this.dictionary.Add(key, value);

    /// <inheritdoc cref="Dictionary{TKey, TValue}.ContainsKey" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key) => this.dictionary.ContainsKey(key);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(TKey key) => this.dictionary.Remove(key);

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryGetValue" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (this.dictionary.TryGetValue(key, out value!)) return true;

        value = this.defaultValue;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.dictionary.Clear();

    /// <inheritdoc cref="Dictionary{TKey, TValue}.GetEnumerator"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => this.dictionary.GetEnumerator();

    /// <inheritdoc />
    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Keys;
    }

    /// <inheritdoc />
    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Values;
    }

    /// <inheritdoc />
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Keys;
    }

    /// <inheritdoc />
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Values;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Add(item);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Contains(item);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)this.dictionary).Remove(item);
    }

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
