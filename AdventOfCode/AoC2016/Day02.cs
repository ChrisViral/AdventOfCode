using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 2
/// </summary>
[Solver(2016, 2)]
public sealed class Day02 : ArraySolver<Direction[]>
{
    private const char EMPTY = ' ';

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<char> keypad = new(3, 3)
        {
            [0] = ['1', '2', '3'],
            [1] = ['4', '5', '6'],
            [2] = ['7', '8', '9']
        };
        string code = GetCode(keypad);
        LogAnswer(code);

        keypad = new Grid<char>(5, 5)
        {
            [0] = [' ', ' ', '1', ' ', ' '],
            [1] = [' ', '2', '3', '4', ' '],
            [2] = ['5', '6', '7', '8', '9'],
            [3] = [' ', 'A', 'B', 'C', ' '],
            [4] = [' ', ' ', 'D', ' ', ' '],
        };
        code = GetCode(keypad);
        LogAnswer(code);
    }

    private string GetCode(Grid<char> keypad)
    {
        Span<char> code = stackalloc char[this.Data.Length];
        Vector2<int> position = keypad.PositionOf('5');
        foreach (int i in ..code.Length)
        {
            Direction[] directions = this.Data[i];
            foreach (Direction direction in directions)
            {
                if (keypad.TryMoveWithinGrid(position, direction, out Vector2<int> moved) && keypad[moved] is not EMPTY)
                {
                    position = moved;
                }
            }
            code[i] = keypad[position];
        }
        return code.ToString();
    }

    /// <inheritdoc />
    protected override Direction[] ConvertLine(string line) => line.Select(Direction.ParseDirection).ToArray();
}
