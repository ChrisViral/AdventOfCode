using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 21
/// </summary>
public class Day21 : ArraySolver<(string code, int value)>
{
    private static readonly StringBuilder Builder = new(256);
    private static readonly ImmutableArray<Direction> NumpadMoveOrder = [Direction.DOWN, Direction.LEFT, Direction.RIGHT, Direction.UP];
    private static readonly ImmutableArray<Direction> KeypadMoveOrder = [Direction.RIGHT, Direction.UP, Direction.DOWN, Direction.LEFT];
    private static readonly FrozenDictionary<Direction, char> Directions = new Dictionary<Direction, char>
    {
        [Direction.UP]    = '^',
        [Direction.DOWN]  = 'v',
        [Direction.LEFT]  = '<',
        [Direction.RIGHT] = '>'
    }.ToFrozenDictionary();
    private static readonly FrozenDictionary<char, Vector2<int>> Numpad = new Dictionary<char, Vector2<int>>
    {
        ['0'] = (1, 3),
        ['1'] = (0, 2),
        ['2'] = (1, 2),
        ['3'] = (2, 2),
        ['4'] = (0, 1),
        ['5'] = (1, 1),
        ['6'] = (2, 1),
        ['7'] = (0, 0),
        ['8'] = (1, 0),
        ['9'] = (2, 0),
        ['A'] = (2, 3)
    }.ToFrozenDictionary();
    private static readonly FrozenDictionary<char, Vector2<int>> Keypad = new Dictionary<char, Vector2<int>>
    {
        ['<'] = (0, 1),
        ['v'] = (1, 1),
        ['>'] = (2, 1),
        ['^'] = (1, 0),
        ['A'] = (2, 0)
    }.ToFrozenDictionary();

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/>[] fails</exception>
    public Day21(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        long complexity = 0L;
        foreach ((string code, int value) in this.Data)
        {
            string moves = GetMovesSequence(code, 2);
            AoCUtils.Log(moves);
            complexity += value * moves.Length;
        }

        AoCUtils.LogPart1(complexity);

        complexity = 0L;
        foreach ((string code, int value) in this.Data)
        {
            string moves = GetMovesSequence(code, 25);
            AoCUtils.Log(moves);
            complexity += value * moves.Length;
        }

        AoCUtils.LogPart2("");
    }

    private static string GetMovesSequence(ReadOnlySpan<char> code, int depth)
    {
        string moves = GetNumpadMoveSequence(code);
        foreach (int i in ..depth)
        {
            moves = GetKeypadMoveSequence(moves);
            AoCUtils.Log($"{i}: {moves.Length}");
        }
        return moves;
    }

    private static string GetNumpadMoveSequence(ReadOnlySpan<char> keys)
    {
        Vector2<int> position = Numpad['A'];
        foreach (char c in keys)
        {
            Vector2<int> destination = Numpad[c];
            DirectionVector<int> directions = (destination - position).ToDirectionVector();
            switch (directions.X.length, directions.Y.length)
            {
                case (> 0, 0):
                    Builder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                case (0, > 0):
                    Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                case (> 0, > 0) when position.Y is 3 && destination.X is 0 || position.X is 0 && destination.Y is 3:
                    if (KeypadMoveOrder.IndexOf(directions.X.direction) < KeypadMoveOrder.IndexOf(directions.Y.direction))
                    {
                        Builder.Append(Directions[directions.X.direction], directions.X.length);
                        Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    }
                    else
                    {
                        Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                        Builder.Append(Directions[directions.X.direction], directions.X.length);
                    }
                    break;

                case (> 0, > 0):
                    if (NumpadMoveOrder.IndexOf(directions.X.direction) < NumpadMoveOrder.IndexOf(directions.Y.direction))
                    {
                        Builder.Append(Directions[directions.X.direction], directions.X.length);
                        Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    }
                    else
                    {
                        Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                        Builder.Append(Directions[directions.X.direction], directions.X.length);
                    }
                    break;
            }

            Builder.Append('A');
            position = destination;
        }

        string result = Builder.ToString();
        Builder.Clear();
        return result;
    }

    private static string GetKeypadMoveSequence(ReadOnlySpan<char> keys)
    {
        Vector2<int> position = Keypad['A'];
        foreach (char c in keys)
        {
            Vector2<int> destination = Keypad[c];
            DirectionVector<int> directions = (destination - position).ToDirectionVector();
            switch (directions.X.length, directions.Y.length)
            {
                case (> 0, 0):
                    Builder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                case (0, > 0):
                    Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                case (> 0, > 0) when KeypadMoveOrder.IndexOf(directions.X.direction) < KeypadMoveOrder.IndexOf(directions.Y.direction):
                    Builder.Append(Directions[directions.X.direction], directions.X.length);
                    Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                case (> 0, > 0):
                    Builder.Append(Directions[directions.Y.direction], directions.Y.length);
                    Builder.Append(Directions[directions.X.direction], directions.X.length);
                    break;
            }

            Builder.Append('A');
            position = destination;
        }

        string result = Builder.ToString();
        Builder.Clear();
        return result;
    }

    /// <inheritdoc />
    protected override (string code, int value) ConvertLine(string line) => (line, int.Parse(line.AsSpan(..3)));
    #endregion
}