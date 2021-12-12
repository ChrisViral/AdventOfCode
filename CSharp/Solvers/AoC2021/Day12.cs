using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 12
/// </summary>
public class Day12 : Solver<(Day12.Cave start, Day12.Cave end)>
{
    public record Cave(string Name, bool IsSmall)
    {
        public bool AllowTwice { get; set; }

        public HashSet<Cave> Neighbours { get; } = new();
    }

    private const string START = "start";
    private const string END   = "end";

    private Dictionary<string, Cave> Caves { get; } = new();

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver for 2021 - 12 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T, T}"/>[] fails</exception>
    public Day12(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        HashSet<Cave> visited = new() { this.Data.start };
        Stack<Cave> path      = new();
        path.Push(this.Data.start);
        HashSet<string> paths = new(ExploreCave(this.Data.start, visited, path));
        AoCUtils.LogPart1(paths.Count);

        foreach (Cave cave in Caves.Values.Where(cave => cave.IsSmall && cave.Name is not START or END))
        {
            cave.AllowTwice = true;
            paths.UnionWith(ExploreCave(this.Data.start, visited, path));
            cave.AllowTwice = false;
        }

        AoCUtils.LogPart2(paths.Count);
    }

    private static IEnumerable<string> ExploreCave(Cave cave, ISet<Cave> visited, Stack<Cave> path)
    {
        foreach (Cave neighbour in cave.Neighbours.Where(n => !n.IsSmall || n.AllowTwice || !visited.Contains(n)))
        {
            path.Push(neighbour);
            if (neighbour.Name is END)
            {
                yield return string.Join("-", path.Select(p => p.Name));

                path.Pop();
                continue;
            }

            bool allowed = neighbour.AllowTwice;
            if (neighbour.AllowTwice)
            {
                neighbour.AllowTwice = false;
            }
            else if (neighbour.IsSmall)
            {
                visited.Add(neighbour);
            }

            foreach (string found in ExploreCave(neighbour, visited, path))
            {
                yield return found;
            }

            neighbour.AllowTwice = allowed;
            path.Pop();
            visited.Remove(neighbour);
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Cave, Cave) Convert(string[] rawInput)
    {
        foreach (string line in rawInput)
        {
            string[] splits = line.Split('-');
            string fromName = splits[0], toName = splits[1];

            if (!Caves.TryGetValue(fromName, out Cave? from))
            {
                from = new(fromName, char.IsLower(fromName[0]));
                Caves.Add(fromName, from);
            }

            if (!Caves.TryGetValue(toName, out Cave? to))
            {
                to = new(toName, char.IsLower(toName[0]));
                Caves.Add(toName, to);
            }

            from.Neighbours.Add(to);
            to.Neighbours.Add(from);
        }

        return (Caves[START], Caves[END]);
    }
    #endregion
}
