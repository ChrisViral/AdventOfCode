using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 12
/// </summary>
public sealed class Day12 : Solver<Day12.Moon[]>
{
    /// <summary>
    /// Axis
    /// </summary>
    [Flags]
    public enum Axes
    {
        NONE = 0b000,
        X    = 0b001,
        Y    = 0b010,
        Z    = 0b100,
        ALL  = 0b111
    }

    /// <summary>
    /// Moon's axis state
    /// </summary>
    /// <param name="Position">Position value</param>
    /// <param name="Velocity">Velocity value</param>
    public readonly record struct AxisState(int Position, int Velocity);

    /// <summary>
    /// Moon's system state
    /// </summary>
    /// <param name="A">First moon's axis state</param>
    /// <param name="B">Second moon's axis state</param>
    /// <param name="C">Third moon's axis state</param>
    /// <param name="D">Fourth moon's axis state</param>
    private readonly record struct SystemState(AxisState A, AxisState B, AxisState C, AxisState D);

    /// <summary>
    /// Moon object
    /// </summary>
    /// <param name="px">Starting X position</param>
    /// <param name="py">Starting Y position</param>
    /// <param name="pz">Starting Z position</param>
    [UsedImplicitly]
    public sealed class Moon(int px, int py, int pz)
    {
        private Vector3<int> position = new(px, py, pz);
        public Vector3<int> velocity = Vector3<int>.Zero;

        /// <summary>
        /// Total energy of the moon
        /// </summary>
        public int Energy => this.position.ManhattanLength * this.velocity.ManhattanLength;

        /// <summary>
        /// Applies velocity to the moon's position
        /// </summary>
        public void ApplyVelocity() => this.position += this.velocity;

        /// <summary>
        /// Gets the given axis state for this moon
        /// </summary>
        /// <param name="axes">Axis to get</param>
        /// <returns>The specified Axis state</returns>
        /// <exception cref="InvalidEnumArgumentException">Unknown value of <paramref name="axes"/></exception>
        public AxisState GetCurrentState(Axes axes) => axes switch
        {
            Axes.X => new AxisState(this.position.X, this.velocity.X),
            Axes.Y => new AxisState(this.position.Y, this.velocity.Y),
            Axes.Z => new AxisState(this.position.Z, this.velocity.Z),
            _      => throw new InvalidEnumArgumentException(nameof(axes), (int)axes, typeof(Axes))
        };

        /// <summary>
        /// Applies gravity between two moons
        /// </summary>
        /// <param name="first">First moon</param>
        /// <param name="second">Second moon</param>
        public static void ApplyGravity(Moon first, Moon second)
        {
            int dx = first.position.X.CompareTo(second.position.X);
            int dy = first.position.Y.CompareTo(second.position.Y);
            int dz = first.position.Z.CompareTo(second.position.Z);
            Vector3<int> delta = (dx, dy, dz);
            first.velocity  -= delta;
            second.velocity += delta;
        }

        /// <summary>
        /// Applies gravity between two moons
        /// </summary>
        /// <param name="first">First moon</param>
        /// <param name="second">Second moon</param>
        /// <param name="axes">Axes to apply</param>
        public static void ApplyGravity(Moon first, Moon second, Axes axes)
        {
            int dx = (axes & Axes.X) is not 0 ? first.position.X.CompareTo(second.position.X) : 0;
            int dy = (axes & Axes.Y) is not 0 ? first.position.Y.CompareTo(second.position.Y) : 0;
            int dz = (axes & Axes.Z) is not 0 ? first.position.Z.CompareTo(second.position.Z) : 0;
            Vector3<int> delta = (dx, dy, dz);
            first.velocity  -= delta;
            second.velocity += delta;
        }

        /// <inheritdoc />
        public override string ToString() => $"<p={this.position}, v={this.velocity}>";
    }

    /// <summary>
    /// Moon position parse pattern
    /// </summary>
    private const string MOON_PATTERN = @"<x=(-?\d+), y=(-?\d+), z=(-?\d+)>";
    /// <summary>
    /// Part 1 steps
    /// </summary>
    private const int STEPS = 1000;

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<SystemState> xStates = new(1000);
        HashSet<SystemState> yStates = new(1000);
        HashSet<SystemState> zStates = new(1000);
        foreach (int _ in ..STEPS)
        {
            // Apply gravity between all pairs
            foreach (int i in ..(this.Data.Length - 1))
            {
                Moon first = this.Data[i];
                foreach (Moon second in this.Data.AsSpan(i + 1))
                {
                    Moon.ApplyGravity(first, second);
                }
            }

            // Apply velocity
            this.Data.ForEach(m => m.ApplyVelocity());
            xStates.Add(GetSystemState(Axes.X));
            yStates.Add(GetSystemState(Axes.Y));
            zStates.Add(GetSystemState(Axes.Z));
        }

        AoCUtils.LogPart1(this.Data.Sum(m => m.Energy));

        Axes axes = Axes.ALL;
        do
        {
            // Apply gravity between all pairs
            foreach (int i in ..(this.Data.Length - 1))
            {
                Moon first = this.Data[i];
                foreach (Moon second in this.Data.AsSpan(i + 1))
                {
                    Moon.ApplyGravity(first, second, axes);
                }
            }

            // Apply velocity
            this.Data.ForEach(m => m.ApplyVelocity());

            // Check each state
            if ((axes & Axes.X) is not 0 && !xStates.Add(GetSystemState(Axes.X)))
            {
                axes ^= Axes.X;
                this.Data.ForEach(m => m.velocity = m.velocity with { X = 0 });
            }
            if ((axes & Axes.Y) is not 0 && !yStates.Add(GetSystemState(Axes.Y)))
            {
                axes ^= Axes.Y;
                this.Data.ForEach(m => m.velocity = m.velocity with { Y = 0 });
            }
            if ((axes & Axes.Z) is not 0 && !zStates.Add(GetSystemState(Axes.Z)))
            {
                axes ^= Axes.Z;
                this.Data.ForEach(m => m.velocity = m.velocity with { Z = 0 });
            }
        }
        while (axes is not Axes.NONE);

        long repeatTime = long.LCM(xStates.Count, yStates.Count, zStates.Count);
        AoCUtils.LogPart2(repeatTime);
    }

    /// <summary>
    /// Gets the system state for the specified axis
    /// </summary>
    /// <param name="axes">Axis to get the state for</param>
    /// <returns>The current system state on the specified axis</returns>
    private SystemState GetSystemState(Axes axes) => new(this.Data[0].GetCurrentState(axes),
                                                         this.Data[1].GetCurrentState(axes),
                                                         this.Data[2].GetCurrentState(axes),
                                                         this.Data[3].GetCurrentState(axes));

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Moon[] Convert(string[] rawInput) => RegexFactory<Moon>.ConstructObjects(MOON_PATTERN, rawInput, RegexOptions.Compiled);
}
