using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class CollectionDebugView<T>(ICollection<T>? collection)
{
    private readonly ICollection<T> collection = collection ?? throw new ArgumentNullException(nameof(collection));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items
    {
        get
        {
            T[] items = new T[this.collection.Count];
            this.collection.CopyTo(items, 0);
            return items;
        }
    }
}
