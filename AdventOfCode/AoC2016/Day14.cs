using System.Security.Cryptography;
using System.Text;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 14
/// </summary>
public sealed class Day14 : Solver<byte[]>
{
    private const int KEYS_COUNT   = 64;
    private const int PART2_HASHES = 2016;
    private const int CHECK_WINDOW = 1000;

    private static readonly byte[] HashBytes = new byte[MD5.HashSizeInBytes];
    private static readonly byte[] HexBytes  = new byte[MD5.HashSizeInBytes * 2];

    private readonly MD5 md5 = MD5.Create();

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<string> cache = new(30_000);
        int keyIndex = GetFinalKeyIndex(cache);
        AoCUtils.LogPart1(keyIndex);

        cache.Clear();
        keyIndex = GetFinalKeyIndex(cache, PART2_HASHES);
        AoCUtils.LogPart2(keyIndex);
    }

    private int GetFinalKeyIndex(List<string> cache, int extraHashes = 0)
    {
        int index;
        List<string> keys  = new(KEYS_COUNT);
        for (index = 0; keys.Count < KEYS_COUNT; )
        {
            // Check if key has three in a row
            string key = GetNextHash(ref index, cache, extraHashes);
            if (!HasTripleChar(key, out char target)) continue;

            int nextEnd = index + CHECK_WINDOW;
            for (int nextIndex = index; nextIndex < nextEnd; )
            {
                // Check if key has five in a row of the same as before
                string nextKey = GetNextHash(ref nextIndex, cache, extraHashes);
                if (HasQuintupleChar(nextKey, target))
                {
                    keys.Add(key);
                    break;
                }
            }
        }

        return index - 1;
    }

    private string GetNextHash(ref int index, List<string> cache, int extraHashes)
    {
        // Check cache
        if (index < cache.Count)
        {
            return cache[index++];
        }

        // Get data buffer and copy salt and index into it
        Span<byte> data = stackalloc byte[this.Data.Length + index.DigitCount];
        this.Data.CopyTo(data);
        index.TryFormat(data[this.Data.Length..], out int _);

        // Compute hash
        this.md5.TryComputeHash(data, HashBytes, out _);

        // Repeat hashes if necessary
        foreach (int __ in ..extraHashes)
        {
            System.Convert.TryToHexStringLower(HashBytes, HexBytes, out _);
            this.md5.TryComputeHash(HexBytes, HashBytes, out _);
        }

        // Get hash hex and store in cache
        string hash = System.Convert.ToHexStringLower(HashBytes);
        cache.Add(hash);

        // Increment index and return hash
        index++;
        return hash;
    }

    private static bool HasTripleChar(ReadOnlySpan<char> hash, out char triple)
    {
        if (hash.Length < 3)
        {
            triple = char.MinValue;
            return false;
        }

        int end = hash.Length - 2;
        for (int i = 0; i < end; /* i++ */)
        {
            // Check next two characters for matches
            char first = hash[i];
            if (first != hash[++i]) continue;
            if (first != hash[++i]) continue;

            triple = first;
            return true;
        }

        triple = char.MinValue;
        return false;
    }

    private static bool HasQuintupleChar(ReadOnlySpan<char> hash, char target)
    {
        if (hash.Length < 5) return false;

        int run = 0;
        int end = hash.Length - 4;
        for (int i = 0; i < end + run; i++)
        {
            if (hash[i] == target)
            {
                // Check if we are at 5 in a row
                if (run == 4) return true;

                // If not increment current run
                run++;
            }
            else
            {
                // Reset run on invalid char
                run = 0;
            }
        }

        return false;
    }

    /// <inheritdoc />
    protected override byte[] Convert(string[] rawInput) => Encoding.UTF8.GetBytes(rawInput[0]);

    /// <inheritdoc />
    public override void Dispose()
    {
        this.md5.Dispose();
        base.Dispose();
    }
}
