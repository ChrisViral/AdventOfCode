using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;

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
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int severity = this.Data
                           .Where(f => f.Period.IsFactor(f.Depth))
                           .Sum(f => f.Depth * f.Range);
        AoCUtils.LogPart1(severity);

        int delay = Enumerable.InfiniteSequence(1, 1)
                              .First(d => this.Data.All(f => !f.Period.IsFactor(f.Depth + d)));
        AoCUtils.LogPart2(delay);
    }
}
