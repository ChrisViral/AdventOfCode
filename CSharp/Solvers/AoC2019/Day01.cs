using System;
using System.Linq;
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
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
    public Day01(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.Sum(m => (m / 3) - 2));
        AoCUtils.LogPart2(this.Data.Sum(CalculateFuel));
    }

    /// <summary>
    /// Calculates the fuel necessary for a given mass, accounting for the mass of the fuel
    /// </summary>
    /// <param name="mass">Mass to calculate for</param>
    /// <returns>The total fuel required for the given mass</returns>
    private static int CalculateFuel(int mass)
    {
        mass = (mass / 3) - 2;
        int totalFuel = 0;
        while (mass > 0)
        {
            totalFuel += mass;
            mass = (mass / 3) - 2;
        }

        return totalFuel;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int ConvertLine(string line) => int.Parse(line);
    #endregion
}
