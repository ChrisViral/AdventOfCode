using System.Collections.Generic;

namespace AdventOfCode.Intcode.Input;

/// <summary>
/// Queue backed Intcode input
/// </summary>
public sealed class QueueInput : IInputProvider
{    /// <summary>
    /// Default queue capacity
    /// </summary>
    private const int DEFAULT_CAPACITY = 16;

    /// <summary>
    /// Backing queue
    /// </summary>
    private readonly Queue<long> inputQueue;

    /// <inheritdoc />
    public int Count => this.inputQueue.Count;

    /// <summary>
    ///  Creates a new empty queue input with the default capacity
    /// </summary>
    public QueueInput() : this(DEFAULT_CAPACITY) { }

    /// <summary>
    /// Creates a new queue input of the specified capacity
    /// </summary>
    /// <param name="capacity">Queue capacity</param>
    public QueueInput(int capacity) : this(new Queue<long>(capacity)) { }

    /// <summary>
    /// Creates a new queue input primed with the specified values
    /// </summary>
    /// <param name="values">Values to place into the queue</param>
    public QueueInput(IEnumerable<long> values) : this(new Queue<long>(values)) { }

    /// <summary>
    /// Creates a new queue input from the specified Queue
    /// </summary>
    /// <param name="queue">Queue to use for the input</param>
    public QueueInput(Queue<long> queue) => this.inputQueue = queue;

    /// <inheritdoc />
    public void AddInput(long value) => this.inputQueue.Enqueue(value);

    /// <inheritdoc />
    public void FillInput(IEnumerable<long> values)
    {
        foreach (long value in values)
        {
            this.inputQueue.Enqueue(value);
        }
    }

    /// <inheritdoc />
    public bool TryGetInput(out long input) => this.inputQueue.TryDequeue(out input);

    /// <inheritdoc />
    public void Clear() => this.inputQueue.Clear();

    /// <inheritdoc />
    public IInputProvider Clone() => new QueueInput(this.inputQueue);}
