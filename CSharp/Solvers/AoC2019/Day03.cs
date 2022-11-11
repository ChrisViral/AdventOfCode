using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 03
/// </summary>
public class Day03 : Solver<(Vector2<int>[] first, Vector2<int>[] second)>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1, T2}"/> fails</exception>
    public Day03(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Dictionary<Vector2<int>, int> firstVisited = GetVisited(this.Data.first);
        Dictionary<Vector2<int>, int> secondVisited = GetVisited(this.Data.second);

        HashSet<Vector2<int>> intersections = new(firstVisited.Keys);
        intersections.IntersectWith(secondVisited.Keys);

        int min = intersections.Min(i => Math.Abs(i.X) + Math.Abs(i.Y));
        AoCUtils.LogPart1(min);

        min = intersections.Min(i => firstVisited[i] + secondVisited[i]);
        AoCUtils.LogPart2(min);
    }

    /// <summary>
    /// Gets the visited positions for a given wire
    /// </summary>
    /// <param name="movements">Sequence of movements made by the wire</param>
    /// <returns>A dictionary containing the visited location as key and steps amount as values</returns>
    private static Dictionary<Vector2<int>, int> GetVisited(IEnumerable<Vector2<int>> movements)
    {
        int steps = 0;
        Vector2<int> position = Vector2<int>.Zero;
        Dictionary<Vector2<int>, int> visited = new();
        foreach (Vector2<int> movement in movements)
        {
            Vector2<int> step = movement / Math.Max(Math.Abs(movement.X), Math.Abs(movement.Y));
            Vector2<int> target = position + movement;
            do
            {
                steps++;
                position += step;
                visited.TryAdd(position, steps);
            }
            while (position != target);
        }

        return visited;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector2<int>[], Vector2<int>[]) Convert(string[] rawInput) => (rawInput[0].Split(',').ConvertAll(Vector2<int>.ParseFromDirection), rawInput[1].Split(',').ConvertAll(Vector2<int>.ParseFromDirection));
    #endregion
}