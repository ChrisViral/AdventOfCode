using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 01
/// </summary>
[Solver(2016, 1)]
public sealed class Day01 : Solver<DirectionVector<int>[]>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int>? hqPosition = null;
        HashSet<Vector2<int>> visited = new(this.Data.Length);
        Vector2<int> position = Vector2<int>.Zero;
        Direction direction = Direction.NORTH;
        foreach (DirectionVector<int> vector in this.Data)
        {
            direction = direction.TurnBy(vector.X.direction);
            foreach (int _ in ..vector.X.length)
            {
                position += direction;
                if (hqPosition is null && !visited.Add(position))
                {
                    hqPosition = position;
                }
            }
        }
        LogAnswer(position.ManhattanLength);
        LogAnswer(hqPosition!.Value.ManhattanLength);
    }

    /// <inheritdoc />
    protected override DirectionVector<int>[] Convert(string[] rawInput) => rawInput[0].Split(", ").ConvertAll(s => Vector2<int>.ParseFromDirection(s).ToDirectionVector());
}
