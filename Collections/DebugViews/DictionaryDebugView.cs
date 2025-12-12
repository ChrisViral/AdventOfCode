using System.Diagnostics;

namespace AdventOfCode.Collections.DebugViews;

internal sealed class DictionaryDebugView<TKey, TValue>(IDictionary<TKey, TValue>? dictionary) where TKey : notnull
{
    private readonly IDictionary<TKey, TValue> dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public DictionaryItemDebugView<TKey, TValue>[] Items
    {
        get
        {
            KeyValuePair<TKey, TValue>[] keyValuePairs = new KeyValuePair<TKey, TValue>[this.dictionary.Count];
            this.dictionary.CopyTo(keyValuePairs, 0);

            DictionaryItemDebugView<TKey, TValue>[] items = new DictionaryItemDebugView<TKey, TValue>[keyValuePairs.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new DictionaryItemDebugView<TKey, TValue>(keyValuePairs[i]);
            }
            return items;
        }
    }
}
