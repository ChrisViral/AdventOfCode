using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 18
/// </summary>
public class Day18 : ArraySolver<Vector2<int>>
{
    private const int PART1_COUNT = 1024;
    private static readonly Vector2<int> End = (70, 70);

    private Grid<bool> memory = null!;

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Vector2{T}"/> fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.memory = new Grid<bool>(End.X + 1, End.Y + 1, b => b ? "#" : ".");
        foreach (Vector2<int> bytePos in this.Data.Take(PART1_COUNT))
        {
            this.memory[bytePos] = true;
        }

        Vector2<int>[]? path = SearchUtils.Search(Vector2<int>.Zero, End, Heuristic, Neighbours, MinSearchComparer<int>.Comparer);
        AoCUtils.LogPart1(path!.Length);

        // This would be faster as a binary search, but 300ms is good enough
        HashSet<Vector2<int>> pathContents = [..path];
        foreach (Vector2<int> bytePos in this.Data.AsSpan(PART1_COUNT))
        {
            this.memory[bytePos] = true;
            if (!pathContents.Contains(bytePos)) continue;

            path = SearchUtils.Search(Vector2<int>.Zero, End, Heuristic, Neighbours, MinSearchComparer<int>.Comparer);
            if (path is null)
            {
                AoCUtils.LogPart2($"{bytePos.X},{bytePos.Y}");
                break;
            }

            pathContents.Clear();
            pathContents.AddRange(path);
        }
    }

    private static int Heuristic(Vector2<int> c) => Vector2<int>.ManhattanDistance(c, End);

    private IEnumerable<MoveData<Vector2<int>, int>> Neighbours(Vector2<int> node)
    {
        foreach (Vector2<int> adjacent in node.Adjacent().Where(a => this.memory.TryGetPosition(a, out bool wall) && !wall))
        {
            yield return new MoveData<Vector2<int>, int>(adjacent, 1);
        }
    }

    /// <inheritdoc />
    protected override Vector2<int> ConvertLine(string line) => Vector2<int>.Parse(line);
}
