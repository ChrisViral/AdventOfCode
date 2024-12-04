using System;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 04
/// </summary>
public class Day04 : Solver<string>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        AoCUtils.LogPart1("");
        AoCUtils.LogPart2("");
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] rawInput)
    {
        return default;
    }
    #endregion
}