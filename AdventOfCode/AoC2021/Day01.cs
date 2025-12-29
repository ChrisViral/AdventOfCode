using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 01
/// </summary>
public sealed class Day01 : ArraySolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver for 2021 - 01 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day01(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Check the one window differences
        int total = 0;
        foreach (int i in 1..this.Data.Length)
        {
            if (this.Data[i] > this.Data[i - 1])
            {
                total++;
            }
        }

        AoCUtils.LogPart1(total);

        // Check the three window differences
        total = 0;
        int previous = this.Data[..3].Sum();
        foreach (int i in 3..this.Data.Length)
        {
            int current = previous + this.Data[i] - this.Data[i - 3];
            if (current > previous)
            {
                total++;
            }

            previous = current;
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
