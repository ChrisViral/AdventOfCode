using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 03
/// </summary>
public sealed class Day03 : Solver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> finalPosition = GenerateSpiral().Skip(this.Data - 1)
                                                     .First();
        AoCUtils.LogPart1(finalPosition.ManhattanLength);

        int value = 0;
        Dictionary<Vector2<int>, int> grid = new(this.Data) { [Vector2<int>.Zero] = 1 };
        foreach (Vector2<int> position in GenerateSpiral().Skip(1)
                                                          .TakeWhile(_ => value <= this.Data))
        {
            value = 0;
            foreach (Vector2<int> adjacent in position.Adjacent(withDiagonals: true))
            {
                if (grid.TryGetValue(adjacent, out int adjacentValue))
                {
                    value += adjacentValue;
                }
            }
            grid[position] = value;
        }
        AoCUtils.LogPart2(value);
    }

    private static IEnumerable<Vector2<int>> GenerateSpiral()
    {
        Vector2<int> position = Vector2<int>.Zero;
        Direction direction = Direction.RIGHT;
        int turnDistance = 1;
        int steps = 0;
        while (true)
        {
            yield return position;
            position += direction;
            steps++;
            if (steps == turnDistance)
            {
                steps = 0;
                direction = direction.TurnLeft();
                if (direction.IsHorizontal())
                {
                    turnDistance++;
                }
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
