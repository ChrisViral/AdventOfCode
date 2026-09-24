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
    /// Creates a new <see cref="IntcodeSolver"/>
    /// </summary>
    protected IntcodeSolver() : base([], StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    protected sealed override IntcodeVM Convert(string[] rawInput) => new(rawInput[0]);
}
