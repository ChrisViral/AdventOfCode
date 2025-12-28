using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode.Collections.DebugViews;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Collections;

/// <summary>
/// Fixed-capacity ring buffer
/// </summary>
/// <typeparam name="T">Element type within the buffer</typeparam>
[PublicAPI, DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public sealed class RingBuffer<T> : IList<T>
{
    private T[] buffer;
    private int start;
    private int end;
    private int version;

    /// <summary>
    /// Empty <see cref="RingBuffer{T}"/>
    /// </summary>
    public static RingBuffer<T> Empty { get; } = [];

    /// <inheritdoc />
    public int Count { get; private set; }

    /// <summary>
    /// Buffer capacity, setting it resizes the buffer
    /// </summary>
    /// <exception cref="InvalidOperationException">If the new set capacity is less than the currently held elements</exception>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.Length;
        set
        {
            // Ignore if no change
            if (this.buffer.Length == value) return;

            // Make sure the new value isn't less than the current count
            if (this.Count > value) throw new InvalidOperationException("Cannot shrink Deque smaller than it's currently held items");

            if (this.Count is 0)
            {
                // If empty, resize the buffer and do nothing else
                this.buffer = new T[value];
            }
            else if (this.IsSingleSegment && this.end <= value)
            {
                // For single segments which aren't going to be nuked, we can resize the array without touching the data
                Array.Resize(ref this.buffer, value);
                this.end = (this.start + this.Count) % value;
            }
            else
            {
                // Copy to new buffer
                T[] newBuffer = new T[value];
                CopyTo(newBuffer);
                this.buffer = newBuffer;
                this.start = 0;
                // Put the end index at the front if the buffer is now full
                this.end = this.Count != value ? this.Count : 0;
            }

            // Increment version
            this.version++;
        }
    }

    /// <summary>
    /// Checks if the buffer is currently empty
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Count is 0;
    }

    /// <summary>
    /// Checks if the buffer is currently full
    /// </summary>
    public bool IsFull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Count == this.Capacity;
    }

    /// <summary>
    /// If the buffer currently occupies one single continuous segment
    /// </summary>
    private bool IsSingleSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.end is 0 || this.start < this.end;
    }

    /// <summary>
    /// First segment length when in split segmenets
    /// </summary>
    private int FirstSegmentLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Capacity - this.start;
    }

    /// <summary>
    /// Second segment length when in split segments
    /// </summary>
    /// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    private int SecondSegmentLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.end;
    }

    /// <summary>
    /// First split segment
    /// </summary>
    private ReadOnlySpan<T> FirstSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.AsSpan(this.start);
    }

    /// <summary>
    /// Second split segment
    /// </summary>
    private ReadOnlySpan<T> SecondSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.AsSpan(0, this.end);
    }

    /// <summary>
    /// Singular continuous segment
    /// </summary>
    private ReadOnlySpan<T> SingleSegment
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.buffer.AsSpan(this.start, this.Count);
    }

    /// <inheritdoc />
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of buffer");
            return this.buffer[NormalizeIndexIn(index)];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of buffer");
            this.buffer[NormalizeIndexIn(index)] = value;
            this.version++;
        }
    }

    /// <inheritdoc cref="RingBuffer{T}.Item(int)" />
    public T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index.GetOffset(this.Count)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[index.GetOffset(this.Count)] = value;
    }

    /// <summary>
    /// Creates a new empty RingBuffer
    /// </summary>
    private RingBuffer() => this.buffer = [];

    /// <summary>
    /// Creates a new RingBuffer with the specified capacity
    /// </summary>
    /// <param name="capacity">Buffer capacity</param>
    /// <exception cref="ArgumentOutOfRangeException">If the capacity is less than one</exception>
    public RingBuffer(int capacity)
    {
        if (capacity < 1) throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity cannot be less than one");
        this.buffer = new T[capacity];
    }

    /// <summary>
    /// Creates a new RingBuffer with the specified capacity and items
    /// </summary>
    /// <param name="items">Buffer items</param>
    /// <param name="capacity">Buffer capacity</param>
    /// <exception cref="ArgumentOutOfRangeException">If the capacity is less than one</exception>
    /// <exception cref="ArgumentException">If there are more items than capacity in the buffer</exception>
    public RingBuffer(ICollection<T> items, int capacity) : this(capacity)
    {
        if (items.Count > capacity) throw new ArgumentException("Items size cannot be greater than buffer capacity", nameof(items));

        items.CopyTo(this.buffer, 0);
        this.Count = items.Count;
    }

    /// <inheritdoc cref="RingBuffer{T}(ICollection{T}, int)" />
    public RingBuffer(ReadOnlySpan<T> items, int capacity) : this(capacity)
    {
        if (items.Length > capacity) throw new ArgumentException("Items size cannot be greater than buffer capacity", nameof(items));

        items.CopyTo(this.buffer);
        this.Count = items.Length;
    }

    /// <inheritdoc cref="RingBuffer{T}(ICollection{T}, int)" />
    public RingBuffer(IEnumerable<T> items, int capacity) : this(capacity)
    {
        if (items.TryGetNonEnumeratedCount(out int count) && count > capacity) throw new ArgumentException("Items size cannot be greater than buffer capacity", nameof(items));

        foreach (T item in items)
        {
            if (this.IsFull) throw new ArgumentException("Items size cannot be greater than buffer capacity", nameof(items));
            this.buffer[this.Count++] = item;
        }

        this.end = !this.IsFull ? this.Count : 0;
    }

    /// <summary>
    /// Pushes an item to the back of the buffer
    /// </summary>
    /// <remarks>Pushing an item to the back of the buffer when it's full will remove the item at the front of the buffer</remarks>
    /// <param name="item">Item to push to the back</param>
    public T? Push(T item)
    {
        this.version++;
        if (this.IsFull)
        {
            ref T top = ref this.buffer[this.end];
            T removed = top;
            top = item;
            this.end = this.start = NextIndex(this.start);
            return removed;
        }

        this.buffer[this.end] = item;
        this.end = NextIndex(this.end);
        this.Count++;
        return default;
    }

    /// <summary>
    /// Pops and returns an item from the back of the buffer
    /// </summary>
    /// <returns>Item that was removed from the back of buffer</returns>
    /// <exception cref="InvalidOperationException">If the buffer was empty</exception>
    public T Pop()
    {
        if (this.IsEmpty) throw new InvalidOperationException("Buffer is empty, nothing to pop");

        this.end = PreviousIndex(this.end);
        ref T element = ref this.buffer[this.end];
        T item = element;
        element = default!;
        this.Count--;
        this.version++;

        if (this.IsEmpty)
        {
            this.start = 0;
            this.end = 0;
        }
        return item;
    }

    /// <summary>
    /// Returns the item at the back of the buffer without removing it
    /// </summary>
    /// <returns>Item at the back of the buffer</returns>
    /// <exception cref="InvalidOperationException">If the buffer was empty</exception>
    public T Peek() => !this.IsEmpty ? this.buffer[PreviousIndex(this.end)] : throw new InvalidOperationException("Buffer is empty, nothing to pop");

    /// <summary>
    /// Pushes an item to the front of the buffer
    /// </summary>
    /// <remarks>Pushing an item to the front of the buffer when it's full will remove the item at the back of the buffer</remarks>
    /// <param name="item">Item to push to the front</param>
    public T? PushFront(T item)
    {
        if (this.IsEmpty)
        {
            return Push(item);
        }

        this.version++;
        if (this.IsFull)
        {
            this.start = this.end = PreviousIndex(this.end);
            ref T bottom = ref this.buffer[this.start];
            T removed = bottom;
            bottom = item;
            return removed;
        }

        this.start = PreviousIndex(this.start);
        this.buffer[this.start] = item;
        this.Count++;
        return default;
    }

    /// <summary>
    /// Pops and returns an item from the front of the buffer
    /// </summary>
    /// <returns>Item that was removed from the front of buffer</returns>
    /// <exception cref="InvalidOperationException">If the buffer was empty</exception>
    public T PopFront()
    {
        if (this.IsEmpty) throw new InvalidOperationException("Buffer is empty, nothing to pop");

        ref T element = ref this.buffer[this.start];
        T item = element;
        element = default!;
        this.Count--;
        this.version++;

        if (this.IsEmpty)
        {
            this.start = 0;
            this.end = 0;
        }
        else
        {
            this.start = NextIndex(this.start);
        }
        return item;
    }

    /// <summary>
    /// Returns the item at the front of the buffer without removing it
    /// </summary>
    /// <returns>Item at the front of the buffer</returns>
    /// <exception cref="InvalidOperationException">If the buffer was empty</exception>
    public T PeekFront() => !this.IsEmpty ? this.buffer[this.start] : throw new InvalidOperationException("Buffer is empty, nothing to pop");

    /// <inheritdoc />
    public int IndexOf(T item)
    {
        if (this.IsEmpty) return -1;

        if (this.IsSingleSegment)
        {
            return this.SingleSegment.IndexOf(item);
        }

        int index = this.FirstSegment.IndexOf(item);
        if (index is not -1) return index;

        index = this.SecondSegment.IndexOf(item);
        return index is not -1 ? index + this.FirstSegmentLength : -1;
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        if (this.IsEmpty) return false;

        if (this.IsSingleSegment)
        {
            return this.SingleSegment.Contains(item);
        }

        return this.FirstSegment.Contains(item)
            || this.SecondSegment.Contains(item);
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        // Check bounds
        if (index < 0 || index > this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of buffer");

        // Check trivial cases (inserting at back/front)
        if (index == this.Count)
        {
            Push(item);
            return;
        }
        if (index is 0)
        {
            PushFront(item);
            return;
        }

        // Get normalized index and last element
        index = NormalizeIndexIn(index);
        int currentIndex = this.IsFull ? PreviousIndex(this.end) : this.end;
        ref T current = ref this.buffer[currentIndex];
        for ( ; currentIndex != index; currentIndex = PreviousIndex(currentIndex))
        {
            // Nudge elements back
            ref T previous = ref currentIndex is not 0 ? ref Unsafe.Subtract(ref current, 1) : ref this.buffer[^1];
            current = previous;
            current = ref previous;
        }

        // Set item
        current = item;
        this.version++;

        if (!this.IsFull)
        {
            // Else only move end index and increment count
            this.end = NextIndex(this.end);
            this.Count++;
        }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        if (this.IsEmpty) return false;

        int index = IndexOf(item);
        if (index is -1) return false;

        RemoveAt(index);
        return true;
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        if (this.IsEmpty) throw new InvalidOperationException("Buffer is empty, nothing to remove");
        if (index < 0 || index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be within bounds of buffer");

        if (index == this.Count - 1)
        {
            Pop();
            return;
        }
        if (index is 0)
        {
            PopFront();
            return;
        }

        // Get normalized index and element to remove
        index = NormalizeIndexIn(index);
        ref T current = ref this.buffer[index];
        for (index = NextIndex(index); index != this.end; index = NextIndex(index))
        {
            // Push elements forward
            ref T next = ref index is not 0 ? ref Unsafe.Add(ref current, 1) : ref this.buffer[0];
            current = next;
            current = ref next;
        }

        // Clear final element and decrement count
        current = default!;
        this.end = PreviousIndex(this.end);
        this.Count--;
        this.version++;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (this.IsEmpty) return;

        CopyTo(array.AsSpan(arrayIndex));
    }

    /// <summary>
    /// Copies the data from this buffer to a contiguous span
    /// </summary>
    /// <param name="span">Span to copy the buffer to</param>
    public void CopyTo(Span<T> span)
    {
        if (this.IsEmpty) return;

        if (this.IsSingleSegment)
        {
            // Single segment copy
            this.SingleSegment.CopyTo(span);
        }
        else
        {
            // Double segment copy
            this.FirstSegment.CopyTo(span);
            this.SecondSegment.CopyTo(span[this.FirstSegmentLength..]);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        // Clear everything
        this.buffer.Clear();
        this.start   = 0;
        this.end     = 0;
        this.Count   = 0;
        this.version++;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(this);

    /// <summary>
    /// Gets the next index in the buffer
    /// </summary>
    /// <param name="index">current index</param>
    /// <returns>The next index in the buffer</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NextIndex(int index) => (index + 1) % this.Capacity;

    /// <summary>
    /// Gets the previous index in the buffer
    /// </summary>
    /// <param name="index">current index</param>
    /// <returns>The previous index in the buffer</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PreviousIndex(int index) => (index + this.Capacity - 1) % this.Capacity;

    /// <summary>
    /// Normalizes an index from [0, count[ space to [start, end[ space
    /// </summary>
    /// <param name="index">Index to normalize</param>
    /// <returns>The index in buffer space</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NormalizeIndexIn(int index) => (this.start + index) % this.Capacity;

    /// <summary>
    /// Normalizes an index from [start, end[ space to [0, count[ space
    /// </summary>
    /// <param name="index">Index to normalize</param>
    /// <returns>The index in normal space</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NormalizeIndexOut(int index) => (index - this.start).Mod(this.Capacity);

    /// <inheritdoc />
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ICollection<T>.Add(T item) => Push(item);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Ring buffer enumerator
    /// </summary>
    /// <param name="ringBuffer">Buffer to enumerate</param>
    public struct Enumerator(RingBuffer<T> ringBuffer) : IEnumerator<T>
    {
        private readonly RingBuffer<T> ringBuffer = ringBuffer;
        private readonly int version = ringBuffer.version;
        private int index = ringBuffer.start;
        private bool hasReachedEnd = ringBuffer.IsEmpty;

        /// <inheritdoc />
        public T Current { get; private set; }

        /// <inheritdoc />
        public bool MoveNext()
        {
            // Ensure no modification is made during enumeration
            if (this.ringBuffer.version != this.version) throw new InvalidOperationException("Ring buffer was modified during enumeration");

            if (this.hasReachedEnd)
            {
                // Clear after reaching end
                this.Current = default!;
                return false;
            }

            // Set current item and increment
            this.Current = this.ringBuffer.buffer[this.index];
            this.index   = this.ringBuffer.NextIndex(this.index);
            this.hasReachedEnd = this.index == this.ringBuffer.end;
            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            if (this.ringBuffer.version != this.version) throw new InvalidOperationException("Ring buffer was modified during enumeration");

            this.index = this.ringBuffer.start;
            this.Current = default!;
            this.hasReachedEnd = this.ringBuffer.IsEmpty;
        }

        /// <inheritdoc />
        object? IEnumerator.Current => this.Current;

        /// <inheritdoc />
        void IDisposable.Dispose() { }
    }
}
