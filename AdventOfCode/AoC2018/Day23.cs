using System.Text.RegularExpressions;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 23
/// </summary>
public sealed partial class Day23 : RegexSolver<Day23.Nanobot>
{
    public sealed record Nanobot(Vector3<long> Position, long Radius)
    {
        public bool IsInRange(Vector3<long> other) => Vector3<long>.ManhattanDistance(this.Position, other) <= this.Radius;
    }

    private const int SEARCH_SIZE = 10;

    /// <inheritdoc />
    [GeneratedRegex(@"pos=<(-?\d+,-?\d+,-?\d+)>, r=(\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Nanobot strongest = this.Data.MaxBy(n => n.Radius)!;
        int nanobotsInRange = this.Data.Count(n => strongest.IsInRange(n.Position));
        AoCUtils.LogPart1(nanobotsInRange);

        // Get maximum component size
        int digitCount = this.Data
                            .AsSpan()
                            .Select(n => Vector3<long>.Abs(n.Position))
                            .Aggregate(Vector3<long>.Max)
                            .AsSpan()
                            .Max()
                            .DigitCount;
        // Get max component magnitude
        int magnitude = (digitCount - 1).Pow10;

        // Setup for search
        int bestInRange   = 0;
        long bestDistance = int.MaxValue;
        Vector3<long> bestPosition = Vector3<long>.Zero;
        Vector3<long> offset       = Vector3<long>.Zero;
        for (int precision = magnitude / SEARCH_SIZE; precision > 0; precision /= 10)
        {
            // For each degree of precision, test areas at regular intervals
            foreach (Vector3<long> position in Search(precision, offset))
            {
                // Count how many nanobots are in range
                int inRange = this.Data.Count(n => n.IsInRange(position));
                if (bestInRange < inRange)
                {
                    // If we have a new best, keep it
                    bestInRange  = inRange;
                    bestPosition = position;
                    bestDistance = position.ManhattanLength;
                }
                else if (bestInRange == inRange)
                {
                    // If we have a match, keep the one closer to the origin
                    long distance = position.ManhattanLength;
                    if (bestDistance > distance)
                    {
                        bestInRange  = inRange;
                        bestPosition = position;
                        bestDistance = distance;
                    }
                }
            }

            // Setup new offset to current best position
            offset = bestPosition;
        }

        // Return final distance from origin
        AoCUtils.LogPart2(bestPosition.ManhattanLength);
    }

    private static IEnumerable<Vector3<long>> Search(long precision, Vector3<long> offset)
    {
        for (int x = -SEARCH_SIZE + 1; x < SEARCH_SIZE; x++)
        {
            long xValue = (x * precision) + offset.X;
            for (int y = -SEARCH_SIZE + 1; y < SEARCH_SIZE; y++)
            {
                long yValue = (y * precision) + offset.Y;
                for (int z = -SEARCH_SIZE + 1; z < SEARCH_SIZE; z++)
                {
                    yield return new Vector3<long>(xValue, yValue, (z * precision) + offset.Z);
                }
            }
        }
    }
}
