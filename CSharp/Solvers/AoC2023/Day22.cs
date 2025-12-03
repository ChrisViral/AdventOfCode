using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 22
/// </summary>
public class Day22 : ArraySolver<Day22.Brick>
{
    public class Brick : IComparable<Brick>, IEquatable<Brick>
    {
        private readonly string data;

        public Vector3<int> Min { get; private set; }

        public Vector3<int> Max { get; private set; }

        public int Bottom => this.Min.Z;

        public int Top    => this.Max.Z;

        public List<Brick> Supports { get; }    = [];

        public List<Brick> SupportedBy { get; } = [];

        public Brick(string data)
        {
            this.data = data;
            string[] points = data.Split('~');
            Vector3<int> min = Vector3<int>.Parse(points[0]);
            Vector3<int> max = Vector3<int>.Parse(points[1]);

            if (max.X < min.X
             || max.Y < min.Y
             || max.Z < min.Z)
            {
                AoCUtils.Swap(ref min, ref max);
            }

            this.Min = min;
            this.Max = max;
        }

        public void MoveDown()
        {
            this.Min += Vector3<int>.Backwards;
            this.Max += Vector3<int>.Backwards;
        }

        public bool OverlapsWith(Brick other) => this.Min.X <= other.Max.X
                                              && this.Max.X >= other.Min.X
                                              && this.Min.Y <= other.Max.Y
                                              && this.Max.Y >= other.Min.Y;

        public void AddSupport(Brick other)
        {
            this.SupportedBy.Add(other);
            other.Supports.Add(this);
        }

        public bool SafeToDisintegrate() => this.Supports.IsEmpty
                                         || this.Supports.All(b => b.SupportedBy.Count > 1);

        /// <inheritdoc />
        public int CompareTo(Brick? other) => other is not null ? this.Min.Z - other.Min.Z : -1;

        public override string ToString() => this.data;

        /// <inheritdoc />
        public bool Equals(Brick? other) => !ReferenceEquals(null, other)
                                         && (ReferenceEquals(this, other)
                                          || this.data == other.data);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Brick other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.data.GetHashCode();
        }

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day22(string input) : base(input) => this.Data.Sort();

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int maxHeight = this.Data[^1].Top;
        Dictionary<int, List<Brick>> brickTops = Enumerable.Range(1, maxHeight)
                                                           .ToDictionary(i => i, _ => new List<Brick>());

        foreach (Brick brick in this.Data)
        {
            while (brick.Bottom > 1)
            {
                brickTops[brick.Bottom - 1].Where(brick.OverlapsWith)
                                           .ForEach(brick.AddSupport);

                if (!brick.SupportedBy.IsEmpty) break;

                brick.MoveDown();
            }

            brickTops[brick.Top].Add(brick);
        }

        Brick[] notSafe = this.Data.Where(b => !b.SafeToDisintegrate()).ToArray();
        AoCUtils.LogPart1(this.Data.Length - notSafe.Length);

        int total = 0;
        HashSet<Brick> collapsed = [];
        Queue<Brick> toCheck     = [];
        foreach (Brick brick in notSafe)
        {
            collapsed.Add(brick);
            brick.Supports.ForEach(toCheck.Enqueue);
            while (toCheck.TryDequeue(out Brick? supported))
            {
                if (supported.SupportedBy.Count(collapsed.Contains) == supported.SupportedBy.Count)
                {
                    collapsed.Add(supported);
                    supported.Supports.ForEach(toCheck.Enqueue);
                }
            }

            total += collapsed.Count - 1;
            collapsed.Clear();
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc />
    protected override Brick ConvertLine(string line) => new(line);
}
