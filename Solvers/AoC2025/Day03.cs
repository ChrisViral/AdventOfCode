using System;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2025;

/// <summary>
/// Solver for 2025 Day 03
/// </summary>
public sealed class Day03 : Solver
{
    private const char BEST_BATTERY = '9';
    private const int PART1_COUNT   = 2;
    private const int PART2_COUNT   = 12;

    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long pow1 = (PART1_COUNT - 1).LongPow10;
        long joltage = this.Data.Sum(b => GetMaxJoltage(b, PART1_COUNT, pow1));
        AoCUtils.LogPart1(joltage);

        long pow2 = (PART2_COUNT - 1).LongPow10;
        joltage = this.Data.Sum(b => GetMaxJoltage(b, PART2_COUNT, pow2));
        AoCUtils.LogPart2(joltage);
    }

    private static long GetMaxJoltage(ReadOnlySpan<char> bank, int count, long pow)
    {
        long joltage = 0L;
        while (count --> 1)
        {
            (char best, int index) = GetBestBattery(bank[..^count]);
            joltage += (best - '0') * pow;

            pow /= 10L;
            bank = bank[(index + 1)..];
        }
        joltage += GetBestBattery(bank).battery - '0';

        return joltage;
    }

    private static (char battery, int index) GetBestBattery(ReadOnlySpan<char> bank)
    {
        if (bank.Length is 1) return (bank[0], 0);

        char best = bank[0];
        if (best is BEST_BATTERY) return (best, 0);

        int index = 0;
        foreach (int i in 1..bank.Length)
        {
            char battery = bank[i];
            if (battery is BEST_BATTERY) return (battery, i);

            if (battery > best)
            {
                best  = battery;
                index = i;
            }
        }

        return (best, index);
    }
}
