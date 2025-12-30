using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 02
/// </summary>
public sealed class Day02 : ArraySolver<Direction[]>
{
    private const char EMPTY = ' ';

    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day02(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<char> keypad = new(3, 3)
        {
            [0] = "123",
            [1] = "456",
            [2] = "789"
        };
        string code = GetCode(keypad);
        AoCUtils.LogPart1(code);

        keypad = new Grid<char>(5, 5)
        {
            [0] = "  1  ",
            [1] = " 234 ",
            [2] = "56789",
            [3] = " ABC ",
            [4] = "  D  ",
        };
        code = GetCode(keypad);
        AoCUtils.LogPart2(code);
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
