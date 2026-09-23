using System.Numerics;
using Challenge.Solvers;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 19
/// </summary>
[Solver(2016, 19)]
public sealed partial class Day19 : Solver<int>
{

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
        LogAnswer(final);

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
        LogAnswer(final);
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
