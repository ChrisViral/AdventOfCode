using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 12
/// </summary>
public sealed class Day12 : GridSolver<char>
{
    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Queue<Vector2<int>> visiting = [];
        HashSet<Vector2<int>> notVisited = [..this.Grid.Dimensions.Enumerate()];
        Dictionary<Direction, HashSet<Vector2<int>>> fences = Direction.CardinalDirections.ToDictionary(d => d, _ => new HashSet<Vector2<int>>());

        (int regular, int bulk) prices = (0, 0);
        while (notVisited.Count > 0)
        {
            prices += GetAreaPrices(notVisited.First(), notVisited, visiting, fences);
        }

        AoCUtils.LogPart1(prices.regular);
        AoCUtils.LogPart2(prices.bulk);
    }

    // ReSharper disable once CognitiveComplexity
    private Vector2<int> GetAreaPrices(Vector2<int> startingPosition, HashSet<Vector2<int>> notVisited, Queue<Vector2<int>> visiting, Dictionary<Direction, HashSet<Vector2<int>>> fences)
    {
        char region = this.Grid[startingPosition];
        notVisited.Remove(startingPosition);
        visiting.Enqueue(startingPosition);

        int area = 0;
        Vector2<int> currentPosition = startingPosition;
        do
        {
            area++;
            foreach (Direction direction in Direction.CardinalDirections)
            {
                Vector2<int> neighbourPosition = currentPosition + direction;
                if (this.Grid.TryGetPosition(neighbourPosition, out char otherRegion) && otherRegion == region)
                {
                    if (notVisited.Remove(neighbourPosition))
                    {
                        visiting.Enqueue(neighbourPosition);
                    }
                }
                else
                {
                    fences[direction].Add(currentPosition);
                }
            }
        }
        while (visiting.TryDequeue(out currentPosition));

        int sides = 0;
        int perimeter = 0;
        foreach ((Direction direction, HashSet<Vector2<int>> positions) in fences)
        {
            perimeter += positions.Count;
            Direction perpendicular = direction.TurnRight();
            while (positions.Count > 0)
            {
                sides++;
                Vector2<int> current = positions.First();
                positions.Remove(current);
                do
                {
                    Vector2<int> neighbour = current + perpendicular;
                    if (positions.Remove(neighbour))
                    {
                        visiting.Enqueue(neighbour);
                    }

                    neighbour = current + perpendicular.Invert();
                    if (positions.Remove(neighbour))
                    {
                        visiting.Enqueue(neighbour);
                    }
                }
                while (visiting.TryDequeue(out current));
            }
        }

        return (perimeter * area, sides * area);
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
