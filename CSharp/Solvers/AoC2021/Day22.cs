﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 22
/// </summary>
public class Day22 : Solver<(bool command, Day22.Cuboid cube)[]>
{
    /// <summary>
    /// Cube range
    /// </summary>
    /// <param name="From">Range starting point, inclusive</param>
    /// <param name="To">Range end point, exclusive</param>
    public readonly record struct Range(int From, int To)
    {
        /// <summary>
        /// Range length
        /// </summary>
        public long Length => Math.Abs(To - From);

        /// <summary>
        /// Checks if two ranges overlap
        /// </summary>
        /// <param name="other">Other range to check</param>
        /// <returns><see langword="true"/>if both ranges overlap, <see langword="false"/> otherwise</returns>
        public bool Overlaps(Range other) => From < other.To || To > other.From;
    }

    /// <summary>
    /// Reactor cuboid
    /// </summary>
    /// <param name="X">Cuboid X range</param>
    /// <param name="Y">Cuboid Y range</param>
    /// <param name="Z">Cuboid Z range</param>
    public readonly record struct Cuboid(Range X, Range Y, Range Z)
    {
        /// <summary>
        /// Cuboid volume
        /// </summary>
        public long Volume => this.X.Length * this.Y.Length * this.Z.Length;

        /// <summary>
        /// Checks if two cuboids intersect
        /// </summary>
        /// <param name="other">Other cuboid to intersect with</param>
        /// <returns><see langword="true"/>if both Cuboids intersect, <see langword="false"/> otherwise</returns>
        private bool Intersects(Cuboid other) => this.X.Overlaps(other.X)
                                              && this.Y.Overlaps(other.Y)
                                              && this.Z.Overlaps(other.Z);

        /// <summary>
        /// Subtracts a cuboid from another and finds the remainder cuboids
        /// </summary>
        /// <param name="a">Cuboid to subtract from</param>
        /// <param name="b">Cuboid to subtract</param>
        /// <param name="children">Buffer array to store the results, must be of length 6</param>
        /// <returns>The integer amount of cuboids stored in <paramref name="children"/></returns>
        public static int Subtract(Cuboid a, Cuboid b, ref Cuboid[] children)
        {
            if (!a.Intersects(b))
            {
                children[0] = a;
                return 1;
            }

            b = new(new(Math.Min(Math.Max(b.X.From, a.X.From), a.X.To), Math.Min(Math.Max(b.X.To, a.X.From), a.X.To)),
                    new(Math.Min(Math.Max(b.Y.From, a.Y.From), a.Y.To), Math.Min(Math.Max(b.Y.To, a.Y.From), a.Y.To)),
                    new(Math.Min(Math.Max(b.Z.From, a.Z.From), a.Z.To), Math.Min(Math.Max(b.Z.To, a.Z.From), a.Z.To)));

            children[0] = new(new(a.X.From, b.X.From), new(a.Y.From, a.Y.To),   new(a.Z.From, a.Z.To));
            children[1] = new(new(b.X.To,   a.X.To),   new(a.Y.From, a.Y.To),   new(a.Z.From, a.Z.To));
            children[2] = new(new(b.X.From, b.X.To),   new(a.Y.From, b.Y.From), new(a.Z.From, a.Z.To));
            children[3] = new(new(b.X.From, b.X.To),   new(b.Y.To,   a.Y.To),   new(a.Z.From, a.Z.To));
            children[4] = new(new(b.X.From, b.X.To),   new(b.Y.From, b.Y.To),   new(a.Z.From, b.Z.From));
            children[5] = new(new(b.X.From, b.X.To),   new(b.Y.From, b.Y.To),   new(b.Z.To,   a.Z.To));
            return 6;
        }
    }

    #region Constants
    /// <summary>Parsing Regex pattern</summary>
    private const string PATTERN = @"(on|off) x=(-?\d+)\.\.(-?\d+),y=(-?\d+)\.\.(-?\d+),z=(-?\d+)\.\.(-?\d+)";
    /// <summary>Size of the 3D grid for part 1</summary>
    private const int SIZE = 101;
    /// <summary>Offset for the 3D grid for part 1</summary>
    private const int OFFSET = SIZE / 2;
    /// <summary>Size of the intersect lists for part 2</summary>
    private const int INTERSECT_SIZE = 120000;
    /// <summary>Size of the cuboid buffer for part 2</summary>
    private const int BUFFER_SIZE = 6;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver for 2021 - 22 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day22(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Just doing the naive approach for part one, it's much easier
        bool[,,] grid = new bool[SIZE, SIZE, SIZE];
        foreach ((bool command, (Range xRange, Range yRange, Range zRange)) in this.Data)
        {
            for (int z = Math.Clamp(zRange.From, -OFFSET, OFFSET + 1); z < Math.Clamp(zRange.To, -OFFSET - 1, OFFSET); z++)
            {
                for (int y = Math.Clamp(yRange.From, -OFFSET, OFFSET + 1); y < Math.Clamp(yRange.To, -OFFSET - 1, OFFSET); y++)
                {
                    for (int x = Math.Clamp(xRange.From, -OFFSET, OFFSET + 1); x < Math.Clamp(xRange.To, -OFFSET - 1, OFFSET); x++)
                    {
                        grid[x + 50, y + 50, z + 50] = command;
                    }
                }
            }
        }

        // Count what's left
        long count = Vector3<int>.Enumerate(SIZE, SIZE, SIZE).Count(p => grid[p.X, p.Y, p.Z]);
        AoCUtils.LogPart1(count);

        // Setup buffers
        List<Cuboid> current     = new(INTERSECT_SIZE) { this.Data[0].cube };
        List<Cuboid> intersected = new(INTERSECT_SIZE);
        Cuboid[] buffer          = new Cuboid[BUFFER_SIZE];
        for(int i = 1; i < this.Data.Length; i++)
        {
            (bool turnOn, Cuboid cube) = this.Data[i];
            for (int j = 0; j < current.Count; j++)
            {
                // Get all subtracted cubes
                int size = Cuboid.Subtract(current[j], cube, ref buffer);
                for (int k = 0; k < size; k++)
                {
                    // Add the child if it has a positive volume
                    Cuboid child = buffer[k];
                    if (child.Volume > 0)
                    {
                        intersected.Add(child);
                    }
                }
            }

            // Add the cube at the end if turning on
            if (turnOn)
            {
                intersected.Add(cube);
            }

            // Switch lists for next iteration
            (current, intersected) = (intersected, current);
            intersected.Clear();
        }

        count = current.Sum(cube => cube.Volume);
        AoCUtils.LogPart2(count);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (bool, Cuboid)[] Convert(string[] rawInput)
    {
        (string, int, int, int, int, int, int)[] data = RegexFactory<(string, int, int, int, int, int, int)>.ConstructObjects(PATTERN, rawInput, RegexOptions.Compiled);
        (bool command, Cuboid cuboid)[] cuboids = new (bool command, Cuboid cuboid)[data.Length];

        foreach (int i in ..data.Length)
        {
            (string on, int ax, int bx, int ay, int by, int az, int bz) = data[i];
            cuboids[i] = (on is "on", new(new(ax, bx + 1), new(ay, by + 1), new(az, bz + 1)));
        }

        return cuboids;
    }
    #endregion
}
