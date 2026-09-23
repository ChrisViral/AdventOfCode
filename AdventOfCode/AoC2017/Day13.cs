using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Numbers;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 13
/// </summary>
[Solver(2017, 13)]
public sealed partial class Day13 : RegexSolver<Day13.Firewall>
{
    public readonly record struct Firewall(int Depth, int Range)
    {
        public int Period { get; } = (Range - 1) * 2;
    }

    /// <inheritdoc />
    [GeneratedRegex(@"(\d+): (\d+)")]
    protected override partial Regex Matcher { get; }

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
