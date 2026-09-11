using System.Text.RegularExpressions;
using ZLinq;

namespace AdventOfCode.Utils.ValueEnumerators;

/// <summary>
/// Regex captures enumerator
/// </summary>
/// <param name="groups">Regex group collection</param>
public ref struct CapturesEnumerator(GroupCollection groups) : IValueEnumerator<Group>
{
    private readonly GroupCollection groups = groups;
    private int index = 1;

    /// <inheritdoc />
    public bool TryGetNext(out Group current)
    {
        while (this.index < this.groups.Count)
        {
            current = this.groups[this.index++];
            if (!current.ValueSpan.IsEmpty) return true;
        }

        current = null!;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetSpan(out ReadOnlySpan<Group> span)
    {
        span = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryCopyTo(scoped Span<Group> destination, Index offset) => false;

    /// <inheritdoc />
    public void Dispose() { }
}
