using System;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2025;

/// <summary>
/// Solver for 2025 Day 02
/// </summary>
public class Day02 : Solver<Day02.IdRange[]>
{
    public readonly record struct IdRange(long Start, long End);

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    private static readonly char[] Buffer = new char[19];

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long invalid = 0L;
        long invalidRepeated = 0L;
        foreach (IdRange range in this.Data)
        {
            for (long id = range.Start; id <= range.End; id++)
            {
                id.TryFormat(Buffer, out int size);
                ReadOnlySpan<char> value = Buffer.AsSpan(0, size);
                if (IsInvalid(value))
                {
                    invalid         += id;
                }
                else if (IsInvalidRepeated(value))
                {
                    invalidRepeated += id;
                }
            }
        }

        AoCUtils.LogPart1(invalid);
        AoCUtils.LogPart2(invalid + invalidRepeated);
    }

    private static bool IsInvalid(ReadOnlySpan<char> value)
    {
        if (!value.Length.IsEven) return false;

        int middle = value.Length / 2;
        return value[..middle].SequenceEqual(value[middle..]);
    }

    private static bool IsInvalidRepeated(ReadOnlySpan<char> value)
    {
        if (value.Length < 3) return false;

        int chunkSize = 1;
        int maxChunkSize = value.Length / 2;
        do
        {
            if (AllChunksEqual(value, chunkSize)) return true;
        }
        while (++chunkSize < maxChunkSize);
        return false;
    }

    private static bool AllChunksEqual(ReadOnlySpan<char> value, int chunkSize)
    {
        if (!chunkSize.IsFactor(value.Length)) return false;

        ReadOnlySpan<char> firstChunk = value[..chunkSize];
        for (int chunkStart = chunkSize; chunkStart < value.Length; chunkStart += chunkSize)
        {
            if (!value.Slice(chunkStart, chunkSize).SequenceEqual(firstChunk)) return false;
        }
        return true;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override IdRange[] Convert(string[] rawInput)
    {
        // Split ID ranges
        ReadOnlySpan<char> data = rawInput[0];
        int count = data.Count(',') + 1;
        Span<Range> splits = stackalloc Range[count];
        data.Split(splits, ',');

        IdRange[] ranges = new IdRange[count];
        Span<Range> rangeSplits = stackalloc Range[2];
        foreach (int i in ..count)
        {
            // Parse individual ranges
            ReadOnlySpan<char> rangeData = data[splits[i]];
            rangeData.Split(rangeSplits, '-');
            long start = long.Parse(rangeData[rangeSplits[0]]);
            long end   = long.Parse(rangeData[rangeSplits[1]]);
            ranges[i]  = new IdRange(start, end);
        }
        return ranges;
    }
    #endregion
}
