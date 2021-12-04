using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 17
/// </summary>
public class Day17 : Solver<(Day17.Cube<Vector3<int>> part1, Day17.Cube<Day17.Vector4> part2)>
{
    /// <summary>
    /// Four component integer vector
    /// </summary>
    public readonly struct Vector4 : IEquatable<Vector4>
    {
        #region Properties
        /// <summary>
        /// X component
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Y component
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// Z component
        /// </summary>
        public int Z { get; }
        /// <summary>
        /// W component
        /// </summary>
        public int W { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new 4 component vector
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public Vector4(int x, int y, int z, int w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
        #endregion

        #region Methods
        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object? obj) => obj is Vector4 other && Equals(other);

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(Vector4 other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Z, this.W);

        /// <summary>
        /// Gets all the adjacent vectors to this one
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector4> Adjacent()
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        for (int w = -1; w <= 1; w++)
                        {
                            if (x is 0 && y is 0 && z is 0 && w is 0) continue;

                            yield return new Vector4(this.X + x, this.Y + y, this.Z + z, this.W + w);
                        }
                    }
                }
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Equality operator on two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are equals, false otherwise</returns>
        public static bool operator ==(Vector4 a, Vector4 b) => a.Equals(b);

        /// <summary>
        /// Inequality operator on two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>True if both are unequal, false otherwise</returns>
        public static bool operator !=(Vector4 a, Vector4 b) => !a.Equals(b);
        #endregion
    }

    /// <summary>
    /// Conway cube implementation
    /// </summary>
    /// <typeparam name="T">Type of element within the cube</typeparam>
    public class Cube<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Delegate to create new objects from
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <returns>The created object</returns>
        public delegate T Factory(int x, int y);

        /// <summary>
        /// Delegate to explore the surroundings of a value
        /// </summary>
        /// <param name="value">Value to explore</param>
        /// <returns>It's surrounding values</returns>
        public delegate IEnumerable<T> Explorer(T value);

        #region Fields
        private readonly HashSet<T> activeCubes;
        private readonly Dictionary<T, int> surrounding = new();
        private readonly Explorer explorer;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new cube with the given input, object factory, and explorer
        /// </summary>
        /// <param name="input">Input to create the cube from</param>
        /// <param name="factory">Object factory function</param>
        /// <param name="explorer">Object explorer function</param>
        public Cube(IReadOnlyList<string> input, Factory factory, Explorer explorer)
        {
            this.activeCubes = new HashSet<T>();
            this.explorer = explorer;
            int n = input.Count;
            int l = n / 2;
            foreach (int y in ..n)
            {
                string s = input[y];
                foreach (int x in ..n)
                {
                    if (s[x] is '#')
                    {
                        this.activeCubes.Add(factory(x - l, y - l));
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Simulates the cube for a certain amount of turns
        /// </summary>
        /// <param name="n">Turns amount</param>
        /// <returns>The number of active cubes at the end of the simulation</returns>
        public int Simulate(int n)
        {
            foreach (int _ in ..n)
            {
                foreach (T active in this.activeCubes)
                {
                    foreach (T adjacent in this.explorer(active))
                    {
                        this.surrounding.TryGetValue(adjacent, out int hits);
                        this.surrounding[adjacent] = hits + 1;
                    }
                }

                //Check all positions with hits
                foreach ((T pos, int hits) in this.surrounding)
                {
                    if (!this.activeCubes.Contains(pos) && hits is 3)
                    {
                        this.activeCubes.Add(pos);
                    }
                }

                //Check all active cubes
                foreach (T active in this.activeCubes.ToArray())
                {
                    this.surrounding.TryGetValue(active, out int hits);
                    if (hits is not 2 and not 3)
                    {
                        this.activeCubes.Remove(active);
                    }
                }
                this.surrounding.Clear();
            }

            return this.activeCubes.Count;
        }
        #endregion
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="bool"/>[,,] fails</exception>
    public Day17(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.part1.Simulate(6));
        AoCUtils.LogPart2(this.Data.part2.Simulate(6));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Cube<Vector3<int>>, Cube<Vector4>) Convert(string[] rawInput)
    {
        Cube<Vector3<int>> cube3 = new(rawInput, (x, y) => new(x, y, 0), v => v.Adjacent());
        Cube<Vector4> cube4 = new(rawInput, (x, y) => new(x, y, 0, 0), v => v.Adjacent());
        return (cube3, cube4);
    }
    #endregion
}