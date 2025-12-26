using System.Collections.Immutable;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;
using SpanLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 10
/// </summary>
public sealed class Day10 : Solver<string>
{
    private const int SIZE   = 256;
    private const int ROUNDS = 64;
    private static readonly ImmutableArray<byte> StandardSuffix = [17, 31, 73, 47, 23];

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create hash list
        int position = 0;
        int skip     = 0;
        Span<byte> list = stackalloc byte[SIZE];
        foreach (int i in ..SIZE)
        {
            list[i] = (byte)i;
        }

        // Get length numbers
        byte[] numbers = this.Data.Split(',').ConvertAll(byte.Parse);

        // Hash and output
        KnotHash(ref list, ref position, ref skip, numbers);
        AoCUtils.LogPart1(list[0] * list[1]);

        // Reset hash list
        position = 0;
        skip     = 0;
        foreach (int i in ..SIZE)
        {
            list[i] = (byte)i;
        }

        // Obtain new lengths
        Span<byte> lengths = stackalloc byte[this.Data.Length + StandardSuffix.Length];
        foreach (int i in ..this.Data.Length)
        {
            lengths[i] = (byte)this.Data[i];
        }
        StandardSuffix.CopyTo(lengths[^5..]);

        // Hash
        foreach (int _ in ..ROUNDS)
        {
            KnotHash(ref list, ref position, ref skip, lengths);
        }

        // Get dense hash
        int start = 0;
        Span<char> hash = stackalloc char[32];
        foreach (byte[] chunk in list.Chunk(16))
        {
            byte dense = (byte)chunk.Aggregate(0u, (a, b) => a ^ b);
            dense.TryFormat(hash[start..], out int _, "x");
            start += 2;
        }
        AoCUtils.LogPart2(hash.ToString());
    }

    private static void KnotHash(ref Span<byte> list, ref int position, ref int skip, ReadOnlySpan<byte> lengths)
    {
        foreach (int length in lengths)
        {
            int end = position + length - 1;
            if (end < SIZE)
            {
                MemoryExtensions.Reverse(list[position..(end + 1)]);
            }
            else
            {
                int swapCounts = length / 2;
                foreach (int offset in ..swapCounts)
                {
                    AoCUtils.Swap(ref list[(position + offset) % SIZE],
                                  ref list[(end - offset) % SIZE]);
                }
            }

            position = (position + length + skip) % SIZE;
            skip = (skip + 1) % SIZE;
        }

    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
