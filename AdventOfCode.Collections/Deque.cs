using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Ring buffer backed dequeue which supports indexing
/// </summary>
/// <typeparam name="T">Dequeue element type</typeparam>
[PublicAPI, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public sealed class Deque<T> : IList<T>
{
    private const int DEFAULT_CAPACITY = 16;
    private const int CAPACITY_MULTIPLIER = 4;

    private readonly RingBuffer<T> buffer;

    /// <inheritdoc />
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.Count;
    }

    /// <summary>
    /// The current capacity of this Deque
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.Capacity;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.buffer.Capacity = value;
    }

    /// <inheritdoc />
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.buffer[index] = value;
    }

    /// <inheritdoc cref="Deque{T}.Item(int)" />
    public T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.buffer[index] = value;
    }

    /// <summary>
    /// Creates a new empty Deque
    /// </summary>
    public Deque() : this(DEFAULT_CAPACITY) { }

    /// <summary>
    /// Creates a new deque with the specified initial capacity
    /// </summary>
    /// <param name="capacity">Initial capacity</param>
    public Deque(int capacity) => this.buffer = new RingBuffer<T>(capacity);

    /// <summary>
    /// Creates a new Deque with the given items
    /// </summary>
    /// <param name="items">Items to put in the deque</param>
    public Deque(ICollection<T> items) : this(items, items.Count) { }

    /// <summary>
    /// Creates a new Deque with the given items and capacity
    /// </summary>
    /// <param name="items">Items to put in the deque</param>
    /// <param name="capacity">Initial capacity</param>
    public Deque(ICollection<T> items, int capacity) => this.buffer = new RingBuffer<T>(items, capacity);

    /// <inheritdoc cref="Deque{T}(ICollection{T})" />
    public Deque(ReadOnlySpan<T> items) : this(items, items.Length) { }

    /// <inheritdoc cref="Deque{T}(ICollection{T}, int)" />
    public Deque(ReadOnlySpan<T> items, int capacity) => this.buffer = new RingBuffer<T>(items, capacity);

    /// <inheritdoc cref="Deque{T}(ICollection{T}, int)" />
    public Deque(IEnumerable<T> items, int capacity) => this.buffer = new RingBuffer<T>(items, capacity);

    /// <summary>
    /// Enqueues a value to the back of deque
    /// </summary>
    /// <param name="item">Item to enque</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Enqueue(T item)
    {
        EnsureCapacity();
        this.buffer.Push(item);
    }

    /// <summary>
    /// Dequeues an item from the back of the deque
    /// </summary>
    /// <returns>The dequeued item</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Dequeue() => this.buffer.Pop();

    /// <summary>
    /// Returns the item at the back of the deque without removing it
    /// </summary>
    /// <returns>The item at the back of the deque</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Peek() => this.buffer.Peek();

    /// <summary>
    /// Tries to dequeue an item from the back of the deque
    /// </summary>
    /// <param name="value">Dequeued value, if any</param>
    /// <returns><see langword="true"/> if an item was dequeued, otherwise <see langword="false"/></returns>
    public bool TryDequeue([MaybeNullWhen(false)] out T value)
    {
        if (this.buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = this.buffer.Pop();
        return true;
    }

    /// <summary>
    /// Tries to peek the item at the back of the deque
    /// </summary>
    /// <param name="value">Value at the back of the deque, if any</param>
    /// <returns><see langword="true"/> if an item was returned, otherwise <see langword="false"/></returns>
    public bool TryPeek([MaybeNullWhen(false)] out T value)
    {
        if (this.buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = this.buffer.Peek();
        return true;
    }

    /// <summary>
    /// Enqueues a value to the front of deque
    /// </summary>
    /// <param name="item">Item to enque</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnqueueFront(T item)
    {
        EnsureCapacity();
        this.buffer.PushFront(item);
    }

    /// <summary>
    /// Dequeues an item from the front of the deque
    /// </summary>
    /// <returns>The dequeued item</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T DequeueFront() => this.buffer.PopFront();

    /// <summary>
    /// Returns the item at the front of the deque without removing it
    /// </summary>
    /// <returns>The item at the front of the deque</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T PeekFront() => this.buffer.PeekFront();

    /// <summary>
    /// Tries to dequeue an item from the front of the deque
    /// </summary>
    /// <param name="value">Dequeued value, if any</param>
    /// <returns><see langword="true"/> if an item was dequeued, otherwise <see langword="false"/></returns>
    public bool TryDequeueFront([MaybeNullWhen(false)] out T value)
    {
        if (this.buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = this.buffer.PopFront();
        return true;
    }

    /// <summary>
    /// Tries to peek the item at the front of the deque
    /// </summary>
    /// <param name="value">Value at the front of the deque, if any</param>
    /// <returns><see langword="true"/> if an item was returned, otherwise <see langword="false"/></returns>
    public bool TryPeekFront([MaybeNullWhen(false)] out T value)
    {
        if (this.buffer.IsEmpty)
        {
            value = default;
            return false;
        }

        value = this.buffer.PeekFront();
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf(T item) => this.buffer.IndexOf(item);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T item) => this.buffer.Contains(item);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Insert(int index, T item)
    {
        if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of buffer");

        EnsureCapacity();
        this.buffer.Insert(index, item);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(T item) => this.buffer.Remove(item);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveAt(int index) => this.buffer.RemoveAt(index);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array, int arrayIndex) => this.buffer.CopyTo(array, arrayIndex);

    /// <inheritdoc cref="RingBuffer{T}.CopyTo(Span{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Span<T> span) => this.buffer.CopyTo(span);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.buffer.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureCapacity()
    {
        if (this.buffer.IsFull)
        {
            this.buffer.Capacity *= CAPACITY_MULTIPLIER;
        }
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RingBuffer<T>.Enumerator GetEnumerator() => this.buffer.GetEnumerator();

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T item) => Enqueue(item);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
