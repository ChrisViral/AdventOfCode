﻿using System;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 09
/// </summary>
public class Day09 : IntcodeSolver
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        this.VM.AddInput(1L);
        this.VM.Run();
        AoCUtils.LogPart1(this.VM.GetNextOutput());

        this.VM.Reset();
        this.VM.AddInput(2L);
        this.VM.Run();
        AoCUtils.LogPart2(this.VM.GetNextOutput());
    }
    #endregion
}