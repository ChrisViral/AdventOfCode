using System;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 01
/// </summary>
public class Day01 : Solver<SortedList<int>>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver for 2022 - 01 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day01(string input) : base(input, options: StringSplitOptions.TrimEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        // Top value
        AoCUtils.LogPart1(Data[0]);

        // Top three values
        AoCUtils.LogPart2(Data[..3].Sum());
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
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
    #endregion
}
