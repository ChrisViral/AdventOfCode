using System;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 01
/// </summary>
public class Day01 : Solver<int>
{
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/> fails</exception>
    public Day01(string input) : base(input)
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        AoCUtils.LogPart1("");
        AoCUtils.LogPart2("");
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int Convert(string[] rawInput)
    {
        return default;
    }

    #endregion
}