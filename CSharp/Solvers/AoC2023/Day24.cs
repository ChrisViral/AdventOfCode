using System;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023
{
    /// <summary>
    /// Solver for 2023 Day 24
    /// </summary>
    public class Day24 : ArraySolver<Day24.Hail>
    {
        public readonly struct Hail
        {
            private readonly long a;
            private readonly long b;
            private readonly long c;

            public Vector3<long> Position { get; }

            public Vector3<long> Velocity { get; }

            public Hail(string data)
            {
                string[] splits = data.Split('@', DEFAULT_OPTIONS);
                this.Position = Vector3<long>.Parse(splits[0]);
                this.Velocity = Vector3<long>.Parse(splits[1]);

                this.a = this.Velocity.Y;
                this.b = -this.Velocity.X;
                this.c = (this.Velocity.X * this.Position.Y) - (this.Velocity.Y * this.Position.X);
            }

            public static bool FindIntersection(in Hail first, in Hail second, out (decimal x, decimal y) result)
            {
                if ((first.Velocity.Y / (decimal)first.Velocity.X) == (second.Velocity.Y / (decimal)second.Velocity.X))
                {
                    // Parallel
                    result = (0m, 0m);
                    return false;
                }

                decimal denominator = (first.a * second.b) - (second.a * first.b);
                decimal x = ((first.b * second.c) - (second.b * first.c)) / denominator;
                decimal t1 = (x - first.Position.X) / first.Velocity.X;
                if (t1 < 0m)
                {
                    // Past first
                    result = (0m, 0m);
                    return false;
                }

                decimal t2 = (x - second.Position.X) / second.Velocity.X;
                if (t2 < 0m)
                {
                    // Past second
                    result = (0m, 0m);
                    return false;
                }

                decimal y = ((first.c * second.a) - (second.c * first.a)) / denominator;
                result = (x, y);
                return true;
            }
        }

        private const long MIN = 7L;
        private const long MAX = 27L;

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Hail"/> fails</exception>
        public Day24(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int collisions = 0;
            foreach (int i in ..(this.Data.Length - 1))
            {
                Hail a = this.Data[i];
                foreach (int j in (i + 1)..this.Data.Length)
                {
                    Hail b = this.Data[j];
                    if (!Hail.FindIntersection(a, b, out (decimal x, decimal y) intersection)) continue;

                    if (intersection.x is >= MIN and <= MAX
                     && intersection.y is >= MIN and <= MAX)
                    {
                        AoCUtils.Log($"{intersection.x:0.00}, {intersection.y:0.00}");
                        collisions++;
                    }
                }
            }

            AoCUtils.LogPart1(collisions);

            AoCUtils.LogPart2("");
        }

        /// <inheritdoc />
        protected override Hail ConvertLine(string line) => new(line);
        #endregion
    }
}

