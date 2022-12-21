using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 12
/// </summary>
public class Day12 : GridSolver<int>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver for 2022 - 12 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day12(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        Vector2<int> start = Vector2<int>.Zero;
        Vector2<int> end   = Vector2<int>.Zero;
        foreach (Vector2<int> position in Vector2<int>.Enumerate(this.Grid.Width, this.Grid.Height))
        {
            // Find the start and end positions
            int value = this.Grid[position];
            switch (value)
            {
                case -14: // 'S' - 'a'
                    start = position;
                    this.Grid[position] = 0; // 'a' - 'a'
                    break;

                case -28: // 'E' - 'a'
                    end = position;
                    this.Grid[position] = 25; // 'z' - 'a'
                    break;
            }
        }

        Dictionary<(Vector2<int>, Vector2<int>), int> distances = new();
        int path = SearchUtils.GetPathLength(start, end,
                                             p => Vector2<int>.ManhattanDistance(p, end),
                                             FindNeighbours,
                                             MinSearchComparer.Comparer,
                                             distances)
                              .GetValueOrDefault(-1);
        AoCUtils.LogPart1(path);

        int shortestPath = path;
        foreach (Vector2<int> position in Vector2<int>.Enumerate(this.Grid.Width, this.Grid.Height)
                                                      .Where(p => p != start && this.Grid[p] is 0))
        {
            distances.Clear();
            path = SearchUtils.GetPathLength(position, end,
                                             p => Vector2<int>.ManhattanDistance(p, end),
                                             FindNeighbours,
                                             MinSearchComparer.Comparer,
                                             distances)
                              .GetValueOrDefault(-1);
            if (path is -1) continue;

            shortestPath = Math.Min(shortestPath, path);
        }

        AoCUtils.LogPart2(shortestPath);
    }

    private IEnumerable<(Vector2<int> value, double distance)> FindNeighbours(Vector2<int> node)
    {
        int current = this.Grid[node];
        foreach (Vector2<int> adjacent in node.Adjacent())
        {
            if (!this.Grid.WithinGrid(adjacent)) continue;

            int neighbour = this.Grid[adjacent];
            if (neighbour <= current + 1)
            {
                yield return (adjacent, 1d);
            }
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] LineConverter(string line) => line.Select(c => c - 'a').ToArray();
    #endregion
}
