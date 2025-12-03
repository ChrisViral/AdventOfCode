using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 12
/// </summary>
public class Day12 : Solver<Day12.Moon[]>
{
    /// <summary>
    /// Different simulation axes
    /// </summary>
    [Flags]
    public enum Axis
    {
        NONE = 0b000,
        X    = 0b001,
        Y    = 0b010,
        Z    = 0b100,
        ALL  = 0b111
    }

    /// <summary>
    /// Moon object
    /// </summary>
    public class Moon
    {
        /// <summary>
        /// Moon state
        /// </summary>
        public readonly struct State : IEquatable<State>
        {
                    private readonly Vector3<int> position;
            private readonly Vector3<int> velocity;
        
                    /// <summary>
            /// Creates a new State for the given Moon
            /// </summary>
            /// <param name="moon">Moon to create the state for</param>
            private State(Moon moon)
            {
                this.position = moon.position;
                this.velocity = moon.velocity;
            }
        
                    /// <inheritdoc cref="object.Equals(object)"/>
            public override bool Equals(object? obj) => obj is State state && Equals(state);

            /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
            public bool Equals(State other) => this.position == other.position && this.velocity == other.velocity;

            /// <inheritdoc cref="object.GetHashCode"/>
            public override int GetHashCode() => HashCode.Combine(this.position, this.velocity);
        
                    /// <summary>
            /// Automatically creates a State from a given Moon
            /// </summary>
            /// <param name="moon">Moon to create the state for</param>
            public static implicit operator State(Moon moon) => new(moon);

            /// <summary>
            /// Equality between two states
            /// </summary>
            /// <param name="a">First state</param>
            /// <param name="b">Second state</param>
            /// <returns>True if both states are equal, false otherwise</returns>
            public static bool operator ==(State a, State b) => a.Equals(b);

            /// <summary>
            /// Inequality between two states
            /// </summary>
            /// <param name="a">First state</param>
            /// <param name="b">Second state</param>
            /// <returns>True if both states are unequal, false otherwise</returns>
            public static bool operator !=(State a, State b) => !a.Equals(b);
                }

            /// <summary>
        /// Moon match pattern
        /// </summary>
        public const string PATTERN = @"<x=(-?\d+), y=(-?\d+), z=(-?\d+)>";
    
            private Vector3<int> position;
        private Vector3<int> velocity = Vector3<int>.Zero;
    
            /// <summary>
        /// Gets the total energy of the Moon
        /// </summary>
        public int Energy
        {
            get
            {
                (int px, int py, int pz) = Vector3<int>.Abs(this.position);
                (int vx, int vy, int vz) = Vector3<int>.Abs(this.velocity);
                return (px + py + pz) * (vx + vy + vz);
            }
        }
    
            /// <summary>
        /// Creates a new Moon at the given position
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="z">Z position</param>
        public Moon(int x, int y, int z) => this.position = new Vector3<int>(x, y, z);

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other">Moon to copy from</param>
        public Moon(Moon other)
        {
            this.position = other.position;
            this.velocity = other.velocity;
        }
    
            /// <summary>
        /// Updates the position of the Moon from it's velocity
        /// </summary>
        public void UpdatePosition() => this.position += this.velocity;

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"pos={this.position},".PadRight(18, ' ') + $"vel={this.velocity}";
    
        /// <summary>
        /// Applies gravity between two moons
        /// </summary>
        /// <param name="a">First moon</param>
        /// <param name="b">Second moon</param>
        /// <param name="axes">Simulation axes to update in</param>
        public static void ApplyGravity(Moon a, Moon b, Axis axes = Axis.ALL)
        {
            (int x, int y, int z) va  = a.velocity;
            (int x, int y, int z) vb = b.velocity;

            if ((axes & Axis.X) is not Axis.NONE)
            {
                if (a.position.X < b.position.X)
                {
                    va.x++;
                    vb.x--;
                }
                else if (a.position.X > b.position.X)
                {
                    va.x--;
                    vb.x++;
                }
            }

            if ((axes & Axis.Y) is not Axis.NONE)
            {
                if (a.position.Y < b.position.Y)
                {
                    va.y++;
                    vb.y--;
                }
                else if (a.position.Y > b.position.Y)
                {
                    va.y--;
                    vb.y++;
                }
            }

            if ((axes & Axis.Z) is not Axis.NONE)
            {
                if (a.position.Z < b.position.Z)
                {
                    va.z++;
                    vb.z--;
                }
                else if (a.position.Z > b.position.Z)
                {
                    va.z--;
                    vb.z++;
                }

            }

            a.velocity = va;
            b.velocity = vb;
        }
        }

    /// <summary>
    /// Iterations of the simulation
    /// </summary>
    private const int ITERATIONS = 1000;
    /// <summary>
    /// Amount of Moons simulated
    /// </summary>
    private const int MOONS = 4;

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Moon"/>[] fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Moon[] moons = CopyMoons();
        foreach (int _ in ..ITERATIONS)
        {
            UpdateMoons(moons);
        }
        AoCUtils.LogPart1(moons.Sum(m => m.Energy));

        long x = CheckAxis(Axis.X);
        long y = CheckAxis(Axis.Y);
        long z = CheckAxis(Axis.Z);

        AoCUtils.LogPart2(long.LCM(x, y, z));
    }

    /// <summary>
    /// Applies gravity and updates the position of each Moon
    /// </summary>
    /// <param name="moons">Moons to update</param>
    /// <param name="axes">Axes to update on</param>
    /// ReSharper disable once SuggestBaseTypeForParameter
    private static void UpdateMoons(Moon[] moons, Axis axes = Axis.ALL)
    {
        for (int i = 0; i < MOONS - 1; /*i++*/)
        {
            Moon a = moons[i++];
            foreach (Moon b in moons[i..])
            {
                Moon.ApplyGravity(a, b, axes);
            }
        }
        moons.ForEach(m => m.UpdatePosition());
    }

    /// <summary>
    /// Creates a deep copy of the Moons
    /// </summary>
    /// <returns>Copy of the Moons</returns>
    private Moon[] CopyMoons() => this.Data.Select(m => new Moon(m)).ToArray();

    /// <summary>
    /// Checks for recurrence on a specific axis
    /// </summary>
    /// <param name="axis">Axis to check for recurrence on</param>
    /// <returns>Steps taken for recurrence to happen</returns>
    private long CheckAxis(Axis axis)
    {
        Moon[] moons = CopyMoons();
        HashSet<(Moon.State, Moon.State, Moon.State, Moon.State)> states = [GetStates(moons)];

        long steps = 0L;
        do
        {
            UpdateMoons(moons, axis);
            steps++;
        }
        while (states.Add(GetStates(moons)));

        return steps;
    }

    /// <summary>
    /// Gets a tuple of the Moons' states
    /// </summary>
    /// <param name="moons">Moons to get the states for</param>
    /// <returns>Tuple of the state of each moon</returns>
    private static (Moon.State, Moon.State, Moon.State, Moon.State) GetStates(IReadOnlyList<Moon> moons) => (moons[0], moons[1], moons[2], moons[3]);

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Moon[] Convert(string[] rawInput) => RegexFactory<Moon>.ConstructObjects(Moon.PATTERN, rawInput);
}
