using System;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 09
/// </summary>
public class Day09 : ArraySolver<long[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        long total = this.Data.Sum(a => GetNextValue(a));
        AoCUtils.LogPart1(total);

        total = this.Data.Sum(a => GetNextValue(a, true));
        AoCUtils.LogPart2(total);
    }

    public long GetNextValue(in Span<long> values, bool backwards = false)
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

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override long[] ConvertLine(string line) => line.Split(' ', DEFAULT_OPTIONS).ConvertAll(long.Parse);
    #endregion
}