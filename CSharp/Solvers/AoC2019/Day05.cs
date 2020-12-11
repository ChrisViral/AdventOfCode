using System;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 05
    /// </summary>
    public class Day05 : IntcodeSolver
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day05(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            this.VM.AddInput(1L);
            this.VM.Run();
            AoCUtils.LogPart1(string.Join(' ', this.Data.GetOutput()));
            
            this.VM.Reset();
            this.VM.AddInput(5L);
            this.VM.Run();
            AoCUtils.LogPart2(this.Data.GetNextOutput());
        }
        #endregion
    }
}
