using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 22
/// </summary>
public sealed partial class Day22 : Solver<(Grid<char> board, Day22.Movement[] movements)>
{
    /// <summary>
    /// No element
    /// </summary>
    private const char NONE = ' ';
    /// <summary>
    /// Wall element
    /// </summary>
    private const char WALL = '#';

    /// <summary>
    /// Movement struct
    /// </summary>
    /// <param name="Distance">Movement distance</param>
    /// <param name="Turn">Movement turn</param>
    public readonly record struct Movement(int Distance, Direction Turn);

    /// <summary>
    /// Movement match regex
    /// </summary>
    [GeneratedRegex(@"(\d+)([RL])|(\d+)$")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver for 2022 - 22 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day22(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> position = (50, 0);
        Direction direction   = Direction.RIGHT;
        foreach ((int distance, Direction turn) in this.Data.movements)
        {
            foreach (int _ in ..distance)
            {
                Vector2<int> moved = this.Data.board.MoveWithinGrid(position, direction, true, true)!.Value;
                char element    = this.Data.board[moved];
                for (; element is NONE; element = this.Data.board[moved])
                {
                    moved = this.Data.board.MoveWithinGrid(moved, direction, true, true)!.Value;
                }

                if (element is WALL) break;

                position = moved;
            }

            direction = direction.TurnBy(turn);
        }

        // Get final position value
        int password = CalculateLocationPassword(position, direction);
        AoCUtils.LogPart1(password);

        // Definitely not coding a generic way of generating the faces

        // Each face is 50 units wide, and the layout is
        //   0011
        //   0011
        //   22
        //   22
        // 3344
        // 3344
        // 55
        // 55
        Grid<char>[] cube =
        [
            // ReSharper disable RedundantRangeBound
            new(this.Data.board[050..100, 000..050]), // 0
            new(this.Data.board[100..150, 000..050]), // 1
            new(this.Data.board[050..100, 050..100]), // 2
            new(this.Data.board[000..050, 100..150]), // 3
            new(this.Data.board[050..100, 100..150]), // 4
            new(this.Data.board[000..050, 150..200])  // 5
            // ReSharper enable RedundantRangeBound
        ];

        int faceIndex   = 0;
        Grid<char> face = cube[0];
        position  = Vector2<int>.Zero;
        direction = Direction.RIGHT;
        foreach ((int distance, Direction turn) in this.Data.movements)
        {
            foreach (int _ in ..distance)
            {
                // Check if we can move on our current face
                if (face.TryMoveWithinGrid(position, direction, out Vector2<int> moved))
                {
                    // Check if we've hit a wall
                    char element = face[moved];
                    if (element is WALL) break;

                    // If not, update position
                    position = moved;
                }
                else
                {
                    // Transition to new face
                    (moved, Direction newDirection, int newFaceIndex) = TransitionGrids(position, direction, faceIndex);
                    Grid<char> newFace = cube[newFaceIndex];

                    // Check if we've hit a wall
                    char element = newFace[moved];
                    if (element is WALL) break;

                    // If not, location values
                    position  = moved;
                    direction = newDirection;
                    faceIndex = newFaceIndex;
                    face      = newFace;
                }
            }

            // Apply turn
            direction = direction.TurnBy(turn);
        }

        // Update position back to map space
        position += faceIndex switch
        {
            0 => (050, 000),
            1 => (100, 000),
            2 => (050, 050),
            3 => (000, 100),
            4 => (050, 100),
            5 => (000, 150),
            _ => throw new UnreachableException("Unknown face")
        };

        // Get final position value
        password = CalculateLocationPassword(position, direction);
        AoCUtils.LogPart2(password);
    }

    /// <summary>
    /// Gets the password for a given location on the map
    /// </summary>
    /// <param name="position">Map position</param>
    /// <param name="direction">Facing direction</param>
    /// <returns>The password at this location</returns>
    /// <exception cref="UnreachableException">For invalid directions</exception>
    private static int CalculateLocationPassword(Vector2<int> position, Direction direction)
    {
        // Get final position value
        int row    = (position.Y + 1) * 1000;
        int column = (position.X + 1) * 4;
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        int facing = direction switch
        {
            Direction.RIGHT => 0,
            Direction.DOWN  => 1,
            Direction.LEFT  => 2,
            Direction.UP    => 3,
            _               => throw new UnreachableException("Unknown direction")
        };
        return row + column + facing;
    }

    /// <summary>
    /// Transitions a movement between grids
    /// </summary>
    /// <param name="position">Current position</param>
    /// <param name="faceIndex">Cube face</param>
    /// <param name="direction">Travel direction</param>
    /// <returns>A tuple containing the new location values</returns>
    private static (Vector2<int> position, Direction direction, int faceIndex) TransitionGrids(Vector2<int> position, Direction direction, int faceIndex)
    {
        const int MAX = 49;
        return (faceIndex, direction) switch
        {
            // === Face 0 ===
            (0, Direction.LEFT)  => (position with { Y = MAX - position.Y }, Direction.RIGHT, 3),
            (0, Direction.RIGHT) => (position with { X = 0 },                direction,       1),
            (0, Direction.UP)    => (new Vector2<int>(0, position.X),        Direction.RIGHT, 5),
            (0, Direction.DOWN)  => (position with { Y = 0 },                direction,       2),

            // === Face 1 ===
            (1, Direction.LEFT)  => (position with { X = MAX },              direction,       0),
            (1, Direction.RIGHT) => (position with { Y = MAX - position.Y }, Direction.LEFT,  4),
            (1, Direction.UP)    => (position with { Y = MAX },              direction,       5),
            (1, Direction.DOWN)  => (new Vector2<int>(MAX, position.X),      Direction.LEFT,  2),

            // === Face 2 ===
            (2, Direction.LEFT)  => (new Vector2<int>(position.Y, 0),        Direction.DOWN,  3),
            (2, Direction.RIGHT) => (new Vector2<int>(position.Y, MAX),      Direction.UP,    1),
            (2, Direction.UP)    => (position with { Y = MAX },              direction,       0),
            (2, Direction.DOWN)  => (position with { Y = 0 },                direction,       4),

            // === Face 3 ===
            (3, Direction.LEFT)  => (position with { Y = MAX - position.Y }, Direction.RIGHT, 0),
            (3, Direction.RIGHT) => (position with { X = 0 },                direction,       4),
            (3, Direction.UP)    => (new Vector2<int>(0, position.X),        Direction.RIGHT, 2),
            (3, Direction.DOWN)  => (position with { Y = 0 },                direction,       5),

            // === Face 4 ===
            (4, Direction.LEFT)  => (position with { X = MAX },              direction,       3),
            (4, Direction.RIGHT) => (position with { Y = MAX - position.Y }, Direction.LEFT,  1),
            (4, Direction.UP)    => (position with { Y = MAX },              direction,       2),
            (4, Direction.DOWN)  => (new Vector2<int>(MAX, position.X),      Direction.LEFT,  5),

            // === Face 5 ===
            (5, Direction.LEFT)  => (new Vector2<int>(position.Y, 0),        Direction.DOWN,  0),
            (5, Direction.RIGHT) => (new Vector2<int>(position.Y, MAX),      Direction.UP,    4),
            (5, Direction.UP)    => (position with { Y = MAX },              direction,       3),
            (5, Direction.DOWN)  => (position with { Y = 0 },                direction,       1),

            // Invalid
            _                    => throw new UnreachableException("Unknown combination of face and direction")
        };
    }

    /// <inheritdoc />
    protected override (Grid<char>, Movement[]) Convert(string[] lines)
    {
        string[] map = lines[..^1];
        int width = map.Max(l => l.Length);
        Grid<char> board = new(width, map.Length,
                               map.Select(l => l.PadRight(width)).ToArray(),
                               l => l.ToCharArray(),
                               e => e.ToString());

        string path = lines[^1];
        MatchCollection matches = Matcher.Matches(path);
        Movement[] movements = new Movement[matches.Count];
        foreach (int i in ..matches.Count)
        {
            string[] groups = matches[i].CapturedGroups
                                        .Select(g => g.Value)
                                        .ToArray();
            int distance = int.Parse(groups[0]);

            Direction turn = groups switch
            {
                [_]      => Direction.NONE,
                [_, "L"] => Direction.LEFT,
                [_, "R"] => Direction.RIGHT,
                _        => throw new UnreachableException("Unknown movement pattern")
            };
            movements[i] = new Movement(distance, turn);
        }

        return (board, movements);
    }
}
