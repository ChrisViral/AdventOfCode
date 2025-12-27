using System.Runtime.CompilerServices;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 15
/// </summary>
public sealed class Day15 : Solver<(Day15.Generator A, Day15.Generator B)>
{
    public sealed class Generator(int seed, int factor, int multiple)
    {
        private const long MOD = 2147483647L;

        private readonly int seed     = seed;
        private readonly int factor   = factor;
        private readonly int multiple = multiple;
        private int value = seed;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Next() => this.value = (int)(Math.BigMul(this.value, this.factor) % MOD);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextPicky()
        {
            do
            {
                this.value = (int)(Math.BigMul(this.value, this.factor) % MOD);
            }
            while (!this.value.IsMultiple(this.multiple));
            return this.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => this.value = this.seed;
    }

    private const int FACTOR_A    = 16807;
    private const int FACTOR_B    = 48271;
    private const int MULTIPLE_A  = 4;
    private const int MULTIPLE_B  = 8;
    private const int PART1_PAIRS = 40_000_000;
    private const int PART2_PAIRS = 5_000_000;
    private const int MASK        = 0b11111111_11111111;

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int matches = 0;
        foreach (int _ in ..PART1_PAIRS)
        {
            int a = this.Data.A.Next() & MASK;
            int b = this.Data.B.Next() & MASK;
            if (a == b)
            {
                matches++;
            }
        }
        AoCUtils.LogPart1(matches);

        this.Data.A.Reset();
        this.Data.B.Reset();
        matches = 0;
        foreach (int _ in ..PART2_PAIRS)
        {
            int a = this.Data.A.NextPicky() & MASK;
            int b = this.Data.B.NextPicky() & MASK;
            if (a == b)
            {
                matches++;
            }
        }
        AoCUtils.LogPart2(matches);
    }

    /// <inheritdoc />
    protected override (Generator, Generator) Convert(string[] rawInput)
    {
        Generator a = new(int.Parse(rawInput[0].AsSpan(^3)), FACTOR_A, MULTIPLE_A);
        Generator b = new(int.Parse(rawInput[1].AsSpan(^3)), FACTOR_B, MULTIPLE_B);
        return (a, b);
    }
}
