using System.Text.RegularExpressions;
using JetBrains.Annotations;
using ZLinq;

namespace AdventOfCode.Utils.ValueEnumerators;

/// <summary>
/// Non ref struct ValueMatch
/// </summary>
/// <param name="Index">Match start index</param>
/// <param name="Length">Match length</param>
public readonly record struct MatchData(int Index, int Length);

/// <summary>
/// ValueEnumerator converter for Regex ValueMatchEnumerator
/// </summary>
/// <param name="enumerator">Enumerator instance</param>
[PublicAPI]
public ref struct FromValueMatchEnumerator(Regex.ValueMatchEnumerator enumerator) : IValueEnumerator<MatchData>
{
    private Regex.ValueMatchEnumerator enumerator = enumerator;

    /// <inheritdoc />
    public bool TryGetNext(out MatchData current)
    {
        if (this.enumerator.MoveNext())
        {
            System.Text.RegularExpressions.ValueMatch match = this.enumerator.Current;
            current = new MatchData(match.Index, match.Length);
            return true;
        }

        current = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = 0;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetSpan(out ReadOnlySpan<MatchData> span)
    {
        span = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryCopyTo(scoped Span<MatchData> destination, Index offset) => false;

    /// <inheritdoc />
    public void Dispose() { }
}
