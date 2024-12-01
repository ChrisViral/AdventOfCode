using System;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Specialized;

/// <summary>
/// IntcodeVM solver base
/// </summary>
[PublicAPI]
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
    /// <param name="input">Puzzle input</param>
    /// <param name="splitters">Splitting characters, defaults to newline only</param>
    /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    protected IntcodeSolver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(input, splitters, options) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override IntcodeVM Convert(string[] rawInput) => new (rawInput[0]);
    #endregion
}