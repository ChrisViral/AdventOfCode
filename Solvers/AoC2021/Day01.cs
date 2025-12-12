using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

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

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Check the one window differences
        int total = 0;
        foreach (int i in 1..Data.Length)
        {
            if (Data[i] > Data[i - 1])
            {
                total++;
            }
        }

        AoCUtils.LogPart1(total);

        // Check the three window differences
        total = 0;
        int previous = Data[..3].Sum();
        foreach (int i in 3..Data.Length)
        {
            int current = previous + Data[i] - Data[i - 3];
            if (current > previous)
            {
                total++;
            }

            previous = current;
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int ConvertLine(string line) => int.Parse(line);
}
