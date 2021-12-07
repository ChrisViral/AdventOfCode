using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 07
/// </summary>
public class Day07 : Solver<int[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver for 2021 - 07 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day07(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int max = this.Data.Max();
        long best = (..^max).AsEnumerable().Min(position => this.Data.Sum(crab => Math.Abs(position - crab)));
        AoCUtils.LogPart1(best);

        best = (..^max).AsEnumerable().Min(position => this.Data.Sum(crab => Math.Abs(position - crab).Triangular()));
        AoCUtils.LogPart2(best);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split(',').ConvertAll(int.Parse);
    #endregion
}
