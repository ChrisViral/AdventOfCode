using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Strings;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 10
/// </summary>
public sealed partial class Day10 : Solver<string>
{
    private const int PART1_REPEATS = 40;
    private const int PART2_REPEATS = 50;

    [GeneratedRegex(@"(\d)\1*")]
    private static partial Regex GroupMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        string result = this.Data;
        foreach (int _ in ..PART1_REPEATS)
        {
            result = GroupMatcher.Replace(result, Expander);
        }
        AoCUtils.LogPart1(result.Length);

        foreach (int _ in PART1_REPEATS..PART2_REPEATS)
        {
            result = GroupMatcher.Replace(result, Expander);
        }
        AoCUtils.LogPart2(result.Length);
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
