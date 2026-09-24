using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Utils;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 3
/// </summary>
[Solver(2015, 3)]
public sealed class Day03 : Solver<Direction[]>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        SparseGrid<int> houses = new(this.Data.Length);
        Vector2<int> position = Vector2<int>.Zero;
        houses[position]++;
        foreach (Direction direction in this.Data)
        {
            position += direction;
            houses[position]++;
        }
        LogAnswer(houses.Size);

        houses.Clear();
        Vector2<int> otherPosition = position = Vector2<int>.Zero;
        houses[position] += 2;
        foreach (Direction direction in this.Data)
        {
            position += direction;
            houses[position]++;
            SwapUtils.Swap(ref position, ref otherPosition);
        }
        LogAnswer(houses.Size);
    }

    /// <inheritdoc />
    protected override Direction[] Convert(string[] rawInput) => rawInput[0].AsValueEnumerable()
                                                                            .Select(Direction.ParseDirection)
                                                                            .ToArray();
}
