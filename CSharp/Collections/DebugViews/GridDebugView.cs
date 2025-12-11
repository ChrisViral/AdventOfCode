using System;
using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class GridDebugView<T>(Grid<T>? grid)
{
    private readonly Grid<T> grid = grid ?? throw new ArgumentNullException(nameof(grid));

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public T[,]? Items => this.grid.AsSpan2D().ToArray();
}
