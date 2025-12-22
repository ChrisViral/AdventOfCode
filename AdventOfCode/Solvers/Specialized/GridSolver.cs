using AdventOfCode.Collections;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Ranges;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Specialized;

/// <summary>
/// Grid Solver base
/// </summary>
[PublicAPI]
public abstract class GridSolver<T> : Solver<Grid<T>>
{
    /// <summary>
    /// Grid parse mode
    /// </summary>
    protected enum GridParseMode
    {
        /// <summary>
        /// Populate the grid from the raw input string
        /// </summary>
        POPULATE,
        /// <summary>
        /// Convert input lines and create to grid
        /// </summary>
        CONVERT
    }

    /// <summary>
    /// Grid parsing mode, default to Populate
    /// </summary>
    protected virtual GridParseMode ParseMode => GridParseMode.POPULATE;

    /// <summary>
    /// Input Grid
    /// </summary>
    protected Grid<T> Grid => this.Data;

    /// <summary>
    /// Creates a new <see cref="GridSolver{T}"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="splitters">Splitting characters, defaults to newline only</param>
    /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Grid{T}"/> fails</exception>
    protected GridSolver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(input, splitters, options) { }

    /// <inheritdoc />
    protected sealed override Grid<T> Convert(string[] rawInput)
    {
        int height = rawInput.Length;
        switch (this.ParseMode)
        {
            case GridParseMode.POPULATE:
                int width = rawInput[0].Length;
                return new Grid<T>(width, height, rawInput, LineConverter, StringConversion);

            case GridParseMode.CONVERT:
                T[] line = LineConverter(rawInput[0]);
                width = line.Length;
                Grid<T> grid = new(width, height, StringConversion);
                grid.SetRow(0, line);
                foreach (int y in 1..height)
                {
                    grid.SetRow(y, LineConverter(rawInput[y]));
                }

                return grid;

            default:
                throw this.ParseMode.Invalid();
        }
    }

    /// <summary>
    /// Converts an input line into a Grid row array<br/>
    /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
    /// </summary>
    /// <param name="line">Line to convert</param>
    /// <returns>The created row</returns>
    [Pure]
    protected abstract T[] LineConverter(string line);

    /// <summary>
    /// Perf object string conversion for the Grid's to string method
    /// </summary>
    /// <param name="obj">Object being converted</param>
    /// <returns>A string representation of the object</returns>
    protected virtual string StringConversion(T obj) => obj?.ToString() ?? string.Empty;
}
