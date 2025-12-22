using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 06
/// </summary>
public sealed class Day06 : ArraySolver<Vector2<int>>
{
    private const int MAX_DISTANCE = 10_000;

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup data storage
        int[] area = new int[this.Data.Length];
        HashSet<int> notInfinite = new(Enumerable.Range(0, this.Data.Length));

        // Get boundaries
        Vector2<int> min = this.Data.Aggregate(Vector2<int>.Min);
        Vector2<int> max = this.Data.Aggregate(Vector2<int>.Max);
        foreach (Vector2<int> p in (max - min).Enumerate())
        {
            Vector2<int> position = p + min;
            int minIndex = 0;
            int minCount = 1;
            int minDistance = Vector2<int>.ManhattanDistance(position, this.Data[0]);
            foreach (int i in 1..this.Data.Length)
            {
                int distance = Vector2<int>.ManhattanDistance(position, this.Data[i]);
                if (minDistance == distance)
                {
                    minCount++;
                }
                else if (minDistance > distance)
                {
                    minDistance = distance;
                    minIndex = i;
                    minCount = 1;
                    if (distance is 0) break;
                }
            }

            if (minCount is not 1) continue;

            area[minIndex]++;
            if (position.X == min.X || position.X == max.X || position.Y == min.Y || position.Y == max.Y)
            {
                notInfinite.Remove(minIndex);
            }
        }

        int largest = notInfinite.Max(i => area[i]);
        AoCUtils.LogPart1(largest);

        int inRange = 0;
        foreach (Vector2<int> p in (max - min).Enumerate())
        {
            Vector2<int> position = p + min;
            int totalDistance = this.Data.Sum(c => Vector2<int>.ManhattanDistance(position, c));
            if (totalDistance < MAX_DISTANCE)
            {
                inRange++;
            }
        }

        AoCUtils.LogPart2(inRange);
    }

    /// <inheritdoc />
    protected override Vector2<int> ConvertLine(string line) => Vector2<int>.Parse(line);
}
