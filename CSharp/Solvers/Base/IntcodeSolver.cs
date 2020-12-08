using System;
using System.IO;
using AdventOfCode.Intcode;

namespace AdventOfCode.Solvers.Base
{
    /// <summary>
    /// IntcodeVm solver base
    /// </summary>
    public abstract class IntcodeSolver : Solver<IntcodeVM>
    {
        #region Properties
        /// <summary>
        /// Intcode VM for this solver
        /// </summary>
        protected IntcodeVM VM => this.Data;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="IntcodeSolver"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <param name="splitters">Splitting characters, defaults to newline only</param>
        /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        protected IntcodeSolver(FileInfo file, char[]? splitters = null, StringSplitOptions options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries) : base(file, splitters, options) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver{T}.Convert"/>
        public override IntcodeVM Convert(string[] rawInput) => new (rawInput[0]);
        #endregion
    }
}
