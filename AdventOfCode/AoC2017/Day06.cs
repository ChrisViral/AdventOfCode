using System.Runtime.CompilerServices;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Enumerables;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 6
/// </summary>
[Solver(2017, 6)]
public sealed class Day06 : Solver<int[]>
{
    [InlineArray(SIZE)]
    private struct MemoryBanks : IEquatable<MemoryBanks>
    {
        private int element;

        /// <inheritdoc />
        public bool Equals(MemoryBanks other) => ((ReadOnlySpan<int>)this).SequenceEqual(other);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is MemoryBanks other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => ((ReadOnlySpan<int>)this).Aggregate(HashCode.Combine);
    }

    private const int SIZE = 16;

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day06(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        MemoryBanks memoryBanks = new();
        this.Data.CopyTo(memoryBanks);

        int steps = 0;
        Dictionary<MemoryBanks, int> states = new(100);
        while (states.TryAdd(memoryBanks, steps))
        {
            // Get value to distribute
            int maxIndex = Enumerable.Range(0, SIZE).MaxBy(i => memoryBanks[i]);
            int maxValue = memoryBanks[maxIndex];

            // Empty bank to distribute
            memoryBanks[maxIndex] = 0;

            // Calculate how much is spread to every bank and how much is left
            (int spread, int remainder) = Math.DivRem(maxValue, SIZE);

            // Add to all banks
            if (spread > 0)
            {
                Enumerable.Range(0, SIZE).ForEach(i => memoryBanks[i] += spread);
            }

            for (int n = 1; n <= remainder; n++)
            {
                int index = (maxIndex + n) % SIZE;
                memoryBanks[index]++;
            }

            steps++;
        }
        LogAnswer(steps);

        int lastSeen = states[memoryBanks];
        LogAnswer(steps - lastSeen);
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split('\t').ConvertAll(int.Parse);
}
