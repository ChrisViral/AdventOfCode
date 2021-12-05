using System;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 05
/// </summary>
public class Day05 : Solver<(Vector2<int> from, Vector2<int> to)[]>
{
    #region Constants
    private const string PATTERN = @"(\d+),(\d+) -> (\d+),(\d+)";
    #endregion

    #region Fields
    private readonly Grid<int> grid;
    private int maxX;
    private int maxY;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver for 2021 - 05 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day05(string input) : base(input)  => this.grid = new(this.maxX + 1, this.maxY + 1);
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // ReSharper disable UseDeconstruction
        foreach ((Vector2<int> from, Vector2<int> to) in this.Data)
        {
            if (from.X == to.X)
            {
                foreach (int y in from.Y..^to.Y)
                {
                    grid[from.X, y]++;
                }
            }
            else if (from.Y == to.Y)
            {
                foreach (int x in from.X..^to.X)
                {
                    grid[x, from.Y]++;
                }
            }
        }

        int crosses = grid.Count(n => n > 1);
        AoCUtils.LogPart1(crosses);

        foreach ((Vector2<int> from, Vector2<int> to) in this.Data.Where(d => d.from.X != d.to.X && d.from.Y != d.to.Y))
        {
            (int x, int y) = from;
            Vector2<int> diff = to - from;
            int xInc   = Math.Sign(diff.X);
            int yInc   = Math.Sign(diff.Y);
            int length = Math.Abs(diff.X);
            foreach (int _ in ..^length)
            {
                grid[x, y]++;
                x += xInc;
                y += yInc;
            }
        }

        crosses = grid.Count(n => n > 1);
        AoCUtils.LogPart2(crosses);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector2<int> from, Vector2<int> to)[] Convert(string[] rawInput)
    {
        (int x1, int y1, int x2, int y2)[] vents = RegexFactory<(int, int, int, int)>.ConstructObjects(PATTERN, rawInput);
        (Vector2<int>, Vector2<int>)[] data = new (Vector2<int>, Vector2<int>)[vents.Length];

        this.maxX = int.MinValue;
        this.maxY = int.MinValue;
        foreach (int i in ..vents.Length)
        {
            (int x1, int y1, int x2, int y2) = vents[i];
            data[i] = (new(x1, y1), new(x2, y2));
            this.maxX = MathUtils.Max(x1, x2, this.maxX);
            this.maxY = MathUtils.Max(y1, y2, this.maxY);
        }
        return data;
    }
    #endregion
}
