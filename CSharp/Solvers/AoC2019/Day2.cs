using System;
using System.IO;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 2
    /// </summary>
    public class Day2 : Solver<IntcodeVM>
    {
        #region Constants
        /// <summary>
        /// Target to find using the Intcode VM
        /// </summary>
        private const int TARGET = 19690720;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day2"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day2(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            int result = this.Input.Run(12, 2);
            AoCUtils.LogPart1(result);

            foreach (int noun in ..100)
            {
                foreach (int verb in ..100)
                {
                    this.Input.Reset();
                    result = this.Input.Run(noun, verb);
                    if (result is TARGET)
                    {
                        AoCUtils.LogPart2((100 * noun) + verb);
                        return;
                    }
                }
            }
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override IntcodeVM Convert(string[] rawInput) => new(rawInput[0]);
        #endregion
    }
}
