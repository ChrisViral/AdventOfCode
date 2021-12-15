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
public class Day15 : GridSolver<byte>
{
    private const int FULL_SIZE = 5;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver for 2021 - 15 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="byte"/> fails</exception>
    public Day15(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Vector2<int> start   = Vector2<int>.Zero, end = (this.Grid.Width - 1, this.Grid.Height - 1);
        Vector2<int>[] path = SearchUtils.Search(start, end, p => this.Grid[p],  node => FindNeighbours(node, this.Grid), MinSearchComparer.Comparer)!;
        int total = path.Sum(p => this.Grid[p]);
        AoCUtils.LogPart1(total);

        Grid<byte> fullMap = new(this.Data.Width * FULL_SIZE, this.Data.Height * FULL_SIZE);
        foreach (Vector2<int> position in Vector2<int>.Enumerate(this.Data.Width, this.Data.Height))
        {
            int risk = this.Data[position];
            foreach ((int x, int y) in Vector2<int>.Enumerate(FULL_SIZE, FULL_SIZE))
            {
                Vector2<int> pos = (position.X + (this.Grid.Width * x), position.Y + (this.Grid.Height * y));
                int newRisk = risk + x + y;
                fullMap[pos] = (byte)(newRisk > 9 ? newRisk - 9 : newRisk);
            }
        }

        start = Vector2<int>.Zero;
        end   = (fullMap.Width - 1, fullMap.Height - 1);
        path  = SearchUtils.Search(start, end, p => fullMap[p], node => FindNeighbours(node, fullMap), MinSearchComparer.Comparer)!;
        total = path.Sum(p => fullMap[p]);
        AoCUtils.LogPart2(total);
    }

    private static IEnumerable<(Vector2<int>, double)> FindNeighbours(Vector2<int> node, Grid<byte> map)
    {
        return node.Adjacent()
                   .Where(map.WithinGrid)
                   .Select(neighbour => (neighbour, (double)map[neighbour]));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override byte[] LineConverter(string line)
    {
        return line.Select(c => (byte)(c - '0')).ToArray();
    }
    #endregion
}
