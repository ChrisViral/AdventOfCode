using Challenge.Maths.Vectors;
using Challenge.Solvers;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 3
/// </summary>
[Solver(2017, 3)]
public sealed partial class Day03 : Solver<int>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> finalPosition = GenerateSpiral().Skip(this.Data - 1)
                                                     .First();
        LogAnswer(finalPosition.ManhattanLength);

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
        LogAnswer(value);
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
