using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 03
/// </summary>
public sealed class Day03 : Solver<(Vector2<int>[] first, Vector2<int>[] second)>
{
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup to walk first wire
        int steps = 0;
        Vector2<int> current = Vector2<int>.Zero;
        Dictionary<Vector2<int>, int> firstWirePath = new(this.Data.first.Sum(v => v.ManhattanLength));
        foreach (Vector2<int> travel in this.Data.first)
        {
            // Get vector length and travel
            int length = travel.ManhattanLength;
            Vector2<int> direction = travel / length;
            foreach (int _ in ..length)
            {
                // Move along wire and add to dictionary with steps taken
                current += direction;
                firstWirePath.TryAdd(current, ++steps);
            }
        }

        // Setup to walk second wire
        steps = 0;
        current = Vector2<int>.Zero;
        Dictionary<Vector2<int>, int> intersections = new(100);
        foreach (Vector2<int> travel in this.Data.second)
        {
            // Get vector length and travel
            int length = travel.ManhattanLength;
            Vector2<int> direction = travel / length;
            foreach (int _ in ..length)
            {
                // Move along wire and add to dictionary with steps taken
                current += direction;
                steps++;
                // Only store intersections, along with total steps taken
                if (firstWirePath.TryGetValue(current, out int firstSteps))
                {
                    intersections.TryAdd(current, steps + firstSteps);
                }
            }
        }

        // Get closest intersection
        int closest = intersections.Keys.Min(v => v.ManhattanLength);
        AoCUtils.LogPart1(closest);

        // Get intersection with less steps
        closest = intersections.Values.Min();
        AoCUtils.LogPart2(closest);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector2<int>[], Vector2<int>[]) Convert(string[] rawInput)
    {
        string[] splits = rawInput[0].Split(',');
        Vector2<int>[] first = new Vector2<int>[splits.Length];
        foreach (int i in ..splits.Length)
        {
            first[i] = Vector2<int>.ParseFromDirection(splits[i]);
        }

        splits = rawInput[1].Split(',');
        Vector2<int>[] second = new Vector2<int>[splits.Length];
        foreach (int i in ..splits.Length)
        {
            second[i] = Vector2<int>.ParseFromDirection(splits[i]);
        }

        return (first, second);
    }
}
