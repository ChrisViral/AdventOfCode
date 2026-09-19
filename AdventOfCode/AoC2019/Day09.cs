using AdventOfCode.AoC2019.Solvers;
using Challenge.Solvers;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 09
/// </summary>
[Solver(2019, 9)]
public sealed class Day09 : IntcodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM.Input.AddValue(1L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetValue());

        this.VM.Reset();
        this.VM.Input.AddValue(2L);
        this.VM.Run();
        LogAnswer(this.VM.Output.GetValue());
    }
}
