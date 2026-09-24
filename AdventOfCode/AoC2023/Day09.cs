using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 9
/// </summary>
[Solver(2023, 9)]
public sealed class Day09 : ArraySolver<long[]>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long total = this.Data.Sum(a => GetNextValue(a));
        LogAnswer(total);

        total = this.Data.Sum(a => GetNextValue(a, true));
        LogAnswer(total);
    }

    private static long GetNextValue(in Span<long> values, bool backwards = false)
    {
        Span<long> diff = stackalloc long[values.Length - 1];
        foreach (int i in 1..values.Length)
        {
            diff[i - 1] = values[i] - values[i - 1];
        }

        long first = diff[0];
        bool done = diff.Count(first) == diff.Length;
        return backwards
            ? values[0] - (done ? first : GetNextValue(diff, true))
            : values[^1] + (done ? first : GetNextValue(diff));
    }

    /// <inheritdoc />
    protected override long[] ConvertLine(string line) => line.Split(' ', DEFAULT_OPTIONS).ConvertAll(long.Parse);
}
