using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 4
/// </summary>
[Solver(2017, 4)]
public sealed class Day04 : ArraySolver<string[]>
{

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
