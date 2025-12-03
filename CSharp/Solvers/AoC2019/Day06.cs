using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 06
/// </summary>
public class Day06 : Solver<(Day06.Orbit com, Day06.Orbit you, Day06.Orbit san)>
{
    /// <summary>
    /// Orbit object
    /// </summary>
    public class Orbit : IEnumerable<Orbit>, IEquatable<Orbit>
    {
            /// <summary>
        /// Orbit name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Orbital parent
        /// </summary>
        public Orbit? Parent { get; set; }

        /// <summary>
        /// Orbital children
        /// </summary>
        public List<Orbit> Children { get; } = [];

        /// <summary>
        /// Which Orbit this was visited from while searching
        /// </summary>
        public Orbit? VisitedFrom { get; set; }
    
            /// <summary>
        /// Creates a new Orbit of the specified name
        /// </summary>
        /// <param name="name">Name of the Orbit</param>
        public Orbit(string name) => this.Name = name;
    
            /// <summary>
        /// Gets the total depth of the orbit system
        /// </summary>
        /// <returns>The amount of direct and indirect orbits</returns>
        public int GetOrbits(int depth = 0) => this.Children.Count is not 0 ? depth + this.Children.Sum(c => c.GetOrbits(depth + 1)) : depth;

        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object? obj) => obj is Orbit orbit && Equals(orbit);

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(Orbit? other) => other is not null && (ReferenceEquals(this, other) || this.Name == other.Name);

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => this.Name.GetHashCode();

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Name;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<Orbit> GetEnumerator()
        {
            if (this.Parent is not null)
            {
                yield return this.Parent;
            }

            foreach (Orbit child in this.Children)
            {
                yield return child;
            }
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

    /// <summary>
    /// Name of the root Orbit
    /// </summary>
    private const string ROOT  = "COM";
    /// <summary>
    /// Name of your orbital object
    /// </summary>
    private const string YOU   = "YOU";
    /// <summary>
    /// Name of Santa's orbital object
    /// </summary>
    private const string SANTA = "SAN";

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1, T2, T3}"/> fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.com.GetOrbits());

        HashSet<Orbit> visited = [this.Data.you];
        Queue<Orbit> toVisit = new();
        toVisit.Enqueue(this.Data.you);
        Orbit? santa = null;
        while (toVisit.TryDequeue(out Orbit? visiting))
        {
            if (visiting.Name is SANTA)
            {
                santa = visiting;
                break;
            }

            foreach (Orbit sibling in visiting)
            {
                if (visited.Add(sibling))
                {
                    sibling.VisitedFrom = visiting;
                    toVisit.Enqueue(sibling);
                }
            }
        }

        if (santa is null) return;

        //No need to transfer to YOU and SAN
        int travel = -2;
        while (santa.VisitedFrom is not null)
        {
            santa = santa.VisitedFrom;
            travel++;
        }

        AoCUtils.LogPart2(travel);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Orbit, Orbit, Orbit) Convert(string[] rawInput)
    {
        Dictionary<string, Orbit> orbits = new();
        foreach (string line in rawInput)
        {
            string[] splits = line.Split(')');
            string parentName = splits[0];
            string childName = splits[1];

            if (!orbits.TryGetValue(parentName, out Orbit? parent))
            {
                parent = new Orbit(parentName);
                orbits.Add(parentName, parent);
            }

            if (!orbits.TryGetValue(childName, out Orbit? child))
            {
                child = new Orbit(childName);
                orbits.Add(childName, child);
            }

            parent.Children.Add(child);
            child.Parent = parent;
        }

        return (orbits[ROOT], orbits[YOU], orbits[SANTA]);
    }
}
