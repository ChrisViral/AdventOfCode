using System;
using System.IO;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 5
    /// </summary>
    public class Day5 : IntcodeSolver
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day5"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day5(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int[] input = { 1 };
            this.VM.SetInput(input);
            this.VM.Run();
            AoCUtils.LogPart1(string.Join(' ', this.Data.GetOutput()));
            
            this.VM.Reset();
            input[0] = 5;
            this.VM.SetInput(input);
            this.VM.Run();
            AoCUtils.LogPart2(this.Data.GetNextOutput());
        }
        #endregion
    }
}
