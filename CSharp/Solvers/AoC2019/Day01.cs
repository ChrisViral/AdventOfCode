﻿using System;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 01
/// </summary>
public class Day01 : ArraySolver<int>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day01(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int fuelRequirement = 0;
        int compoundFuelRequirement = 0;
        foreach (int mass in this.Data)
        {
            int fuel = (mass / 3) - 2;
            fuelRequirement += fuel;
            while (fuel > 8)
            {
                fuel =  (fuel / 3) - 2;
                compoundFuelRequirement += fuel;
            }
        }
        AoCUtils.LogPart1(fuelRequirement);
        AoCUtils.LogPart2(fuelRequirement + compoundFuelRequirement);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
    #endregion
}