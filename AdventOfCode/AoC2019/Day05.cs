using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 05
/// </summary>
public sealed class Day05 : IntcodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM.Input.AddValue(1L);
        this.VM.Run();
        AoCUtils.LogPart1(this.VM.Output.GetAllValues().Last());

        this.VM.Reset();
        this.VM.Input.AddValue(5L);
        this.VM.Run();
        AoCUtils.LogPart2(this.VM.Output.GetValue());
    }
}
