using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

[DebuggerDisplay("{Value}", Name = "[{Key}]")]
internal readonly struct DictionaryItemDebugView<TKey, TValue>(TKey key, TValue value)
{
    public DictionaryItemDebugView(KeyValuePair<TKey, TValue> keyValue) : this(keyValue.Key, keyValue.Value) { }

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public TKey Key { get; } = key;

    [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
    public TValue Value { get; } = value;
}
