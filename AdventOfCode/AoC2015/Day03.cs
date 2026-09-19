using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Utils;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 03
/// </summary>
[Solver(2015, 3)]
public sealed class Day03 : Solver<Direction[]>
{
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input, ILogger logger) : base(input, logger) { }

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
