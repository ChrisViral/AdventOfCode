using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class SpanListDebugView<T>(SpanList<T> span)
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items { get; } = span.ToArray();
}
