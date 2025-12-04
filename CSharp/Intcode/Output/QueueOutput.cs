using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace AdventOfCode.Intcode.Output;

[PublicAPI, DebuggerDisplay("Count = {Count}")]
public sealed class QueueOutput : IOutputProvider
{
    /// <summary>
    /// Default queue capacity
    /// </summary>
    private const int DEFAULT_CAPACITY = 16;

    /// <summary>
    /// Backing queue
    /// </summary>
    private readonly Queue<long> outputQueue;

    /// <inheritdoc />
    public int Count => this.outputQueue.Count;

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
    public void AddOutput(long value) => this.outputQueue.Enqueue(value);

    /// <inheritdoc />
    public long GetOutput() => this.outputQueue.Dequeue();

    /// <inheritdoc />
    public bool TryGetOutput(out long value) => this.outputQueue.TryDequeue(out value);

    /// <inheritdoc />
    public long PeekOutput() => this.outputQueue.Peek();

    /// <inheritdoc />
    public bool TryPeekOutput(out long value) => this.outputQueue.TryPeek(out value);

    /// <inheritdoc />
    public IEnumerable<long> GetAllOutput()
    {
        while (this.outputQueue.TryDequeue(out long value))
        {
            yield return value;
        }
    }

    /// <inheritdoc />
    public void Clear() => this.outputQueue.Clear();

    /// <inheritdoc />
    public IOutputProvider Clone() => new QueueOutput(this.outputQueue);}
