﻿using System;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 10
/// </summary>
public class Day10 : ArraySolver<(Day10.Operation op, int arg)>
{
    // ReSharper disable once IdentifierTypo
    public enum Operation
    {
        NOOP,
        ADDX
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver for 2022 - 10 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day10(string input) : base(input) { }
    #endregion

    /// <summary>
    /// X register
    /// </summary>
    private int X { get; set; } = 1;

    /// <summary>
    /// Current clock cycle
    /// </summary>
    private int Cycle { get; set; }

    /// <summary>
    /// Cycles signal sum
    /// </summary>
    private int CyclesSum { get; set; }

    /// <summary>
    /// CRT position
    /// </summary>
    private Vector2<int> Position { get; set; }

    /// <summary>
    /// CRT grid
    /// </summary>
    private Grid<bool> CRT { get; } = new(40, 6, v => v ? "█" : " ");

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        foreach ((Operation op, int arg) in this.Data)
        {
            ProcessCycle();
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (op)
            {
                case Operation.NOOP:
                    // Do nothing
                    break;

                case Operation.ADDX:
                    ProcessCycle();
                    this.X += arg;
                    break;
            }
        }

        AoCUtils.LogPart1(this.CyclesSum);
        AoCUtils.LogPart2(string.Empty);
        AoCUtils.Log(this.CRT);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Operation, int) ConvertLine(string line)
    {
        string[] splits = line.Split(' ', DEFAULT_OPTIONS);
        Operation op = Enum.Parse<Operation>(splits[0], true);
        if (splits.Length <= 1 || !int.TryParse(splits[1], out int arg))
        {
            arg = 0;
        }

        return new(op, arg);
    }

    private void ProcessCycle()
    {
        // Check if the sum must be updated
        if ((++this.Cycle + 20).IsMultiple(40))
        {
            this.CyclesSum += this.X * this.Cycle;
        }

        // Update the CRT value
        this.CRT[this.Position] = this.Position.X >= this.X - 1
                               && this.Position.X <= this.X + 1;

        // Update the position
        this.Position = this.Cycle.IsMultiple(40)
                            ? new(0, this.Position.Y + 1)
                            : new(this.Position.X + 1, this.Position.Y);
    }
    #endregion
}

