using System;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 22
    /// </summary>
    public class Day22 : ArraySolver<long>
    {
        /// <summary>
        /// Daily secret generation count
        /// </summary>
        private const int GEN_COUNT = 2000;

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day22(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            (int[] prices, byte[] diffs, long secret)[] secrets = this.Data.Select(GeneratePrices).ToArray();
            AoCUtils.LogPart1(secrets.Sum(s => s.secret));

            // It ain't efficient, but it takes less than 30s and I cba
            int bestBananas = 0;
            Span<byte> search = stackalloc byte[4];
            foreach (Vector2<sbyte> a in Vector2<sbyte>.Enumerate(19, 19))
            {
                search[0] = (byte)a.X;
                search[1] = (byte)a.Y;
                foreach (Vector2<sbyte> b in Vector2<sbyte>.Enumerate(19, 19))
                {
                    search[2] = (byte)b.X;
                    search[3] = (byte)b.Y;
                    int bananas = 0;
                    foreach ((int[] prices, byte[] diffs, _) in secrets)
                    {
                        ReadOnlySpan<byte> diffSpan = diffs.AsSpan(1);
                        int index = diffSpan.IndexOf(search);
                        if (index is -1) continue;

                        bananas += prices[index + 4];
                    }

                    bestBananas = Math.Max(bestBananas, bananas);
                }
            }

            AoCUtils.LogPart2(bestBananas);
        }

        private static (int[] prices, byte[] diffs, long secret) GeneratePrices(long seed)
        {
            // Create generation arrays
            int[] prices  = new int[GEN_COUNT + 1];
            byte[] diffs = new byte[GEN_COUNT + 1];

            int previousPrice = (int)(seed % 10);
            prices[0]   = previousPrice;
            long secret = seed;
            foreach (int i in 1..^GEN_COUNT)
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

        /// <inheritdoc />
        protected override long ConvertLine(string line) => long.Parse(line);
        #endregion
    }
}

