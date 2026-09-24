using System.Text;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Strings;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 19
/// </summary>
[Solver(2017, 19)]
public sealed partial class Day19 : GridSolver<char>
{
    private const char EMPTY = ' ';

    /// <inheritdoc />
    public Day19() : base(options: StringSplitOptions.RemoveEmptyEntries) { }

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
        LogAnswer(path);

        LogAnswer(steps);
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
