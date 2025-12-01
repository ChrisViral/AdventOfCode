using System;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2025
{
    /// <summary>
    /// Solver for 2025 Day 01
    /// </summary>
    public class Day01 : ArraySolver<int>
    {
        private const int DIAL_SIZE = 100;
        private const int DIAL_START = 50;

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day01(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int zeroes = 0;
            int dial = DIAL_START;
            foreach (int move in this.Data)
            {
                dial = (dial + move).Mod(DIAL_SIZE);
                if (dial is 0) zeroes++;
            }
            AoCUtils.LogPart1(zeroes);

            zeroes = 0;
            dial = DIAL_START;
            bool wasZero = false;
            foreach (int move in this.Data)
            {
                int rawDial = dial + move;
                dial = rawDial.Mod(DIAL_SIZE);
                switch (rawDial)
                {
                    case 0:
                        zeroes++;
                        break;

                    case < 0 when wasZero:
                        zeroes += -rawDial / DIAL_SIZE;
                        break;

                    case < 0:
                        zeroes += (-rawDial / DIAL_SIZE) + 1;
                        break;

                    case >= DIAL_SIZE:
                        zeroes += rawDial / DIAL_SIZE;
                        break;
                }
                wasZero = dial == 0;
            }
            AoCUtils.LogPart2(zeroes);
        }

        /// <inheritdoc />
        protected override int ConvertLine(string line)
        {
            Direction direction = DirectionsUtils.Parse(line[0]);
            int length = int.Parse(line.AsSpan(1));
            return direction is Direction.LEFT ? -length : length;
        }
        #endregion
    }
}
