using System.Collections.Immutable;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using JetBrains.Annotations;

namespace AdventOfCode.AoC2017.Common;

/// <summary>
/// Knot hashing utilities
/// </summary>
[PublicAPI]
public static class Knot
{
    public const int SIZE       = 256;
    public const int ITERATIONS = 64;
    public const int CHUNK_SIZE = 16;
    public const int CHUNKS     = SIZE / CHUNK_SIZE;
    public static readonly ImmutableArray<byte> StandardSuffix = [17, 31, 73, 47, 23];

    /// <summary>
    /// Knot hashes the given data
    /// </summary>
    /// <param name="data">Data to hash</param>
    /// <returns>The hashed value</returns>
    /// ReSharper disable once CognitiveComplexity
    public static UInt128 Hash(ReadOnlySpan<char> data)
    {
        // Create list
        Span<byte> list = stackalloc byte[SIZE];
        foreach (int i in ..SIZE)
        {
            list[i] = (byte)i;
        }

        // Get lengths
        Span<byte> lengths = stackalloc byte[data.Length + StandardSuffix.Length];
        foreach (int i in ..data.Length)
        {
            lengths[i] = (byte)data[i];
        }
        StandardSuffix.CopyTo(lengths[^5..]);

        // Hash
        int position = 0;
        int skip     = 0;
        foreach (int _ in ..ITERATIONS)
        {
            HashIteration(ref list, ref position, ref skip, lengths);
        }

        // Get dense hash
        UInt128 hash = 0UL;
        foreach (int chunkIndex in ..CHUNKS)
        {
            ReadOnlySpan<byte> chunk = list.Slice(chunkIndex * CHUNK_SIZE, CHUNK_SIZE);
            byte dense = (byte)chunk.Aggregate(0u, (a, b) => a ^ b);
            hash <<= 8;
            hash |= dense;
        }
        return hash;
    }

    /// <summary>
    /// Single iteration of a knot hash
    /// </summary>
    /// <param name="list">Data to hash</param>
    /// <param name="position">Hash position</param>
    /// <param name="skip">Hash skip</param>
    /// <param name="lengths">Lengths span</param>
    public static void HashIteration(ref Span<byte> list, ref int position, ref int skip, ReadOnlySpan<byte> lengths)
    {
        foreach (int length in lengths)
        {
            int end = position + length - 1;
            if (end < SIZE)
            {
                list[position..(end + 1)].Reverse();
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
}
