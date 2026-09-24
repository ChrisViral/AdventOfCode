using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using ZLinq;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 3
/// </summary>
[Solver(2020, 3)]
public sealed class Day03 : GridSolver<bool>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Part one
        long result = CheckSlope((3, 1));
        LogAnswer(result);

        //Part two
        result *= CheckSlope((1, 1));
        result *= CheckSlope((5, 1));
        result *= CheckSlope((7, 1));
        result *= CheckSlope((1, 2));
        LogAnswer(result);
    }

    /// <summary>
    /// Check for collisions on a given slope
    /// </summary>
    /// <param name="slope">Slope to check</param>
    /// <returns>Amount of tree hit on this slope</returns>
    private int CheckSlope(in Vector2<int> slope)
    {
        int hits = 0;
        Vector2<int>? position = slope;
        do
        {
            //Check the position for a hit
            if (this.Data[position.Value])
            {
                hits++;
            }
            //Move along slope
            position = this.Data.MoveWithinGrid(position.Value, slope, Wrap.HORIZONTAL);
        }
        while (position is not null); //Keep moving until out of bounds at the bottom

        return hits;
    }

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();
}
