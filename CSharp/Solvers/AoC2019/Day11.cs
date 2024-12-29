using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 11
/// </summary>
public class Day11 : IntcodeSolver
{
    /// <summary>
    /// Panel colour
    /// </summary>
    private enum Colour
    {
        BLACK = 0,
        WHITE = 1,
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day11(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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

        // Get size of painted area
        Dictionary<Vector2<int>, Colour>.KeyCollection paintedPositions = painted.Keys;
        int minX = paintedPositions.Min(p => p.X);
        int minY = paintedPositions.Min(p => p.Y);
        int maxX = paintedPositions.Max(p => p.X);
        int maxY = paintedPositions.Max(p => p.Y);

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
            this.VM.InputProvider.AddInput((long)current);
            this.VM.Run();

            // Pain hull at position
            painted[position] = (Colour)this.VM.OutputProvider.GetOutput();
            // Turn
            direction = this.VM.OutputProvider.GetOutput() switch
            {
                0L => direction.TurnLeft(),
                1L => direction.TurnRight(),
                _  => direction
            };
            // Move
            position += direction;
        }
    }
    #endregion
}