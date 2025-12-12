using System;
using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class PriorityQueueDebugView<T>(PriorityQueue<T>? queue) where T : notnull
{
    private readonly PriorityQueue<T> queue = queue ?? throw new ArgumentNullException(nameof(queue));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {
            T[] array = new T[this.queue.Heap.Count];
            this.queue.Heap.CopyTo(array, 0);
            array.Sort();
            return array;
        }
    }
}
