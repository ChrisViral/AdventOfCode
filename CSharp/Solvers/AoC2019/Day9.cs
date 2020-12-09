using System;
using System.IO;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 9
    /// </summary>
    public class Day9 : IntcodeSolver
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day9"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day9(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            this.VM.AddInput(1L);
            this.VM.Run();
            AoCUtils.LogPart1(this.VM.GetNextOutput());
            
            this.VM.Reset();
            this.VM.AddInput(2L);
            this.VM.Run();
            AoCUtils.LogPart2(this.VM.GetNextOutput());
        }
        #endregion
    }
}
