using System.Buffers;
using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 5
/// </summary>
[Solver(2015, 5)]
public sealed partial class Day05 : ArraySolver<string>
{
    private static readonly SearchValues<char> Vowels = SearchValues.Create("aeiou");
    private static readonly SearchValues<string> Banned = SearchValues.Create(["ab", "cd", "pq", "xy"], StringComparison.Ordinal);

    [GeneratedRegex(@"([a-z]{2})[a-z]*\1")]
    private static partial Regex PairMatcher { get; }

    [GeneratedRegex(@"([a-z])[a-z]\1")]
    private static partial Regex TripleMatcher { get; }


    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int nice = this.Data.AsValueEnumerable()
                       .Count(l => l.AsSpan().CountAny(Vowels) >= 3
                                && l.AsSpan().ContainsAny(StringUtils.PairsLowercase)
                                && !l.AsSpan().ContainsAny(Banned));
        LogAnswer(nice);

        nice = this.Data.AsValueEnumerable()
                   .Count(l => PairMatcher.IsMatch(l)
                            && TripleMatcher.IsMatch(l));
        LogAnswer(nice);
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
