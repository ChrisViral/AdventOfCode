using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 15
/// </summary>
public sealed class Day15 : GridSolver<byte>
{
    /// <summary>Full size of the map</summary>
    private const int FULL_SIZE = 5;

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver for 2021 - 15 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="byte"/> fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Search for best path to the end
        Vector2<int> start  = Vector2<int>.Zero, end = (this.Grid.Width - 1, this.Grid.Height - 1);
        // ReSharper disable once AccessToModifiedClosure
        Vector2<int>[] path = SearchUtils.Search(start, end, p => Vector2<int>.ManhattanDistance(p, end), node => FindNeighbours(node, this.Grid), MinSearchComparer<double>.Comparer)!;
        int total = path.Sum(p => this.Grid[p]);
        AoCUtils.LogPart1(total);

        // Create scaled map
        Grid<byte> fullMap = new(this.Data.Width * FULL_SIZE, this.Data.Height * FULL_SIZE);
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(this.Data.Width, this.Data.Height))
        {
            int risk = this.Data[position];
            foreach (Vector2<int> offset in Vector2<int>.EnumerateOver(FULL_SIZE, FULL_SIZE))
            {
                Vector2<int> pos = position + offset.Scale(this.Data.Width, this.Data.Height);
                int newRisk = risk + offset.X + offset.Y;
                fullMap[pos] = (byte)(newRisk > 9 ? newRisk - 9 : newRisk);
            }
        }

        // Search for new best path
        end   = (fullMap.Width - 1, fullMap.Height - 1);
        path  = SearchUtils.Search(start, end, p => Vector2<int>.ManhattanDistance(p, end), node => FindNeighbours(node, fullMap), MinSearchComparer<double>.Comparer)!;
        total = path.Sum(p => fullMap[p]);
        AoCUtils.LogPart2(total);
    }

    /// <summary>
    /// Neighbours function for the map
    /// </summary>
    /// <param name="node">Current node position</param>
    /// <param name="map">Map to find the neighbours in</param>
    /// <returns>An enumerable of all the neighbours of the current node</returns>
    private static IEnumerable<MoveData<Vector2<int>, double>> FindNeighbours(Vector2<int> node, Grid<byte> map)
    {
        return node.Adjacent()
                   .Where(n => map.WithinGrid(n))
                   .Select(n => new MoveData<Vector2<int>, double>(n, map[n]));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override byte[] LineConverter(string line) => line.Select(c => (byte)(c - '0')).ToArray();
}
