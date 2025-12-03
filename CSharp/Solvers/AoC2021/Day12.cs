using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 12
/// </summary>
public class Day12 : Solver<Dictionary<string, Day12.Cave>>
{
    /// <summary>
    /// Cave node
    /// </summary>
    /// <param name="Name">Name of the node</param>
    /// <param name="IsSmall">If it is a small node or not</param>
    public record Cave(string Name, bool IsSmall)
    {
        /// <summary>
        /// If visiting this small node twice is allowed
        /// </summary>
        public bool AllowTwice { get; set; }

        /// <summary>
        /// Set of all the neighbours of this cave
        /// </summary>
        public HashSet<Cave> Neighbours { get; } = new();
    }

    #region Constants
    /// <summary>Start cave name</summary>
    private const string START = "start";
    /// <summary>End cave name</summary>
    private const string END   = "end";
    #endregion

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
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Cave start = this.Data[START];
        HashSet<Cave> visited = new() { start };
        Stack<Cave> path      = new();

        // Explore the caves and list the paths
        path.Push(start);
        HashSet<string> paths = new(ExploreCave(start, visited, path));
        AoCUtils.LogPart1(paths.Count);

        foreach (Cave cave in this.Data.Values.Where(cave => cave.IsSmall && cave.Name is not START or END))
        {
            // Set one small cave to be allowed twice and test
            cave.AllowTwice = true;
            paths.UnionWith(ExploreCave(start, visited, path));
            cave.AllowTwice = false;
        }

        AoCUtils.LogPart2(paths.Count);
    }

    /// <summary>
    /// Explores the cave and finds all the paths possible from this node to the end
    /// </summary>
    /// <param name="cave">Currently explored cave</param>
    /// <param name="visited">Set of all visited caves</param>
    /// <param name="path">Current caving path</param>
    /// <returns>An enumerable of all the paths</returns>
    private static IEnumerable<string> ExploreCave(Cave cave, ISet<Cave> visited, Stack<Cave> path)
    {
        // Look through neighbours
        foreach (Cave neighbour in cave.Neighbours.Where(n => !n.IsSmall || n.AllowTwice || !visited.Contains(n)))
        {
            path.Push(neighbour);
            if (neighbour.Name is END)
            {
                // If found end, return the path
                yield return string.Join("-", path.Select(p => p.Name));

                path.Pop();
                continue;
            }

            // If was allowed twice, disallow now
            bool allowed = neighbour.AllowTwice;
            if (neighbour.AllowTwice)
            {
                neighbour.AllowTwice = false;
            }
            else if (neighbour.IsSmall)
            {
                visited.Add(neighbour);
            }

            // Explore neighbour caves
            foreach (string found in ExploreCave(neighbour, visited, path))
            {
                yield return found;
            }

            // Reset allowed twice flag
            neighbour.AllowTwice = allowed;
            path.Pop();
            visited.Remove(neighbour);
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Cave> Convert(string[] rawInput)
    {
        Dictionary<string, Cave> caves = new();
        foreach (string line in rawInput)
        {
            string[] splits = line.Split('-');
            string fromName = splits[0], toName = splits[1];
            if (!caves.TryGetValue(fromName, out Cave? from))
            {
                from = new(fromName, char.IsLower(fromName[0]));
                caves.Add(fromName, from);
            }

            if (!caves.TryGetValue(toName, out Cave? to))
            {
                to = new(toName, char.IsLower(toName[0]));
                caves.Add(toName, to);
            }

            from.Neighbours.Add(to);
            to.Neighbours.Add(from);
        }

        return caves;
    }
    #endregion
}
