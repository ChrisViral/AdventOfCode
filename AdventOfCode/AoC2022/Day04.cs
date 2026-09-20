using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 4
/// </summary>
[Solver(2022, 4)]
public sealed partial class Day04 : Solver<((int, int) first, (int, int) second)[]>
{
    /// <summary>
    /// Input match pattern
    /// </summary>
    [GeneratedRegex(@"(\d+)-(\d+),(\d+)-(\d+)")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver for 2022 - 04 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day04(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int fullOverlaps    = 0;
        int partialOverlaps = 0;
        foreach (((int firstStart, int firstEnd), (int secondStart, int secondEnd)) in this.Data)
        {
            if ((firstStart  <= secondStart && firstEnd  >= secondEnd)
             || (secondStart <= firstStart  && secondEnd >= firstEnd))
            {
                fullOverlaps++;
            }
            if ((firstStart  <= secondStart && firstEnd  >= secondStart)
             || (secondStart <= firstStart  && secondEnd >= firstStart))
            {
                partialOverlaps++;
            }
        }

        LogAnswer(fullOverlaps);
        LogAnswer(partialOverlaps);
    }

    /// <inheritdoc />
    protected override ((int, int), (int, int))[] Convert(string[] lines)
    {
        return RegexFactory<(int a, int b, int c, int d)>.ConstructObjects(Matcher, lines)
                                                         .Select(tuple => ((tuple.a, tuple.b),
                                                                           (tuple.c, tuple.d)))
                                                         .ToArray();
    }
}
