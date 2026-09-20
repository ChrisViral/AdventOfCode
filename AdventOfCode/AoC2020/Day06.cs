using Challenge.Solvers;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 6
/// </summary>
[Solver(2020, 6)]
public sealed class Day06 : Solver<HashSet<char>[][]>
{
    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="HashSet{T}"/>[] fails</exception>
    public Day06(string input, ILogger logger) : base(input, logger, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int anyTotal = 0;
        int allTotal = 0;
        foreach (HashSet<char>[] group in this.Data)
        {
            HashSet<char> anyAnswered = group[0];
            HashSet<char> allAnswered = new(anyAnswered);
            foreach (HashSet<char> answers in group[1..])
            {
                //Part 1
                anyAnswered.UnionWith(answers);
                //Part 2
                allAnswered.IntersectWith(answers);
            }

            anyTotal += anyAnswered.Count;
            allTotal += allAnswered.Count;
        }

        LogAnswer(anyTotal);
        LogAnswer(allTotal);
    }

    /// <inheritdoc />
    protected override HashSet<char>[][] Convert(string[] rawInput) => CombineLines(rawInput).Select(l => l.Select(s => new HashSet<char>(s)).ToArray())
                                                                                             .ToArray();
}
