using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 18
/// </summary>
public sealed class Day18 : ArraySolver<Vector3<int>>
{
    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver for 2022 - 18 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector3<int>> points = new(this.Data);
        int surface = this.Data.Sum(p => p.AsAdjacentEnumerable().Count(a => !points.Contains(a)));
        AoCUtils.LogPart1(surface);

        Vector3<int> max = (this.Data.Max(p => p.X), this.Data.Max(p => p.Y), this.Data.Max(p => p.Z)) + Vector3<int>.One;
        HashSet<Vector3<int>> empty   = new(Vector3<int>.MakeEnumerable(max.X, max.Y, max.Z).Where(p => !points.Contains(p)));
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

        surface -= pockets.Sum(p => p.AsAdjacentEnumerable().Count(points.Contains));
        AoCUtils.LogPart2(surface);
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
