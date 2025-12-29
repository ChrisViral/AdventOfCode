using System.Text;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Strings;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 19
/// </summary>
public sealed class Day19 : GridSolver<char>
{
    private const char EMPTY = ' ';

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int startX = this.Grid[0].IndexOf('|');
        Vector2<int> position = new(startX, 0);
        Direction direction = Direction.DOWN;

        int steps = 1;
        StringBuilder path = new(30);
        while (this.Grid.TryMoveWithinGrid(position, direction, out Vector2<int> moved))
        {
            char current = this.Grid[moved];
            if (this.Grid[moved] is EMPTY)
            {
                direction = direction.TurnRight();
                if (this.Grid.TryMoveWithinGrid(position, direction, out moved)
                 && (current = this.Grid[moved]) is EMPTY)
                {
                    direction = direction.Invert();
                    if (this.Grid.TryMoveWithinGrid(position, direction, out moved)
                     && (current = this.Grid[moved]) is EMPTY)
                    {
                        break;
                    }
                }
            }

            if (current.IsLetterChar)
            {
                path.Append(current);
            }
            position = moved;
            steps++;
        }
        AoCUtils.LogPart1(path);

        AoCUtils.LogPart2(steps);
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
