using System.Diagnostics;
using AdventOfCode.Maths.Vectors;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class GridDebugView<T>(Grid<T>? grid)
{
    private readonly Grid<T> grid = grid ?? throw new ArgumentNullException(nameof(grid));

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public T[,]? Items => this.grid.AsSpan2D().ToArray();
}

internal sealed class SparseGridDebugView<T>(SparseGrid<T>? grid)
{
    private readonly SparseGrid<T> grid = grid ?? throw new ArgumentNullException(nameof(grid));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public DictionaryItemDebugView<Vector2<int>, T>[] Items
    {
        get
        {
            KeyValuePair<Vector2<int>, T>[] keyValuePairs = new KeyValuePair<Vector2<int>, T>[this.grid.Size];
            this.grid.CopyTo(keyValuePairs, 0);

            DictionaryItemDebugView<Vector2<int>, T>[] items = new DictionaryItemDebugView<Vector2<int>, T>[this.grid.Size];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new DictionaryItemDebugView<Vector2<int>, T>(keyValuePairs[i]);
            }
            return items;
        }
    }
}
