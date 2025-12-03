using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 06
/// </summary>
public class Day06 : Solver<Dictionary<string, Day06.OrbitalObject>>
{
    /// <summary>
    /// Orbital object
    /// </summary>
    /// <param name="id">Orbital object ID</param>
    public class OrbitalObject(string id) : IEquatable<OrbitalObject>
    {
        /// <summary>
        /// Object ID
        /// </summary>
        private readonly string id = id;

        /// <summary>
        /// Orbital parent
        /// </summary>
        public OrbitalObject? Parent { get; set; }

        /// <summary>
        /// Orbital children
        /// </summary>
        public List<OrbitalObject> Children { get; } = [];

        /// <summary>
        /// Calculates the orbital checksum
        /// </summary>
        /// <param name="depth">Current orbital depth</param>
        /// <returns>Object's orbital checksum</returns>
        public int GetOrbitalChecksum(int depth = 0)
        {
            int checksum = depth;
            foreach (OrbitalObject child in this.Children)
            {
                checksum += child.GetOrbitalChecksum(depth + 1);
            }
            return checksum;
        }

        /// <inheritdoc />
        public bool Equals(OrbitalObject? other) => this.id == other?.id;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is OrbitalObject other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.id.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => id;
    }

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Calculate the checksum of the COM
        OrbitalObject com = this.Data["COM"];
        int checksum = com.GetOrbitalChecksum();
        AoCUtils.LogPart1(checksum);

        // Get path from start to santa
        OrbitalObject start = this.Data["YOU"];
        OrbitalObject end   = this.Data["SAN"];
        int pathLength = SearchUtils.GetPathLengthBFS(start, end, Neighbours)!.Value;
        AoCUtils.LogPart2(pathLength - 2);
    }

    /// <summary>
    /// OrbitalObject neighbours
    /// </summary>
    /// <param name="orbitalObject">OrbitalObject to get the neighbours for</param>
    /// <returns></returns>
    private static IEnumerable<OrbitalObject> Neighbours(OrbitalObject orbitalObject)
    {
        return orbitalObject.Parent is null
                   ? orbitalObject.Children
                   : orbitalObject.Children.Append(orbitalObject.Parent);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, OrbitalObject> Convert(string[] rawInput)
    {
        Dictionary<string, OrbitalObject> orbitalObjects = new(rawInput.Length / 2);
        var objectsLookup = orbitalObjects.GetAlternateLookup<ReadOnlySpan<char>>();
        foreach (ReadOnlySpan<char> line in rawInput)
        {
            // Get parent
            ReadOnlySpan<char> parentId = line[..3];
            if (!objectsLookup.TryGetValue(parentId, out OrbitalObject? parent))
            {
                parent = new OrbitalObject(parentId.ToString());
                objectsLookup[parentId] = parent;
            }

            // Get child
            ReadOnlySpan<char> childId = line[4..];
            if (!objectsLookup.TryGetValue(childId, out OrbitalObject? child))
            {
                child = new OrbitalObject(childId.ToString());
                objectsLookup[childId] = child;
            }

            // Link child to parent
            child.Parent = parent;
            parent.Children.Add(child);
        }

        return orbitalObjects;
    }
}
