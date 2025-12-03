using System;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 05
/// </summary>
public class Day05 : Solver<(Vector2<int> from, Vector2<int> to)[]>
{
    /// <summary>Vector matching pattern</summary>
    private const string PATTERN = @"(\d+,\d+) -> (\d+,\d+)";

    private readonly Grid<int> grid;
    private int maxX;
    private int maxY;

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver for 2021 - 05 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day05(string input) : base(input)  => this.grid = new Grid<int>(this.maxX + 1, this.maxY + 1);

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Loop through all lines
        foreach (((int fromX, int fromY), (int toX, int toY)) in this.Data)
        {
            if (fromX == toX)
            {
                // Vertical lines
                foreach (int y in fromY..^toY)
                {
                    grid[fromX, y]++;
                }
            }
            else if (fromY == toY)
            {
                // Horizontal lines
                foreach (int x in fromX..^toX)
                {
                    grid[x, fromY]++;
                }
            }
        }

        int crosses = grid.Count(n => n > 1);
        AoCUtils.LogPart1(crosses);

        // Check diagonal lines
        foreach ((Vector2<int> from, Vector2<int> to) in this.Data.Where(d => d.from.X != d.to.X && d.from.Y != d.to.Y))
        {
            // Check sign and direction
            (int x, int y) = from;
            (int diffX, int diffY) = to - from;
            int xInc   = Math.Sign(diffX);
            int yInc   = Math.Sign(diffY);
            int length = Math.Abs(diffX);
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
        // Extract digits
        (int x1, int y1, int x2, int y2)[] vents = RegexFactory<(int, int, int, int)>.ConstructObjects(PATTERN, rawInput);
        (Vector2<int>, Vector2<int>)[] data      = new (Vector2<int>, Vector2<int>)[vents.Length];

        // Get max values for the grid
        this.maxX = int.MinValue;
        this.maxY = int.MinValue;
        foreach (int i in ..vents.Length)
        {
            (int x1, int y1, int x2, int y2) = vents[i];
            data[i]   = (new Vector2<int>(x1, y1), new Vector2<int>(x2, y2));
            this.maxX = int.Max(x1, x2, this.maxX);
            this.maxY = int.Max(y1, y2, this.maxY);
        }
        return data;
    }
}
