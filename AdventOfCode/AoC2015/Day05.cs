using System.Buffers;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 05
/// </summary>
public sealed partial class Day05 : ArraySolver<string>
{
    private static readonly SearchValues<char> Vowels = SearchValues.Create("aeiou");
    private static readonly SearchValues<string> Banned = SearchValues.Create(["ab", "cd", "pq", "xy"], StringComparison.Ordinal);
    private static readonly SearchValues<string> Pairs = SearchValues.Create([
        "aa",
        "bb",
        "cc",
        "dd",
        "ee",
        "ff",
        "gg",
        "hh",
        "ii",
        "jj",
        "kk",
        "ll",
        "mm",
        "nn",
        "oo",
        "pp",
        "qq",
        "rr",
        "ss",
        "tt",
        "uu",
        "vv",
        "ww",
        "xx",
        "yy",
        "zz"
    ], StringComparison.Ordinal);

    [GeneratedRegex(@"([a-z]{2})[a-z]*\1")]
    private static partial Regex PairMatcher { get; }

    [GeneratedRegex(@"([a-z])[a-z]\1")]
    private static partial Regex TripleMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int nice = this.Data.AsValueEnumerable()
                       .Count(l => l.AsSpan().CountAny(Vowels) >= 3
                                && l.AsSpan().ContainsAny(Pairs)
                                && !l.AsSpan().ContainsAny(Banned));
        AoCUtils.LogPart1(nice);

        nice = this.Data.AsValueEnumerable()
                   .Count(l => PairMatcher.IsMatch(l)
                            && TripleMatcher.IsMatch(l));
        AoCUtils.LogPart2(nice);
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
