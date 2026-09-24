using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 1
/// </summary>
[Solver(2021, 1)]
public sealed class Day01 : ArraySolver<int>
{
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

        LogAnswer(total);

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

        LogAnswer(total);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
