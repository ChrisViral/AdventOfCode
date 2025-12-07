using System;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using Vector2 = AdventOfCode.Vectors.Vector2<int>;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 03
/// </summary>
public sealed class Day03 : GridSolver<bool>
{
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Part one
        long result = CheckSlope((3, 1));
        AoCUtils.LogPart1(result);

        //Part two
        result *= CheckSlope((1, 1));
        result *= CheckSlope((5, 1));
        result *= CheckSlope((7, 1));
        result *= CheckSlope((1, 2));
        AoCUtils.LogPart2(result);
    }

    /// <summary>
    /// Check for collisions on a given slope
    /// </summary>
    /// <param name="slope">Slope to check</param>
    /// <returns>Amount of tree hit on this slope</returns>
    private int CheckSlope(in Vector2 slope)
    {
        int hits = 0;
        Vector2? position = slope;
        do
        {
            //Check the position for a hit
            if (this.Data[position.Value])
            {
                hits++;
            }
            //Move along slope
            position = this.Data.MoveWithinGrid(position.Value, slope, true);
        }
        while (position is not null); //Keep moving until out of bounds at the bottom

        return hits;
    }

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();
}
