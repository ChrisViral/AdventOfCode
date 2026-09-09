using System.Numerics;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 19
/// </summary>
public sealed class Day19 : Solver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // This is just Josephus' problem
        // The solution is to represent the number of participants (N) as 2^a + r
        // where a is as large as possible, and then the solution is (2 * r) + 1
        int highestBit = BitOperations.Log2((uint)this.Data);
        int remainder = this.Data - (1 << highestBit);
        int final = (2 * remainder) + 1;
        AoCUtils.LogPart1(final);

        // This is a variation of the Josephus problem,
        // but instead we need the highest power of 3 fitting within N.
        // If N is itself a power of 3, then the answer is N.
        // Otherwise, given the remainder r = N - P, the answer is r + max(0, r - P)

        // Find the largest power of 3
        int p = 1;
        int lowerBound = this.Data / 3;
        while (p <= lowerBound) p *= 3;

        // Calculate winning seat
        remainder = this.Data - p;
        final = remainder is 0 ? this.Data : remainder + Math.Max(0, remainder - p);
        AoCUtils.LogPart2(final);
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
