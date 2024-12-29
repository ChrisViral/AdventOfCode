using System.Collections.Generic;

namespace AdventOfCode.Intcode.Output;

public sealed class QueueOutput : IOutputProvider
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
    private readonly Queue<long> outputQueue;
    #endregion

    #region Properties
    /// <inheritdoc />
    public int Count => this.outputQueue.Count;
    #endregion

    #region Constructors
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
    #endregion

    #region Methods
    /// <inheritdoc />
    public void Output(long value) => this.outputQueue.Enqueue(value);

    /// <inheritdoc />
    public long GetOutput() => this.outputQueue.Dequeue();

    /// <inheritdoc />
    public IEnumerable<long> GetAllOutput()
    {
        while (this.outputQueue.TryDequeue(out long value))
        {
            yield return value;
        }
    }
    #endregion
}