using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using AdventOfCode.Vectors.BitVectors;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 24
/// </summary>
public sealed class Day24 : GridSolver<bool>
{
    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> current = this.Grid;
        Grid<bool> next    = new(this.Grid);
        HashSet<BitVector32> states = new(100);

        BitVector32 latest = GridToBitVector(current);
        while(states.Add(latest))
        {
            UpdateBugs(current, next);
            AoCUtils.Swap(ref current, ref next);
            latest = GridToBitVector(current);
        }

        AoCUtils.LogPart1(latest.Data);
        AoCUtils.LogPart2("");
    }

    private static void UpdateBugs(Grid<bool> current, Grid<bool> next)
    {
        foreach (Vector2<int> position in current.Dimensions.Enumerate())
        {
            bool hasBug = current[position];
            int surrounding = position.AsAdjacentEnumerable()
                                      .Where(current.WithinGrid)
                                      .Count(p => current[p]);

            if (hasBug)
            {
                next[position] = surrounding is 1;
            }
            else
            {
                next[position] = surrounding is 1 or 2;
            }
        }
    }

    private static BitVector32 GridToBitVector(Grid<bool> grid)
    {
        Span2D<bool> data = grid.AsSpan2D();
        if (data.TryGetSpan(out Span<bool> bitArray))
        {
            return BitVector32.FromBitArray(bitArray);
        }

        int i = 0;
        BitVector32 result = new();
        foreach (bool value in data)
        {
            result[i++] = value;
        }
        return result;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();

    /// <inheritdoc />
    protected override string StringConversion(bool value) => value ? "#" : ".";
}
