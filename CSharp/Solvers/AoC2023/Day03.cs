using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023
{
    /// <summary>
    /// Solver for 2023 Day 03
    /// </summary>
    public class Day03 : GridSolver<char>
    {
        private const char GEAR = '*';

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="char"/> fails</exception>
        public Day03(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            Dictionary<Vector2<int>, List<int>> gears = new();

            int total = 0;
            HashSet<Vector2<int>> explored = new();
            foreach (Vector2<int> pos in Vector2<int>.Enumerate(this.Data.Width, this.Data.Height))
            {
                char value = this.Data[pos];
                if (value is '.' || char.IsNumber(value)) continue;

                foreach (Vector2<int> adjacent in pos.Adjacent(true))
                {
                    if (!this.Data.WithinGrid(adjacent) || explored.Contains(adjacent)) continue;

                    char c = this.Data[adjacent];
                    if (!char.IsNumber(c)) continue;

                    Vector2<int> current;
                    for (current = adjacent + Vector2<int>.Left; this.Data.WithinGrid(current) && char.IsNumber(this.Data[current]); current += Vector2<int>.Left) { }

                    int number = 0;
                    for (current += Vector2<int>.Right; this.Data.WithinGrid(current) && char.IsNumber(this.Data[current]); current += Vector2<int>.Right)
                    {
                        number *= 10;
                        number += this.Data[current] - '0';
                        explored.Add(current);
                    }

                    total += number;

                    if (value is not GEAR) continue;

                    if (!gears.TryGetValue(pos, out List<int>? numbers))
                    {
                        numbers = new();
                        gears.Add(pos, numbers);
                    }

                    numbers.Add(number);
                }
            }

            AoCUtils.LogPart1(total);

            long gearRatio = gears.Values.Where(n => n.Count is 2).Sum(n => (long)n[0] * n[1]);

            AoCUtils.LogPart2(gearRatio);
        }

        /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
        protected override char[] LineConverter(string line) => line.ToCharArray();

        #endregion
    }
}