using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 04
/// </summary>
public sealed class Day04 : ArraySolver<string[]>
{
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int valid = this.Data.Count(p => p.Distinct()
                                          .Count() == p.Length);
        AoCUtils.LogPart1(valid);

        valid = this.Data.Count(p => p.Select(w => w.Order())
                                      .Distinct(SequenceComparer<char>.Instance)
                                      .Count() == p.Length);
        AoCUtils.LogPart2(valid);
    }

    /// <inheritdoc />
    protected override string[] ConvertLine(string line) => line.Split(' ');
}
