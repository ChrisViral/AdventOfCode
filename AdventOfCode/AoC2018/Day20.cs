using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 20
/// </summary>
public sealed class Day20 : Solver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1("");
        AoCUtils.LogPart2("");
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput)
    {
        return string.Empty;
    }
}
