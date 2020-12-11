using System;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 1
    /// </summary>
    public class Day01 : Solver<int[]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/> fails</exception>
        public Day01(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            AoCUtils.LogPart1(this.Data.Sum(m => (m / 3) - 2));
            AoCUtils.LogPart2(this.Data.Sum(CalculateFuel));
        }

        /// <summary>
        /// Calculates the fuel necessary for a given mass, accounting for the mass of the fuel
        /// </summary>
        /// <param name="mass">Mass to calculate for</param>
        /// <returns>The total fuel required for the given mass</returns>
        public static int CalculateFuel(int mass)
        {
            mass = (mass / 3) - 2;
            int totalFuel = 0;
            while (mass is > 0)
            {
                totalFuel += mass;
                mass = (mass / 3) - 2;
            }

            return totalFuel;
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override int[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, int.Parse);
        #endregion
    }
}
