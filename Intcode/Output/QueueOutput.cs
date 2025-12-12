using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Output;

[PublicAPI, DebuggerDisplay("Count = {Count}")]
public class QueueOutput : IOutputProvider
{
    /// <summary>
    /// Default queue capacity
    /// </summary>
    private const int DEFAULT_CAPACITY = 16;

    /// <summary>
    /// Backing queue
    /// </summary>
    protected readonly Queue<long> outputQueue;

    /// <inheritdoc />
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.outputQueue.Count;
    }

    /// <summary>
    ///  Creates a new empty queue output with the default capacity
    /// </summary>
    public QueueOutput() : this(DEFAULT_CAPACITY) { }

    /// <summary>
    /// Creates a new queue output of the specified capacity
    /// </summary>
    /// <param name="capacity">Queue capacity</param>
    public QueueOutput(int capacity) : this(new Queue<long>(capacity)) { }

    /// <summary>
    /// Creates a new queue output from the specified Queue
    /// </summary>
    /// <param name="queue">Queue to use for the input</param>
    public QueueOutput(Queue<long> queue) => this.outputQueue = queue;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void AddOutput(long value) => this.outputQueue.Enqueue(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetValue() => this.outputQueue.Dequeue();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(out long value) => this.outputQueue.TryDequeue(out value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long PeekValue() => this.outputQueue.Peek();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPeekValue(out long value) => this.outputQueue.TryPeek(out value);

    /// <inheritdoc />
    public IEnumerable<long> GetAllValues()
    {
        while (this.outputQueue.TryDequeue(out long value))
        {
            yield return value;
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => this.outputQueue.Clear();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual IOutputProvider Clone() => new QueueOutput(this.outputQueue);
}
