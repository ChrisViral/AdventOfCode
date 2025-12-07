using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 22
/// </summary>
public sealed class Day22 : ArraySolver<long>
{
    private sealed class ParallelHelper((int[] prices, byte[] diffs)[] secrets) : ParallelHelper<(byte a, byte b, byte c, byte d), Ref<int>>
    {
        public ConcurrentBag<int> Results { get; } = [];

        /// <inheritdoc />
        protected override Ref<int> Setup() => new(0);

        /// <inheritdoc />
        protected override void Process((byte a, byte b, byte c, byte d) element, in IterationData iteration)
        {
            int bananas = 0;
            Span<byte> search = [element.a, element.b, element.c, element.d];
            foreach ((int[] prices, byte[] diffs) in secrets)
            {
                int index = diffs.AsSpan().IndexOf(search);
                if (index is -1) continue;

                bananas += prices[index + 3];
            }
            iteration.data.Value = Math.Max(iteration.data, bananas);
        }

        /// <inheritdoc />
        protected override void Finalize(Ref<int> data) => Results.Add(data);
    }

    /// <summary>
    /// Daily secret generation count
    /// </summary>
    private const int GEN_COUNT = 2000;

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        (int[] prices, byte[] diffs, long secret)[] data = this.Data.Select(GeneratePrices).ToArray();
        AoCUtils.LogPart1(data.Sum(s => s.secret));

        ParallelHelper helper = new(data.Select(d => (d.prices, d.diffs)).ToArray());
        helper.ForEach(EnumerateSequences());
        int maxBananas = helper.Results.Max();
        AoCUtils.LogPart2(maxBananas);
    }

    private static (int[] prices, byte[] diffs, long secret) GeneratePrices(long seed)
    {
        // Create generation arrays
        int[] prices  = new int[GEN_COUNT];
        byte[] diffs = new byte[GEN_COUNT];

        int previousPrice = (int)(seed % 10);
        long secret = seed;
        foreach (int i in ..GEN_COUNT)
        {
            secret ^= secret << 6;  // * 64
            secret &= 0xFFFFFF;     // % 16777216
            secret ^= secret >> 5;  // / 32
            secret &= 0xFFFFFF;     // % 16777216
            secret ^= secret << 11; // * 2048
            secret &= 0xFFFFFF;     // % 16777216

            int price     = (int)(secret % 10L);
            prices[i]     = price;
            diffs[i]      = (byte)(price - previousPrice + 9);
            previousPrice = price;
        }

        return (prices, diffs, secret);
    }

    private static IEnumerable<(byte, byte, byte, byte)> EnumerateSequences()
    {
        for (byte a = 0; a <= 18; a++)
        {
            for (byte b = 0; b <= 18; b++)
            {
                for (byte c = 0; c <= 18; c++)
                {
                    for (byte d = 0; d <= 18; d++)
                    {
                        yield return (a, b, c, d);
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    protected override long ConvertLine(string line) => long.Parse(line);
}
