using System.Diagnostics;
using System.Runtime.CompilerServices;
using AdventOfCode.Intcode.IO.Input;
using AdventOfCode.Intcode.IO.Output;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.IO;

/// <summary>
/// Simultaneous intcode input/output queue
/// </summary>
[PublicAPI, DebuggerDisplay("Count = {Count}")]
public sealed class QueueInOut : IInputProvider, IOutputProvider
{
    /// <summary>
    /// Default queue capacity
    /// </summary>
    private const int DEFAULT_CAPACITY = 16;

    /// <summary>
    /// Backing queue
    /// </summary>
    private readonly Queue<long> queue;

    /// <summary>
    /// Input/Output count
    /// </summary>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.queue.Count;
    }

    /// <summary>
    ///  Creates a new empty queue in/out with the default capacity
    /// </summary>
    public QueueInOut() : this(DEFAULT_CAPACITY) { }

    /// <summary>
    /// Creates a new queue in/out of the specified capacity
    /// </summary>
    /// <param name="capacity">Queue capacity</param>
    public QueueInOut(int capacity) : this(new Queue<long>(capacity)) { }

    /// <summary>
    /// Creates a new queue in/out primed with the specified values
    /// </summary>
    /// <param name="values">Values to place into the queue</param>
    public QueueInOut(IEnumerable<long> values) : this(new Queue<long>(values)) { }

    /// <summary>
    /// Creates a new queue in/out from the specified Queue
    /// </summary>
    /// <param name="queue">Queue to use for the input</param>
    public QueueInOut(Queue<long> queue) => this.queue = queue;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddValue(long value) => this.queue.Enqueue(value);

    /// <inheritdoc />
    public void FillInput(IEnumerable<long> values)
    {
        foreach (long value in values)
        {
            this.queue.Enqueue(value);
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetInput(out long input) => this.queue.TryDequeue(out input);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddOutput(long value) => this.queue.Enqueue(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetValue() => this.queue.Dequeue();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(out long value) => this.queue.TryDequeue(out value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long PeekValue() => this.queue.Peek();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekValue(out long value) => this.queue.TryPeek(out value);

    /// <inheritdoc />
    public IEnumerable<long> GetAllValues()
    {
        while (this.queue.TryDequeue(out long value))
        {
            yield return value;
        }
    }

    /// <summary>
    /// Clears this input/output provider
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.queue.Clear();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IInputProvider Clone() => new QueueInOut(this.queue);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IOutputProvider IOutputProvider.Clone() => new QueueInOut(this.queue);
}
