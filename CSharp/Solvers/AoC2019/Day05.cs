using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 05
/// </summary>
public sealed class Day05 : IntcodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM.Input.AddInput(1L);
        this.VM.Run();
        AoCUtils.LogPart1(this.VM.Output.GetAllOutput().Last());

        this.VM.Reset();
        this.VM.Input.AddInput(5L);
        this.VM.Run();
        AoCUtils.LogPart2(this.VM.Output.GetOutput());
    }
}
