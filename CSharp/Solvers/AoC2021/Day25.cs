using System;
using System.Collections.Generic;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 25
/// </summary>
public sealed class Day25 : GridSolver<Day25.Element>
{
    /// <summary>
    /// Seafloor element
    /// </summary>
    public enum Element
    {
        EMPTY = '.',
        EAST = '>',
        SOUTH = 'v'
    }

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        bool moved;
        int steps = 0;
        List<(Vector2<int>from, Vector2<int> to)> moves = new(100);
        do
        {
            steps++;
            moved = false;

            // Find possible horizontal moves
            for (int y = 0; y < this.Grid.Height; y++)
            {
                for (int x = 0; x < this.Grid.Width; x++)
                {
                    Vector2<int> position = new(x, y);
                    if (this.Grid[position] is not Element.EAST) continue;

                    this.Grid.TryMoveWithinGrid(position, Direction.EAST, out Vector2<int> movedPos, true, true);
                    if (this.Grid[movedPos] is Element.EMPTY)
                    {
                        moves.Add((position, movedPos));
                        moved = true;
                        x++;
                    }
                }
            }

            // Perform all horizontal moves
            foreach ((Vector2<int> from, Vector2<int> to) in moves)
            {
                // Swap locations
                (this.Grid[from], this.Grid[to]) = (this.Grid[to], this.Grid[from]);
            }
            moves.Clear();

            // Find possible vertical moves
            for (int x = 0; x < this.Grid.Width; x++)
            {
                for (int y = 0; y < this.Grid.Height; y++)
                {
                    Vector2<int> position = new(x, y);
                    if (this.Grid[position] is not Element.SOUTH) continue;

                    this.Grid.TryMoveWithinGrid(position, Direction.SOUTH, out Vector2<int> movedPos, true, true);
                    if (this.Grid[movedPos] is Element.EMPTY)
                    {
                        moves.Add((position, movedPos));
                        moved = true;
                        y++;
                    }
                }
            }

            // Perform all vertical moves
            foreach ((Vector2<int> from, Vector2<int> to) in moves)
            {
                // Swap locations
                (this.Grid[from], this.Grid[to]) = (this.Grid[to], this.Grid[from]);
            }
            moves.Clear();
        }
        while (moved);

        AoCUtils.LogPart1(steps);
    }

    /// <inheritdoc />
    protected override Element[] LineConverter(string line) => line.AsSpan().Select(c => (Element)c).ToArray();
}
