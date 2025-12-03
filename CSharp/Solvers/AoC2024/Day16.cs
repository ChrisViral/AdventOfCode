using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 16
/// </summary>
public class Day16 : Solver<(Grid<bool> maze, Vector2<int> start, Vector2<int> end)>
{
    private readonly record struct Move(Vector2<int> Position, Direction Direction);

    private static readonly SearchValues<char> Markers = SearchValues.Create('E', 'S');

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day16(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Move start = new(this.Data.start, Direction.EAST);
        Move end   = new(this.Data.end, Direction.NONE);
        (Move[]? moves, HashSet<Move>? unique) = SearchUtils.SearchEquivalentPaths(start, end,
                                                                                   null, Neighbours,
                                                                                   MinSearchComparer<int>.Comparer,
                                                                                   (c, e) => c.Position == e.Position);
        int cost = moves!.Length;
        if (moves[0].Direction is not Direction.EAST)
        {
            cost += 1000;
        }
        foreach (int i in 1..moves.Length)
        {
            if (moves[i].Direction != moves[i - 1].Direction)
            {
                cost += 1000;
            }
        }

        AoCUtils.LogPart1(cost);
        AoCUtils.LogPart2(unique!.DistinctBy(m => m.Position).Count());
    }

    private IEnumerable<MoveData<Move, int>> Neighbours(Move move)
    {
        Grid<bool> maze = this.Data.maze;
        Vector2<int> newPosition = move.Position + move.Direction;
        if (maze[newPosition])
        {
            yield return new MoveData<Move, int>(move with { Position = newPosition }, 1);
        }

        Direction newDirection = move.Direction.TurnRight();
        newPosition = move.Position + newDirection;
        if (maze[newPosition])
        {
            yield return new MoveData<Move, int>(new Move(newPosition, newDirection), 1001);
        }

        newDirection = move.Direction.TurnLeft();
        newPosition  = move.Position + newDirection;
        if (maze[newPosition])
        {
            yield return new MoveData<Move, int>(new Move(newPosition, newDirection), 1001);
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Grid<bool>, Vector2<int>, Vector2<int>) Convert(string[] rawInput)
    {
        Grid<bool> maze    = new(rawInput[0].Length, rawInput.Length, rawInput, l => l.AsSpan().Select(c => c is not '#').ToArray(), b => b ? "." : "#");
        Vector2<int> start = Vector2<int>.Zero;
        Vector2<int> end   = Vector2<int>.Zero;
        foreach (int y in ..rawInput.Length)
        {
            ReadOnlySpan<char> line = rawInput[y];
            int x = rawInput[y].AsSpan().IndexOfAny(Markers);
            if (x is not -1)
            {
                switch (line[x])
                {
                    case 'E':
                        end = new Vector2<int>(x, y);
                        break;

                    case 'S':
                        start = new Vector2<int>(x, y);
                        break;
                }
            }
        }
        return (maze, start, end);
    }
    #endregion
}
