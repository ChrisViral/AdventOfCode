using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 22
/// </summary>
public class Day22 : Solver<(Grid<Day22.Element> board, Day22.Movement[] movements)>
{
    public enum Element
    {
        NONE  = ' ',
        OPEN  = '.',
        WALL  = '#',
        LEFT  = '<',
        RIGHT = '>',
        UP    = '^',
        DOWN  = 'v'
    }

    public readonly record struct Movement(int Distance, Directions Turn);

    private static readonly Regex match = new(@"(\d+)([RL])|(\d+)$", RegexOptions.Compiled);

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver for 2022 - 22 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day22(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        Vector2<int> position = Vector2<int>.Enumerate(this.Data.board.Width, this.Data.board.Height).First(p => this.Data.board[p] is Element.OPEN);
        Directions direction  = Directions.RIGHT;
        foreach ((int distance, Directions turn) in this.Data.movements)
        {
            //this.Data.board[position] = GetDirectionElement(direction);
            foreach (int _ in ..distance)
            {
                Vector2<int> moved = this.Data.board.MoveWithinGrid(position, direction, true, true)!.Value;
                Element element    = this.Data.board[moved];
                for (; element is Element.NONE; element = this.Data.board[moved])
                {
                    moved = this.Data.board.MoveWithinGrid(moved, direction, true, true)!.Value;
                }

                if (element is Element.WALL) break;

                position = moved;
                //this.Data.board[position] = GetDirectionElement(direction);
            }

            direction = turn is Directions.LEFT ? direction.TurnLeft() : direction.TurnRight();
        }

        // Since we do a right turn at the end
        direction  = direction.TurnLeft();
        int row    = (position.Y + 1) * 1000;
        int column = (position.X + 1) * 4;
        int facing = direction switch
        {
            Directions.RIGHT => 0,
            Directions.DOWN  => 1,
            Directions.LEFT  => 2,
            Directions.UP    => 3,
            _                => throw new UnreachableException("Unknown direction")
        };
        //AoCUtils.Log(this.Data.board);
        AoCUtils.LogPart1(row + column + facing);

        Grid<Element>[] cube =
        {
            this.Data.board[050..100, 000..050],
            this.Data.board[100..150, 000..050],
            this.Data.board[050..100, 050..100],
            this.Data.board[000..050, 100..150],
            this.Data.board[050..100, 100..150],
            this.Data.board[000..050, 150..200]
        };

        AoCUtils.LogPart2("");
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Grid<Element>, Movement[]) Convert(string[] lines)
    {
        string[] map = lines[..^1];
        int width = map.Max(l => l.Length);
        Grid<Element> board = new(width, map.Length,
                                  map.Select(l => l.PadRight(width)).ToArray(),
                                  l => l.Select(c => (Element)c).ToArray(),
                                  e => ((char)e).ToString());

        string path = lines[^1];
        MatchCollection matches = match.Matches(path);
        Movement[] movements = new Movement[matches.Count];
        foreach (int i in ..matches.Count)
        {
            string[] groups = matches[i].GetCapturedGroups()
                                        .Select(g => g.Value)
                                        .ToArray();
            int distance = int.Parse(groups[0]);
            Directions turn = groups is [_, "L"] ? Directions.LEFT : Directions.RIGHT;
            movements[i] = new(distance, turn);
        }

        return (board, movements);
    }

    private static Element GetDirectionElement(Directions direction) => direction switch
    {
        Directions.RIGHT => Element.RIGHT,
        Directions.LEFT  => Element.LEFT,
        Directions.UP    => Element.UP,
        Directions.DOWN  => Element.DOWN,
        _                => throw new UnreachableException("Unexpected direction")
    };

    private static void TransitionGrids(ref Vector2<int> position, ref int face, ref Directions direction)
    {
        const int max = 49;
        switch (face, direction)
        {
            case (0, Directions.LEFT):
                break;
            case (0, Directions.RIGHT):
                face = 1;
                position = position with { X = 0 };
                break;
            case (0, Directions.UP):
                break;
            case (0, Directions.DOWN):
                face = 2;
                position = position with { Y = 0 };
                break;

            case (1, Directions.LEFT):
                face = 0;
                position = position with { X = max };
                break;
            case (1, Directions.RIGHT):
                break;
            case (1, Directions.UP):
                break;
            case (1, Directions.DOWN):
                break;

            case (2, Directions.LEFT):
                break;
            case (2, Directions.RIGHT):
                break;
            case (2, Directions.UP):
                face = 0;
                position = position with { Y = max };
                break;
            case (2, Directions.DOWN):
                face = 4;
                position = position with { Y = 0 };
                break;

            case (3, Directions.LEFT):
                break;
            case (3, Directions.RIGHT):
                face = 4;
                position = position with { X = 0 };
                break;
            case (3, Directions.UP):
                break;
            case (3, Directions.DOWN):
                face = 5;
                position = position with { Y = 0 };
                break;

            case (4, Directions.LEFT):
                face = 3;
                position = position with { X = max };
                break;
            case (4, Directions.RIGHT):
                break;
            case (4, Directions.UP):
                face = 2;
                position = position with { Y = max };
                break;
            case (4, Directions.DOWN):
                break;

            case (5, Directions.LEFT):
                break;
            case (5, Directions.RIGHT):
                break;
            case (5, Directions.UP):
                face = 3;
                position = position with { Y = max };
                break;
            case (5, Directions.DOWN):
                break;
        }
    }
    #endregion
}
