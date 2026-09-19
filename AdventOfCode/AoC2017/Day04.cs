using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 04
/// </summary>
[Solver(2017, 4)]
public sealed class Day04 : ArraySolver<string[]>
{
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int valid = this.Data.Count(p => p.Distinct()
                                          .Count() == p.Length);
        LogAnswer(valid);

        valid = this.Data.Count(p => p.Select(w => w.AsEnumerable().Order())
                                      .Distinct(SequenceComparer<char>.Instance)
                                      .Count() == p.Length);
        LogAnswer(valid);
    }

    /// <inheritdoc />
    protected override string[] ConvertLine(string line) => line.Split(' ');
}
