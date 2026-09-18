using System.Text.RegularExpressions;
using Challenge.Utils.Extensions.Numbers;
using Challenge.Solvers.Specialized;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 13
/// </summary>
public sealed partial class Day13 : RegexSolver<Day13.Firewall>
{
    public readonly record struct Firewall(int Depth, int Range)
    {
        public int Period { get; } = (Range - 1) * 2;
    }

    /// <inheritdoc />
    [GeneratedRegex(@"(\d+): (\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int severity = this.Data
                           .Where(f => f.Period.IsFactor(f.Depth))
                           .Sum(f => f.Depth * f.Range);
        LogAnswer(severity);

        int delay = Enumerable.InfiniteSequence(1, 1)
                              .First(d => this.Data.All(f => !f.Period.IsFactor(f.Depth + d)));
        LogAnswer(delay);
    }
}
