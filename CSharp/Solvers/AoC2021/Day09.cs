using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 09
/// </summary>
public class Day09 : GridSolver<byte>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver for 2021 - 09 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int risk = 0;
        List<Vector2<int>> lowPoints = new();
        foreach (Vector2<int> position in Vector2<int>.Enumerate(this.Grid.Width, this.Grid.Height))
        {
            byte value = this.Grid[position];
            if (position.Adjacent()
                        .Where(this.Grid.WithinGrid)
                        .Any(pos => value >= this.Grid[pos])) continue;

            lowPoints.Add(position);
            risk += value + 1;
        }

        AoCUtils.LogPart1(risk);

        Queue<Vector2<int>> search = new();
        HashSet<Vector2<int>> basin = new();
        PriorityQueue<int> sizes = new(DescendingComparer<int>.Comparer);
        foreach (Vector2<int> lowPoint in lowPoints)
        {
            search.Enqueue(lowPoint);
            while (search.TryDequeue(out Vector2<int> current))
            {
                basin.Add(current);
                current.Adjacent()
                       .Where(pos => this.Grid.WithinGrid(pos) && this.Grid[pos] is not 9 && basin.Add(pos))
                       .ForEach(search.Enqueue);
            }

            sizes.Enqueue(basin.Count);
            basin.Clear();
        }

        int final = sizes.Dequeue() * sizes.Dequeue() * sizes.Dequeue();
        AoCUtils.LogPart2(final);
    }

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override byte[] LineConverter(string line) => line.Select(c => (byte)(c - '0')).ToArray();
    #endregion
}
