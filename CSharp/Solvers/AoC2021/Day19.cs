﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;
using AdventOfCode.Vectors;
using Transformation = System.Func<AdventOfCode.Vectors.Vector3<int>, AdventOfCode.Vectors.Vector3<int>>;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 19
/// </summary>
public class Day19 : Solver<List<Vector3<int>[]>>
{
    private const int MATCHING = 12;
    private static readonly Transformation[] rotations =
    {
        // Rotation around +Y
        v => new( v.X,  v.Y,  v.Z),
        v => new(-v.Z,  v.Y,  v.X),
        v => new(-v.X,  v.Y, -v.Z),
        v => new( v.Z,  v.Y, -v.X),

        // Rotation around -Y
        v => new( v.X, -v.Y, -v.Z),
        v => new( v.Z, -v.Y,  v.X),
        v => new(-v.X, -v.Y,  v.Z),
        v => new(-v.Z, -v.Y, -v.X),

        // Rotation around +X
        v => new( v.Z,  v.X,  v.Y),
        v => new(-v.Y,  v.X,  v.Z),
        v => new(-v.Z,  v.X, -v.Y),
        v => new( v.Y,  v.X, -v.Z),

        // Rotation around -X
        v => new( v.Z, -v.X, -v.Y),
        v => new( v.Y, -v.X,  v.Z),
        v => new(-v.Z, -v.X,  v.Y),
        v => new(-v.Y, -v.X, -v.Z),

        // Rotation around +Z
        v => new( v.Y,  v.Z,  v.X),
        v => new(-v.X,  v.Z,  v.Y),
        v => new(-v.Y,  v.Z, -v.X),
        v => new( v.X,  v.Z, -v.Y),

        // Rotation around -Z
        v => new( v.Y, -v.Z, -v.X),
        v => new( v.X, -v.Z,  v.Y),
        v => new(-v.Y, -v.Z,  v.X),
        v => new(-v.X, -v.Z, -v.Y),
    };
    private static readonly List<Vector3<int>> buffer = new();

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver for 2021 - 19 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day19(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        List<Vector3<int>> beacons       = new();
        HashSet<Vector3<int>> scanners   = new(this.Data.Count) { Vector3<int>.Zero };
        HashSet<Vector3<int>> allBeacons = new(this.Data[0]);
        this.Data.RemoveAt(0);
        while (!this.Data.IsEmpty())
        {
            foreach (int i in ..this.Data.Count)
            {
                bool found = false;
                foreach (Transformation transformation in rotations)
                {
                    beacons.Clear();
                    beacons.AddRange(this.Data[i].Select(transformation));
                    if (CheckMatching(allBeacons, beacons, out Vector3<int> scanner))
                    {
                        scanners.Add(scanner);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    this.Data.RemoveAt(i);
                    break;
                }
            }
        }

        AoCUtils.LogPart1(allBeacons.Count);

        int distance = scanners.Max(first => scanners.Max(second => Vector3<int>.ManhattanDistance(first, second)));
        AoCUtils.LogPart2(distance);
    }

    private bool CheckMatching(HashSet<Vector3<int>> references, List<Vector3<int>> beacons, out Vector3<int> scanner)
    {
        foreach (Vector3<int> reference in references)
        {
            foreach (Vector3<int> beacon in beacons)
            {
                int matching = 0;
                Vector3<int> origin = reference - beacon;
                buffer.Clear();
                foreach (Vector3<int> fromReference in beacons.Select(b => b + origin))
                {
                    buffer.Add(fromReference);
                    if (references.Contains(fromReference))
                    {
                        matching++;
                    }
                }

                if (matching >= MATCHING)
                {
                    references.UnionWith(buffer);
                    scanner = origin;
                    return true;
                }
            }
        }

        scanner = Vector3<int>.Zero;
        return false;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override List<Vector3<int>[]> Convert(string[] rawInput)
    {
        List<Vector3<int>[]> scanners = new();
        for (int start = 1, end = 1; start < rawInput.Length; start = end + 1, end = start)
        {
            while (++end < rawInput.Length && rawInput[end][..3] is not "---") { }
            scanners.Add(rawInput[start..end].Select(b => Vector3<int>.Parse(b)).ToArray());
        }

        return scanners;
    }
    #endregion
}
