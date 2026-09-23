using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using ZLinq;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 18
/// </summary>
[Solver(2022, 18)]
public sealed partial class Day18 : ArraySolver<Vector3<int>>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector3<int>> points = new(this.Data);
        int surface = this.Data.Sum(p => p.Adjacent().Count(a => !points.Contains(a)));
        LogAnswer(surface);

        Vector3<int> max = (this.Data.Max(p => p.X), this.Data.Max(p => p.Y), this.Data.Max(p => p.Z)) + Vector3<int>.One;
        HashSet<Vector3<int>> empty   = Vector3<int>.EnumerateOver(max.X, max.Y, max.Z).Where(p => !points.Contains(p)).ToHashSet();
        HashSet<Vector3<int>> pockets = [], outside = [], visited = [];
        Stack<Vector3<int>>   search  = new();
        foreach (Vector3<int> point in empty)
        {
            if (IsInPocket(point, search, points, empty, pockets, outside, visited))
            {
                pockets.Add(point);
            }
            else
            {
                outside.Add(point);
            }
        }

        surface -= pockets.Sum(p => p.Adjacent().Count(points.Contains));
        LogAnswer(surface);
    }

    // ReSharper disable once CognitiveComplexity
    private static bool IsInPocket(Vector3<int> point,            Stack<Vector3<int>> search,
                                   HashSet<Vector3<int>> points,  HashSet<Vector3<int>> empty,
                                   HashSet<Vector3<int>> pockets, HashSet<Vector3<int>> outside,
                                   HashSet<Vector3<int>> visited)
    {
        if (pockets.Contains(point)) return true;

        search.Clear();
        search.Push(point);
        visited.Clear();
        while (search.TryPop(out Vector3<int> current))
        {
            foreach (Vector3<int> adjacent in current.Adjacent())
            {
                if (!visited.Add(adjacent)) continue;

                if (pockets.Contains(adjacent)) return true;         // Connected to another pocket
                if (outside.Contains(adjacent)) return false;        // Connected to outside air
                if (empty.Contains(adjacent)) search.Push(adjacent); // Found another empty point to search through
                else if (!points.Contains(adjacent)) return false;   // Found a point not registered
            }
        }

        // Could not exit, so within a pocket
        return true;
    }

    /// <inheritdoc />
    protected override Vector3<int> ConvertLine(string line) => Vector3<int>.Parse(line);
}
