using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Ranges;
using Challenge.Utils.Extensions.Strings;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 10
/// </summary>
[Solver(2015, 10)]
public sealed partial class Day10 : Solver<string>
{
    private const int PART1_REPEATS = 40;
    private const int PART2_REPEATS = 50;

    [GeneratedRegex(@"(\d)\1*")]
    private static partial Regex GroupMatcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        string result = this.Data;
        foreach (int _ in ..PART1_REPEATS)
        {
            result = GroupMatcher.Replace(result, Expander);
        }
        LogAnswer(result.Length);

        foreach (int _ in PART1_REPEATS..PART2_REPEATS)
        {
            result = GroupMatcher.Replace(result, Expander);
        }
        LogAnswer(result.Length);
    }

    private static string Expander(Match match)
    {
        Span<char> result = stackalloc char[2];
        result[0] = match.Length.AsAsciiDigit;
        result[1] = match.Groups[1].ValueSpan[0];
        return new string(result);
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
