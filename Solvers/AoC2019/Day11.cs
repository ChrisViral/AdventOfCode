using System;
using System.Collections.Generic;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 11
/// </summary>
public sealed class Day11 : IntcodeSolver
{
    /// <summary>
    /// Panel colour
    /// </summary>
    private enum Colour
    {
        BLACK = 0,
        WHITE = 1,
    }

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup paint surface
        Dictionary<Vector2<int>, Colour> painted = new(1000);

        // Run and output
        PaintHull(painted);
        AoCUtils.LogPart1(painted.Count);

        // Reset Robot and VM
        this.VM.Reset();
        painted.Clear();
        // Paint starting position
        painted[Vector2<int>.Zero] = Colour.WHITE;

        // Run
        PaintHull(painted);

        // Get the min and max values of the painted area
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        foreach (Vector2<int> position in painted.Keys)
        {
            minX = Math.Min(minX, position.X);
            minY = Math.Min(minY, position.Y);
            maxX = Math.Max(maxX, position.X);
            maxY = Math.Max(maxY, position.Y);
        }

        // Get size vector
        Vector2<int> min  = (minX, minY);
        Vector2<int> max  = (maxX + 1, maxY + 1);
        Vector2<int> size = max - min;

        // Make and fill grid
        Grid<Colour> hull = new(size.X, size.Y, c => c is Colour.BLACK ? "░" : "▓");
        foreach ((Vector2<int> position, Colour colour) in painted)
        {
            hull[position - min] = colour;
        }
        AoCUtils.LogPart2("\n" + hull);
    }

    /// <summary>
    /// Runs the hull painting robot program
    /// </summary>
    /// <param name="painted">Painted hull positions output</param>
    private void PaintHull(Dictionary<Vector2<int>, Colour> painted)
    {
        // Starting position
        Vector2<int> position = Vector2<int>.Zero;
        Direction direction   = Direction.UP;

        // Run until VM halts
        while (!this.VM.IsHalted)
        {
            // Get current hull value
            Colour current = painted.GetValueOrDefault(position, Colour.BLACK);
            this.VM.Input.AddValue((long)current);
            this.VM.Run();

            // Pain hull at position
            painted[position] = (Colour)this.VM.Output.GetValue();
            // Turn
            direction = this.VM.Output.GetValue() switch
            {
                0L => direction.TurnLeft(),
                1L => direction.TurnRight(),
                _  => direction
            };
            // Move
            position += direction;
        }
    }
}
