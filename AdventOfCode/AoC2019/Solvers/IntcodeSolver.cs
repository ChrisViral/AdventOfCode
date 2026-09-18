using AdventOfCode.Intcode;
using Challenge.Solvers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019.Solvers;

/// <summary>
/// Intcode problem solver base
/// </summary>
[PublicAPI]
public abstract class IntcodeSolver : Solver<IntcodeVM>
{
    /// <summary>
    /// VM instance
    /// </summary>
    public IntcodeVM VM => this.Data;

    /// <summary>
    /// Creates a new <see cref="IntcodeSolver"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    protected IntcodeSolver(string input, ILogger logger) : base(input, logger, [], StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    protected sealed override IntcodeVM Convert(string[] rawInput) => new(rawInput[0]);
}
