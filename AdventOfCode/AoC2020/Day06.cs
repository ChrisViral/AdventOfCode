using Challenge.Solvers;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 6
/// </summary>
[Solver(2020, 6)]
public sealed partial class Day06 : Solver<HashSet<char>[][]>
{
    /// <inheritdoc />
    public Day06(ILogger logger) : base(logger, options: StringSplitOptions.TrimEntries) { }

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
