using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 18
/// </summary>
public class Day18 : ArraySolver<Vector3<int>>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver for 2022 - 18 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day18(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        HashSet<Vector3<int>> points = new(this.Data);
        int surface = this.Data.Sum(p => p.Adjacent(false).Count(a => !points.Contains(a)));
        AoCUtils.LogPart1(surface);

        Vector3<int> max = (this.Data.Max(p => p.X), this.Data.Max(p => p.Y), this.Data.Max(p => p.Z)) + Vector3<int>.One;
        HashSet<Vector3<int>> empty   = new(Vector3<int>.Enumerate(max.X, max.Y, max.Z).Where(p => !points.Contains(p)));
        HashSet<Vector3<int>> pockets = new(), outside = new(), visited = new();
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

        surface -= pockets.Sum(p => p.Adjacent(false).Count(points.Contains));
        AoCUtils.LogPart2(surface);
    }

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
            foreach (Vector3<int> adjacent in current.Adjacent(false))
            {
                if (visited.Contains(adjacent)) continue;

                visited.Add(adjacent);
                if (pockets.Contains(adjacent)) return true;         // Connected to another pocket
                if (outside.Contains(adjacent)) return false;        // Connected to outside air
                if (empty.Contains(adjacent)) search.Push(adjacent); // Found another empty point to search through
                else if (!points.Contains(adjacent)) return false;   // Found a point not registered
            }
        }

        // Could not exit, so within a pocket
        return true;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Vector3<int> ConvertLine(string line) => Vector3<int>.Parse(line);
    #endregion
}

