using System.Collections.Generic;
using AdventOfCode.Intcode.Input;
using AdventOfCode.Intcode.Output;

namespace AdventOfCode.Intcode;

/// <summary>
/// Simultaneous intcode input/output queue
/// </summary>
public class QueueInOut : IInputProvider, IOutputProvider
{
    #region Constants
    /// <summary>
    /// Default queue capacity
    /// </summary>
    private const int DEFAULT_CAPACITY = 16;
    #endregion

    #region Fields
    /// <summary>
    /// Backing queue
    /// </summary>
    private readonly Queue<long> queue;
    #endregion

    #region Properties
    /// <summary>
    /// Input/Output count
    /// </summary>
    public int Count => this.queue.Count;
    #endregion

    #region Constructors
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
    #endregion

    #region Methods
    /// <inheritdoc />
    public void AddInput(long value) => this.queue.Enqueue(value);

    /// <inheritdoc />
    public void FillInput(IEnumerable<long> values)
    {
        foreach (long value in values)
        {
            this.queue.Enqueue(value);
        }
    }

    /// <inheritdoc />
    public bool TryGetInput(out long input) => this.queue.TryDequeue(out input);

    /// <inheritdoc />
    public void Output(long value) => this.queue.Enqueue(value);

    /// <inheritdoc />
    public long GetOutput() => this.queue.Dequeue();

    /// <inheritdoc />
    public IEnumerable<long> GetAllOutput()
    {
        while (this.queue.TryDequeue(out long value))
        {
            yield return value;
        }
    }

    /// <summary>
    /// Clears this input/output provider
    /// </summary>
    public void Clear() => this.queue.Clear();

    /// <inheritdoc />
    public IInputProvider Clone() => new QueueInOut(this.queue);

    /// <inheritdoc />
    IOutputProvider IOutputProvider.Clone() => new QueueInOut(this.queue);
    #endregion
}