using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Linq;

[assembly: ZLinqDropInExternalExtension("AdventOfCode.Collections", "AdventOfCode.Collections.SpanList`1", "ZLinq.Linq.FromSpan`1", GenerateAsPublic = true)]

namespace AdventOfCode.Collections;

/// <summary>
/// Non-resizable, Span-backed list
/// </summary>
/// <param name="span">Span instance</param>
/// <typeparam name="T">Type of value stored within the list</typeparam>
[PublicAPI, DebuggerDisplay("Count: {Count}, Capacity: {Capacity}"), DebuggerTypeProxy(typeof(SpanListDebugView<>))]
public ref struct SpanList<T>(Span<T> span)
{
    private readonly Span<T> span = span;
    private int version = 0;

    /// <summary>
    /// Item count in the list
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// List maximum capacity
    /// </summary>
    public int Capacity => this.span.Length;

    /// <summary>
    /// Span over the current elements in the list
    /// </summary>
    public Span<T> AsSpan => this.span[..this.Count];

    /// <summary>
    /// List indexer
    /// </summary>
    /// <param name="index">Index to get or set</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), "Index must be within list range");
            return this.span[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), "Index must be within list range");
            this.span[index] = value;
            this.version++;
        }
    }

    /// <summary>
    /// List indexer
    /// </summary>
    /// <param name="index">Index to get or set</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    public T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index.GetOffset(this.Count)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[index.GetOffset(this.Count)] = value;
    }

    /// <summary>
    /// List range indexer
    /// </summary>
    /// <param name="range">Range to get or set</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="range"/> is outside of range of the list</exception>
    public Span<T> this[Range range]
    {
        get
        {
            (int offset, int length) = range.GetOffsetAndLength(this.Count);
            if (offset < 0 || offset + length >= this.Count) throw new ArgumentOutOfRangeException(nameof(range), "Range includes parts outside of bounds of the list");
            return this.span[range];
        }
        set
        {
            (int offset, int length) = range.GetOffsetAndLength(this.Count);
            if (offset < 0 || offset + length >= this.Count) throw new ArgumentOutOfRangeException(nameof(range), "Range includes parts outside of bounds of the list");
            if (length < value.Length) throw new ArgumentOutOfRangeException(nameof(range), "Range too small to accomodate incoming value");
            value.CopyTo(this.span[range]);
            this.version++;
        }
    }

    /// <summary>
    /// Non-resizable, Span-backed list
    /// </summary>
    /// <param name="span">Span instance</param>
    /// <param name="count">Item count already in the list</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="count"/> is outside of the range of the list</exception>
    public SpanList(Span<T> span, int count) : this(span)
    {
        if (count > span.Length) throw new ArgumentOutOfRangeException(nameof(count), "SpanList initial count must be lower or equal to backing span length");
        this.Count = count;
    }

    /// <summary>
    /// Adds a new item to the list
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <exception cref="InvalidOperationException">If the list is full</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        if (this.Count == this.Capacity) throw new InvalidOperationException("SpanList already at maximum capacity, cannot add another item");
        this.span[this.Count++] = item;
        this.version++;
    }

    /// <summary>
    /// Adds a range of values to the list
    /// </summary>
    /// <param name="values">Values to add</param>
    /// <exception cref="InvalidOperationException">If adding these items causes an overflow of the list</exception>
    public void AddRange(IEnumerable<T> values)
    {
        if (values.TryGetNonEnumeratedCount(out int count) && this.Count + count > this.Capacity)
        {
            throw new InvalidOperationException("Adding this range of items would cause an overflow of the list");
        }

        foreach (T value in values)
        {
            Add(value);
        }
    }

    /// <summary>
    /// Adds a range of values to the list
    /// </summary>
    /// <param name="values">Values to add</param>
    /// <exception cref="InvalidOperationException">If adding these items causes an overflow of the list</exception>
    public void AddRange(ReadOnlySpan<T> values)
    {
        if (this.Count + values.Length > this.Capacity)
        {
            throw new InvalidOperationException("Adding this range of items would cause an overflow of the list");
        }

        values.CopyTo(this.span.Slice(this.Count, values.Length));
        this.Count += values.Length;
        this.version++;
    }

    /// <summary>
    /// Inserts an item at the specified index in the list
    /// </summary>
    /// <param name="index">Index to insert at</param>
    /// <param name="item">Item to insert</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    /// <exception cref="InvalidOperationException">If the list is full</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException(nameof(index), "Index must be within list range");
        if (index == this.Count)
        {
            Add(item);
            return;
        }

        if (this.Count == this.Capacity) throw new InvalidOperationException("SpanList already at maximum capacity, cannot add another item");

        this.span[index..this.Count].CopyTo(this.span[(index + 1)..(this.Count + 1)]);
        this.span[index] = item;
        this.Count++;
        this.version++;
    }

    /// <summary>
    /// Inserts a range of values to the list
    /// </summary>
    /// <param name="index">Index at which to insert the values</param>
    /// <param name="values">Values to insert</param>
    /// <exception cref="InvalidOperationException">If adding these items causes an overflow of the list</exception>
    public void InsertRange(int index, IEnumerable<T> values)
    {
        if (index == this.Count)
        {
            AddRange(values);
            return;
        }

        if (values.TryGetNonEnumeratedCount(out int count) && this.Count + count > this.Capacity)
        {
            throw new InvalidOperationException("Inserting this range of items would cause an overflow of the list");
        }

        foreach (T value in values)
        {
            Insert(index++, value);
        }
    }

    /// <summary>
    /// Inserts a range of values to the list
    /// </summary>
    /// <param name="index">Index at which to insert the values</param>
    /// <param name="values">Values to insert</param>
    /// <exception cref="InvalidOperationException">If inserting these items causes an overflow of the list</exception>
    public void InsertRange(int index, ReadOnlySpan<T> values)
    {
        if (index == this.Count)
        {
            AddRange(values);
            return;
        }

        if (this.Count + values.Length > this.Capacity)
        {
            throw new InvalidOperationException("Adding this range of items would cause an overflow of the list");
        }

        this.span[index..this.Count].CopyTo(this.span.Slice(index + values.Length, this.Count - index));
        values.CopyTo(this.span.Slice(index, values.Length));
        this.Count += values.Length;
        this.version++;
    }

    /// <summary>
    /// Removes the given item from the list if it's present
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns><see langword="true"/> if the item was found and removed, otherwise <see langword="false"/></returns>
    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index is not -1)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the item at the given index in the list
    /// </summary>
    /// <param name="index">Index to remove at</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), "Index must be within list range");

        if (index < this.Count - 1)
        {
            this.span[(index + 1)..this.Count].CopyTo(this.span[index..this.Count]);
        }

        this.Count--;
        this.version++;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            this.span[this.Count] = default!;
        }
    }

    /// <summary>
    /// Removes an element from the list by swapping the last element of the list in it's spot, and then removing the last element.<br/>
    /// This should technically run in O(1)
    /// </summary>
    /// <param name="item">Item to remove</param>
    public bool RemoveSwap(T item)
    {
        int index = IndexOf(item);
        if (index is -1) return false;

        int lastIndex = this.Count - 1;
        if (index != lastIndex)
        {
            // Move the last element to the element to remove's spot
            this.span[index] = this.span[lastIndex];
        }

        // Remove last element
        RemoveAt(lastIndex);
        return true;
    }

    /// <summary>
    /// Removes an element from the list by swapping the last element of the list in it's spot, and then removing the last element.<br/>
    /// This should technically run in O(1)
    /// </summary>
    /// <param name="index">Index to remove at</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is out of the range of the list</exception>
    public void RemoveSwap(int index)
    {
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of list");

        int lastIndex = this.Count - 1;
        if (index != lastIndex)
        {
            // Move the last element to the element to remove's spot
            this.span[index] = this.span[lastIndex];
        }

        // Remove last element
        RemoveAt(lastIndex);
    }

    /// <summary>
    /// Removes all items matching the given predicate from the list
    /// </summary>
    /// <param name="predicate">The match predicate</param>
    /// <returns>The amount of items removed from the list</returns>
    public int RemoveAll([InstantHandle] Predicate<T> predicate)
    {
        int removed = 0;
        for (int i = this.Count - 1; i >= 0; i--)
        {
            if (predicate(this.span[i]))
            {
                RemoveAt(i);
                removed++;
            }
        }

        return removed;
    }

    /// <summary>
    /// Removes a range of items from the list
    /// </summary>
    /// <param name="range">Range to remove</param>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="range"/> points to indexes outside of the list</exception>
    public void RemoveRange(Range range)
    {
        (int start, int length) = range.GetOffsetAndLength(this.Count);
        RemoveRange(start, length);
    }

    /// <summary>
    /// Removes a range of items from the list
    /// </summary>
    /// <param name="start">Start of the range to remove</param>
    /// <param name="length">Amount of items to remove</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="start"/> is less than zero, or <paramref name="length"/> reaching past the end of the list</exception>
    public void RemoveRange(int start, int length)
    {
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), "Start value must be greater than zero");
        if (start + length - 1 >= this.Count) throw new ArgumentOutOfRangeException(nameof(length), "Length must stay within list range");

        if (start + length < this.Count)
        {
            this.span[(start + length)..this.Count].CopyTo(this.span.Slice(start, this.Count - (start + length)));
        }

        this.Count -= length;
        this.version++;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            this.span.Slice(start, length).Clear();
        }
    }

    /// <summary>
    /// Gets a ref to a given value in the list
    /// </summary>
    /// <param name="index">Index to get the value at</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef(int index)
    {
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), "Index must be within list range");
        return ref this.span[index];
    }

    /// <summary>
    /// Gets a ref to a given value in the list
    /// </summary>
    /// <param name="index">Index to get the value at</param>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is outside of range of the list</exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef(Index index) => ref GetRef(index.GetOffset(this.Count));

    /// <summary>
    /// Slices this list with a given range
    /// </summary>
    /// <param name="range">Range to slice</param>
    /// <returns>A new list over the specified slice</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="range"/> points to indexes outside of the list</exception>
    public SpanList<T> Slice(Range range)
    {
        (int start, int length) = range.GetOffsetAndLength(this.Count);
        return Slice(start, length);
    }

    /// <summary>
    /// Slices this list with a start point and length
    /// </summary>
    /// <param name="start">Slice start index</param>
    /// <param name="length">Slice length</param>
    /// <returns>A new list over the specified slice</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="start"/> is less than zero, or <paramref name="length"/> reaching past the end of the list</exception>
    public SpanList<T> Slice(int start, int length)
    {
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), "Start value must be greater than zero");
        if (start + length >= this.Count) throw new ArgumentOutOfRangeException(nameof(length), "Length must stay within list range");

        return new SpanList<T>(this.span[start..], length);
    }

    /// <summary>
    /// If the list contains the specified item
    /// </summary>
    /// <param name="item">Item to find in the list</param>
    /// <returns><see langword="true"/> if the item is in the list, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T item) => this.AsSpan.Contains(item);

    /// <summary>
    /// Gets the index of the specified item in the list
    /// </summary>
    /// <param name="item">Item to find in the list</param>
    /// <returns>The index of the item in the list if it was found, otherwise <c>-1</c></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(T item) => this.AsSpan.IndexOf(item);

    /// <summary>
    /// Gets the last index of the specified item in the list
    /// </summary>
    /// <param name="item">Item to find in the list</param>
    /// <returns>The index of the item in the list if it was found, otherwise <c>-1</c></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int LastIndexOf(T item) => this.AsSpan.LastIndexOf(item);

    /// <summary>
    /// Finds the index of the first value matching the given predicate
    /// </summary>
    /// <param name="predicate">Match predicate</param>
    /// <returns>The index of the first value matching <paramref name="predicate"/>, otherwise <c>-1</c></returns>
    public int FindIndex([InstantHandle] Predicate<T> predicate)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (predicate(this.span[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Finds the index of the last value matching the given predicate
    /// </summary>
    /// <param name="predicate">Match predicate</param>
    /// <returns>The index of the last value matching <paramref name="predicate"/>, otherwise <c>-1</c></returns>
    public int FindLastIndex([InstantHandle] Predicate<T> predicate)
    {
        for (int i = this.Count - 1; i >= 0; i--)
        {
            if (predicate(this.span[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <inheritdoc cref="System.MemoryExtensions.BinarySearch{T,TComparable}(System.Span{T},TComparable)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BinarySearch<TComparable>(TComparable value) where TComparable : IComparable<T>, allows ref struct
    {
        return this.AsSpan.BinarySearch(value);
    }

    /// <inheritdoc cref="System.MemoryExtensions.BinarySearch{T,TComparer}(System.Span{T},T,TComparer)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BinarySearch<TComparer>(T value, TComparer comparer) where TComparer : IComparer<T>, allows ref struct
    {
        return this.AsSpan.BinarySearch(value, comparer);
    }

    /// <inheritdoc cref="Span{T}.CopyTo" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> destination) => this.AsSpan.CopyTo(destination);

    /// <summary>
    /// Sorts the elements in this list
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort()
    {
        this.AsSpan.Sort();
        this.version++;
    }

    /// <summary>
    /// Sorts the elements in this list using a comparison function
    /// </summary>
    /// <param name="comparison">Comparison function</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort([InstantHandle] Comparison<T> comparison)
    {
        this.AsSpan.Sort(comparison);
        this.version++;
    }

    /// <summary>
    /// Sorts the elements in this list using a comparer
    /// </summary>
    /// <param name="comparer">Comparer instance</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Sort<TComparer>(TComparer comparer) where TComparer : IComparer<T>
    {
        this.AsSpan.Sort(comparer);
        this.version++;
    }

    /// <inheritdoc cref="System.MemoryExtensions.Reverse{T}(System.Span{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reverse()
    {
        this.AsSpan.Reverse();
        this.version++;
    }

    /// <inheritdoc cref="Span{T}.Clear" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        this.Count = 0;
        this.span.Clear();
        this.version++;
    }

    /// <summary>
    /// Applies an action to each element of this list
    /// </summary>
    /// <param name="action">Action to execute on each element</param>
    public void ForEach([InstantHandle] Action<T> action)
    {
        int check = this.version;
        for (int i = 0; i < this.Count; i++)
        {
            if (check != this.version) throw new InvalidOperationException("List modified during enumeration");
            action(this.span[i]);
        }
    }

    /// <summary>
    /// Converts this list into a ValueEnumerable
    /// </summary>
    /// <returns>ValueEnumerable wrapper over this list</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueEnumerable<FromSpan<T>, T> AsValueEnumerable() => this.AsSpan.AsValueEnumerable();

    /// <summary>
    /// Enumerates the contents of this list
    /// </summary>
    /// <returns>Enumerator over this list</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanListEnumerator GetEnumerator() => new(this);

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">Always thrown by this method</exception>
    [Obsolete("GetHashCode() on Span will always throw an exception."), EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
    public override int GetHashCode() => throw new NotSupportedException("GetHashCode() on Span will always throw an exception.");
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// Creates a new SpanList from the given span
    /// </summary>
    /// <param name="span">Span to create the list over</param>
    /// <returns>The created list for the span</returns>
    public static implicit operator SpanList<T>(Span<T> span) => new(span);

    /// <summary>
    /// SpanList enumerator
    /// </summary>
    /// <param name="list">List to enumerate over</param>
    public ref struct SpanListEnumerator(SpanList<T> list)
    {
        private Span<T>.Enumerator enumerator = list.AsSpan.GetEnumerator();
        private readonly SpanList<T> list = list;
        private readonly int version = list.version;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        /// <exception cref="InvalidOperationException">If the list has been modified during execution</exception>
        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (this.version != this.list.version) throw new InvalidOperationException("List modified during enumeration");
                return this.enumerator.Current;
            }
        }

        /// <summary>
        /// Moves the enumerator to the next element
        /// </summary>
        /// <returns><see langword="true"/> when there is another element, otherwise <see langword="false"/></returns>
        /// <exception cref="InvalidOperationException">If the list has been modified during execution</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining), UsedImplicitly]
        public bool MoveNext()
        {
            if (this.version != this.list.version) throw new InvalidOperationException("List modified during enumeration");
            return this.enumerator.MoveNext();
        }
    }
}
