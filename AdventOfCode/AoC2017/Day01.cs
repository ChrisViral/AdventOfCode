using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 01
/// </summary>
public sealed class Day01 : Solver<int[]>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int length = this.Data.Length;
        int matches = this.Data
                          .Where((v, i) => v == this.Data[(i + 1) % length])
                          .Sum();
        AoCUtils.LogPart1(matches);

        int half = length / 2;
        matches = this.Data
                      .Where((v, i) => v == this.Data[(i + half) % length])
                      .Sum();
        AoCUtils.LogPart2(matches);
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].Select(c => c - '0').ToArray();
}
