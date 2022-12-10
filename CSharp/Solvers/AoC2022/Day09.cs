using System;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 09
/// </summary>
public class Day09 : Solver<string[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver for 2022 - 09 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        // Part 1 answer
        AoCUtils.LogPart1("");

        // Part 2 answer
        AoCUtils.LogPart2("");
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string[] Convert(string[] lines)
    {
        return lines;
    }
    #endregion
}

