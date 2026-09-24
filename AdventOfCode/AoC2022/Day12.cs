using Challenge.Collections.Search;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using ZLinq;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 12
/// </summary>
[Solver(2022, 12)]
public sealed class Day12 : GridSolver<int>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> start = Vector2<int>.Zero;
        Vector2<int> end   = Vector2<int>.Zero;
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(this.Grid.Width, this.Grid.Height))
        {
            // Find the start and end positions
            int value = this.Grid[position];
            switch (value)
            {
                case -14: // 'S' - 'a'
                    start = position;
                    this.Grid[position] = 0; // 'a' - 'a'
                    break;

                case -28: // 'E' - 'a'
                    end = position;
                    this.Grid[position] = 25; // 'z' - 'a'
                    break;
            }
        }

        int path = SearchUtils.GetPathLength(start, end,
                                             p => Vector2<int>.ManhattanDistance(p, end),
                                             FindNeighbours,
                                             MinSearchComparer<double>.Comparer)
                              .GetValueOrDefault(-1);
        LogAnswer(path);

        int shortestPath = path;
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(this.Grid.Width, this.Grid.Height)
                                                      .Where(p => p != start && this.Grid[p] is 0))
        {
            path = SearchUtils.GetPathLength(position, end,
                                             p => Vector2<int>.ManhattanDistance(p, end),
                                             FindNeighbours,
                                             MinSearchComparer<double>.Comparer)
                              .GetValueOrDefault(-1);
            if (path is -1) continue;

            shortestPath = Math.Min(shortestPath, path);
        }

        LogAnswer(shortestPath);
    }

    private IEnumerable<MoveData<Vector2<int>, double>> FindNeighbours(Vector2<int> node)
    {
        int current = this.Grid[node];
        foreach (Vector2<int> adjacent in node.AsAdjacentEnumerable())
        {
            if (!this.Grid.WithinGrid(adjacent)) continue;

            int neighbour = this.Grid[adjacent];
            if (neighbour <= current + 1)
            {
                yield return new MoveData<Vector2<int>, double>(adjacent, 1d);
            }
        }
    }

    /// <inheritdoc />
    protected override int[] LineConverter(string line) => line.Select(c => c - 'a').ToArray();
}
