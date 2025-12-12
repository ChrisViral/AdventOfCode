using System.Collections.Frozen;
using System.Text;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 21
/// </summary>
public sealed class Day21 : ArraySolver<(string code, int value)>
{
    /// <summary>
    /// Part 1 depth
    /// </summary>
    private const int PART1_DEPTH = 2;
    /// <summary>
    /// Part 2 depth
    /// </summary>
    private const int PART2_DEPTH = 25;

    /// <summary>
    /// Move string builder
    /// </summary>
    private static readonly StringBuilder MoveBuilder = new(32);
    /// <summary>
    /// Move sequence length cache per depth
    /// </summary>
    private static readonly Dictionary<(string, int), long> MoveLengthsCache = new(1000);
    /// <summary>
    /// Direction -> character map
    /// </summary>
    private static readonly FrozenDictionary<Direction, char> Directions = new Dictionary<Direction, char>
    {
        [Direction.UP]    = '^',
        [Direction.DOWN]  = 'v',
        [Direction.LEFT]  = '<',
        [Direction.RIGHT] = '>'
    }.ToFrozenDictionary();
    /// <summary>
    /// Numpad key locations
    /// </summary>
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
    /// <summary>
    /// Keypad key locations
    /// </summary>
    private static readonly FrozenDictionary<char, Vector2<int>> Keypad = new Dictionary<char, Vector2<int>>
    {
        ['<'] = (0, 1),
        ['v'] = (1, 1),
        ['>'] = (2, 1),
        ['^'] = (1, 0),
        ['A'] = (2, 0)
    }.ToFrozenDictionary();

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/>[] fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long complexity = this.Data.Sum(t => GetMovesSequenceLength(t.code, PART1_DEPTH) * t.value);
        AoCUtils.LogPart1(complexity);

        complexity = this.Data.Sum(t => GetMovesSequenceLength(t.code, PART2_DEPTH) * t.value);
        AoCUtils.LogPart2(complexity);
    }

    /// <summary>
    /// Gets the length of the final move sequence
    /// </summary>
    /// <param name="code">Code to input</param>
    /// <param name="depth">Proxy robots depth</param>
    /// <returns>The length of the final code to input</returns>
    private static long GetMovesSequenceLength(string code, int depth)
    {
        string[][] candidateMoves = GetNumpadMoveSequence(code);
        return candidateMoves.Sum(moves => moves.Min(m => GetKeypadMoveLength(m, depth)));
    }

    /// <summary>
    /// Gets the possible move sequences for each digit of a numpad code
    /// </summary>
    /// <param name="code">Numpad code to enter</param>
    /// <returns>The possible sequences for each move</returns>
    private static string[][] GetNumpadMoveSequence(string code)
    {
        string[][] moves = new string[code.Length][];
        Vector2<int> position = Numpad['A'];
        foreach (int i in ..moves.Length)
        {
            Vector2<int> destination = Numpad[code[i]];
            DirectionVector<int> directions = (destination - position).ToDirectionVector();
            switch (directions.X.length, directions.Y.length)
            {
                // Horizontal only movement
                case (> 0, 0):
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                // Vertical only movement
                case (0, > 0):
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                // Moving from bottom row to left column
                case (> 0, > 0) when position.Y is 3 && destination.X is 0:
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                // Moving from left column to bottom row
                case (> 0, > 0) when position.X is 0 && destination.Y is 3:
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                // Other moves
                case (> 0, > 0):
                    // Horizontal first move
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append('A');
                    string horizontalFirst = MoveBuilder.ToString();
                    MoveBuilder.Clear();

                    // Vertical first move
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append('A');
                    string verticalFirst = MoveBuilder.ToString();
                    MoveBuilder.Clear();

                    // Add both for consideration, then skip tail part of the loop
                    moves[i] = [horizontalFirst, verticalFirst];
                    position = destination;
                    continue;
            }

            // Finalize single move
            MoveBuilder.Append('A');
            moves[i] = [MoveBuilder.ToString()];
            MoveBuilder.Clear();
            position = destination;
        }

        return moves;
    }

    /// <summary>
    /// Gets the minimum keypad move length for a certain sequence and proxy depth
    /// </summary>
    /// <param name="move">Move sequence</param>
    /// <param name="depth">Proxy robot depth</param>
    /// <returns>The minimum move sequence length</returns>
    // ReSharper disable once CognitiveComplexity
    private static long GetKeypadMoveLength(string move, int depth)
    {
        // At depth 0, no more recursion to do
        if (depth is 0) return move.Length;
        // Check if the move is cached
        if (MoveLengthsCache.TryGetValue((move, depth), out long length)) return length;

        Vector2<int> position = Keypad['A'];
        foreach (char c in move)
        {
            Vector2<int> destination = Keypad[c];
            DirectionVector<int> directions = (destination - position).ToDirectionVector();
            switch (directions.X.length, directions.Y.length)
            {
                // Horizontal only move
                case (> 0, 0):
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                // Vertical only move
                case (0, > 0):
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                // Moving from > key
                case (> 0, > 0) when position is (0, 1):
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    break;

                // Moving to < key
                case (> 0, > 0) when destination is (0, 1):
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    break;

                // Other moves
                case (> 0, > 0):
                    // Try going horizontally first
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append('A');
                    string horizontalFirst = MoveBuilder.ToString();
                    MoveBuilder.Clear();

                    // Try going vertically first
                    MoveBuilder.Append(Directions[directions.Y.direction], directions.Y.length);
                    MoveBuilder.Append(Directions[directions.X.direction], directions.X.length);
                    MoveBuilder.Append('A');
                    string verticalFirst = MoveBuilder.ToString();
                    MoveBuilder.Clear();

                    // We try both and see which is best
                    long horizontalFirstLength = GetKeypadMoveLength(horizontalFirst, depth - 1);
                    long verticalFirstLength   = GetKeypadMoveLength(verticalFirst,   depth - 1);
                    length  += Math.Min(horizontalFirstLength, verticalFirstLength);

                    // Skip tail end of loop
                    position =  destination;
                    continue;
            }

            // Get length for subsequence
            MoveBuilder.Append('A');
            string subMove = MoveBuilder.ToString();
            MoveBuilder.Clear();
            length  += GetKeypadMoveLength(subMove, depth - 1);
            position = destination;
        }

        // Cache result
        MoveLengthsCache[(move, depth)] = length;
        return length;
    }

    /// <inheritdoc />
    protected override (string code, int value) ConvertLine(string line) => (line, int.Parse(line.AsSpan(..3)));
}
