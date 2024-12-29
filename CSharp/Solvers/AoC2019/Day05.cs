using System;
using System.Linq;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 05
/// </summary>
public class Day05 : IntcodeSolver
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day05(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        this.VM.InputProvider.AddInput(1L);
        this.VM.Run();
        AoCUtils.LogPart1(this.VM.OutputProvider.GetAllOutput().Last());

        this.VM.Reset();
        this.VM.InputProvider.AddInput(5L);
        this.VM.Run();
        AoCUtils.LogPart2(this.VM.OutputProvider.GetOutput());
    }
    #endregion
}