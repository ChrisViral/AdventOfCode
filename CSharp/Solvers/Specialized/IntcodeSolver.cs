using System;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Specialized;

/// <summary>
/// Intcode problem solver base
/// </summary>
[PublicAPI]
public abstract class IntcodeSolver : Solver<IntcodeVM>
{
    #region Properties
    /// <summary>
    /// VM instance
    /// </summary>
    public IntcodeVM VM => this.Data;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="IntcodeSolver"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    protected IntcodeSolver(string input) : base(input, [], StringSplitOptions.TrimEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override IntcodeVM Convert(string[] rawInput) => new(rawInput[0]);
    #endregion
}