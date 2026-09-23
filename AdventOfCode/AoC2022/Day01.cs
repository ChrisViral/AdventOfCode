using Challenge.Collections;
using Challenge.Collections.Search;
using Challenge.Solvers;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 1
/// </summary>
[Solver(2022, 1)]
public sealed partial class Day01 : Solver<SortedList<int>>
{
    /// <inheritdoc />
    public Day01(ILogger logger) : base(logger, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Top value
        LogAnswer(this.Data[0]);

        // Top three values
        LogAnswer(this.Data[..3].Sum());
    }

    /// <inheritdoc />
    protected override SortedList<int> Convert(string[] lines)
    {
        int total = 0;
        SortedList<int> elves = new(DescendingComparer<int>.Comparer);
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                total += int.Parse(line);
                continue;
            }

            // Empty lines means end of elf stash
            elves.Add(total);
            total = 0;
        }

        // Add last elf
        elves.Add(total);
        return elves;
    }
}
