using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Numbers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 7
/// </summary>
[Solver(2021, 7)]
public sealed class Day07 : Solver<int[]>
{
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver for 2021 - 07 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day07(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get maximum crab value
        int max = this.Data.Max();
        // Minimize distance to any point within the crabs

        long best = (..^max).Min(position => this.Data.Sum(crab => Math.Abs(position - crab)));
        LogAnswer(best);

        // Minimize the distance of triangular value
        best = (..^max).Min(position => this.Data.Sum(crab => Math.Abs(position - crab).Triangular));
        LogAnswer(best);
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split(',').ConvertAll(int.Parse);
}
