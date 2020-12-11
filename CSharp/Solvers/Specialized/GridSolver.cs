using System;
using System.Diagnostics.Contracts;
using AdventOfCode.Grids;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers.Specialized
{
    /// <summary>
    /// Grid Solver base
    /// </summary>
    public abstract class GridSolver<T> : Solver<Grid<T>>
    {
        #region Properties
        /// <summary>
        /// Input Grid
        /// </summary>
        public Grid<T> Grid => this.Data;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="GridSolver{T}"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <param name="splitters">Splitting characters, defaults to newline only</param>
        /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Grid{T}"/> fails</exception>
        protected GridSolver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(input, splitters, options) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override Grid<T> Convert(string[] rawInput)
        {
            int width = rawInput[0].Length;
            int height = rawInput.Length;
            return new Grid<T>(width, height, rawInput, LineConverter);
        }

        /// <summary>
        /// Converts an input line into a Grid row array<br/>
        /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
        /// </summary>
        /// <param name="line">Line to convert</param>
        /// <returns>The created row</returns>
        [Pure]
        protected abstract T[] LineConverter(string line);
        #endregion
    }
}
