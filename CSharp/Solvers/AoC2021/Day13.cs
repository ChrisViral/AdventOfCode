using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 13
/// </summary>
public class Day13 : Solver<(List<Day13.Fold> folds, Grid<bool> grid)>
{
    /// <summary>
    /// Folding axis
    /// </summary>
    public enum Axis
    {
        X,
        Y
    }

    /// <summary>
    /// Fold structure
    /// </summary>
    /// <param name="Axis">Fold axis</param>
    /// <param name="Value">Fold position</param>
    public record struct Fold(Axis Axis, int Value);

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver for 2021 - 13 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="List{T}"/> fails</exception>
    public Day13(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Apply the first fold and check how many sections are still marked
        Grid<bool> grid = ApplyFold(this.Data.folds[0], this.Data.grid);
        int enabled = grid.Count(b => b);
        AoCUtils.LogPart1(enabled);

        // Apply the rest of the folds
        foreach (int i in 1..this.Data.folds.Count)
        {
            grid = ApplyFold(this.Data.folds[i], grid);
        }
        AoCUtils.LogPart2($"\n{grid}");
    }

    /// <summary>
    /// Applies a given fold to the grid
    /// </summary>
    /// <param name="fold">Fold to apply</param>
    /// <param name="grid">Grid to fold</param>
    /// <returns>The folded grid result</returns>
    private static Grid<bool> ApplyFold(Fold fold, Grid<bool> grid)
    {
        Grid<bool> updated = null!;
        (Axis axis, int value) = fold;
        switch (axis)
        {
            case Axis.X:
                updated = new Grid<bool>(value, grid.Height, b => b ? "▓" : "░");
                foreach (int x in 1..(grid.Width - value))
                {
                    foreach (int y in ..grid.Height)
                    {
                        // Reflect the values leftwards from the fold axis
                        updated[value - x, y] = grid[value + x, y] || grid[value - x, y];
                    }
                }
                break;

            case Axis.Y:
                updated = new Grid<bool>(grid.Width, value, b => b ? "▓" : "░");
                foreach (int x in ..grid.Width)
                {
                    foreach (int y in 1..(grid.Height - value))
                    {
                        // Reflect the values upwards from the fold axis
                        updated[x, value - y] = grid[x, value + y] || grid[x, value - y];
                    }
                }
                break;
        }

        return updated;
    }

    /// <inheritdoc cref="ArraySolver{T}.ConvertLine"/>
    protected override (List<Fold>, Grid<bool>) Convert(string[] rawInput)
    {
        // Get all the shaded position
        List<Vector2<int>> marks = [];
        int maxX = 0, maxY = 0, i;
        for (i = 0; i < rawInput.Length; i++)
        {
            string[] splits = rawInput[i].Split(',');
            if (splits.Length is not 2) break;
            Vector2<int> mark = new(int.Parse(splits[0]), int.Parse(splits[1]));
            maxX = Math.Max(maxX, mark.X);
            maxY = Math.Max(maxY, mark.Y);
            marks.Add(mark);
        }

        // Mark all in the grid
        Grid<bool> grid = new(maxX + 1, maxY + 1, b => b ? "▓" : "░");
        marks.ForEach(mark => grid[mark] = true);

        // Read all folds
        List<Fold> folds = [];
        for (/*int i*/; i < rawInput.Length; i++)
        {
            string[] splits = rawInput[i].Remove(0, 11).Split('=');
            folds.Add(new Fold(splits[0] is "x" ? Axis.X : Axis.Y, int.Parse(splits[1])));
        }

        return (folds, grid);
    }
    #endregion
}
