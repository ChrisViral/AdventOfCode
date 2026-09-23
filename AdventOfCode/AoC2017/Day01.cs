using Challenge.Solvers;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 1
/// </summary>
[Solver(2017, 1)]
public sealed partial class Day01 : Solver<int[]>
{
    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int length = this.Data.Length;
        int matches = this.Data
                          .Where((v, i) => v == this.Data[(i + 1) % length])
                          .Sum();
        LogAnswer(matches);

        int half = length / 2;
        matches = this.Data
                      .Where((v, i) => v == this.Data[(i + half) % length])
                      .Sum();
        LogAnswer(matches);
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].Select(c => c - '0').ToArray();
}
