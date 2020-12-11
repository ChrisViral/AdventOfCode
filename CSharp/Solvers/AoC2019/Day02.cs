using System;
using System.IO;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 2
    /// </summary>
    public class Day02 : IntcodeSolver
    {
        #region Constants
        /// <summary>
        /// Target to find using the Intcode VM
        /// </summary>
        private const long TARGET = 19690720L;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day02(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            this.VM.Run(12L, 2L, 0, out long result);
            AoCUtils.LogPart1(result);

            foreach (int noun in ..100)
            {
                foreach (int verb in ..100)
                {
                    this.VM.Reset();
                    this.VM.Run(noun, verb, 0, out result);
                    if (result is TARGET)
                    {
                        AoCUtils.LogPart2((100 * noun) + verb);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
