using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 20
/// </summary>
public sealed partial class Day20 : RegexSolver<Day20.Particle>
{
    [DebuggerDisplay("Pos: {Position}, Vel: {Velocity}, Acc: {Acceleration}")]
    public sealed class Particle(Vector3<int> position, Vector3<int> velocity, Vector3<int> acceleration)
    {
        public Vector3<int> Position { get; private set; } = position;

        public Vector3<int> Velocity { get; private set; } = velocity;

        public Vector3<int> Acceleration { get; } = acceleration;

        public bool IsDestroyed { get; set; }

        public Particle(Particle other) : this(other.Position, other.Velocity, other.Acceleration) { }

        public void Step()
        {
            this.Velocity += this.Acceleration;
            this.Position += this.Velocity;
        }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"p=<(-?\d+,-?\d+,-?\d+)>, v=<(-?\d+,-?\d+,-?\d+)>, a=<(-?\d+,-?\d+,-?\d+)>")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Dictionary<Vector3<int>, Particle> currentPositions = new(this.Data.Length);
        foreach (int _ in ..500)
        {
            this.Data.ForEach(p => p.Step());

            foreach (Particle particle in this.Data.Where(p => !p.IsDestroyed))
            {
                if (currentPositions.TryGetValue(particle.Position, out Particle? colliding))
                {
                    particle.IsDestroyed  = true;
                    colliding.IsDestroyed = true;
                }
                else
                {
                    currentPositions[particle.Position] = particle;
                }
            }
            currentPositions.Clear();
        }

        Particle closest = this.Data.MinBy(p => p.Position.ManhattanLength)!;
        int id = this.Data.IndexOf(closest);
        AoCUtils.LogPart1(id);

        int remaining = this.Data.Count(p => !p.IsDestroyed);
        AoCUtils.LogPart2(remaining);
    }
}
