using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 05
/// </summary>
public sealed partial class Day05 : Solver<(Day05.IdRange[] Ranges, long[] Products)>
{
    [DebuggerDisplay("{Start}-{End}")]
    public readonly record struct IdRange(long Start, long End)
    {
        public long Size => this.End - this.Start + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(long value) => value >= this.Start && value <= this.End;
    }

    [GeneratedRegex(@"(\d+)-(\d+)")]
    private static partial Regex RangeMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Merge all overlapping ranges
        long valid = 0L;
        int count = this.Data.Ranges.Length;
        for (int i = 0; i < count; i++)
        {
            ref IdRange first = ref this.Data.Ranges[i];
            for (int j = i + 1; j < count; j++)
            {
                ref IdRange second = ref this.Data.Ranges[j];
                if (MergeRanges(first, second, out IdRange merged))
                {
                    // Move merged result to current first
                    first  = merged;
                    // Swap second for current last, then shrink list
                    second = this.Data.Ranges[--count];
                    // Reset second loop to start
                    j = i;
                }
            }
            valid += first.Size;
        }

        // Find fresh products
        Span<IdRange> ranges = this.Data.Ranges.AsSpan(0, count);
        int fresh = 0;
        foreach (long id in this.Data.Products)
        {
            foreach (IdRange range in ranges)
            {
                if (range.IsInRange(id))
                {
                    fresh++;
                    break;
                }
            }
        }
        AoCUtils.LogPart1(fresh);
        AoCUtils.LogPart2(valid);
    }

    private static bool MergeRanges(in IdRange a, in IdRange b, out IdRange merged)
    {
        // Make sure the largest range is first
        if (a.Size < b.Size)
        {
            return MergeRanges(b, a, out merged);
        }

        // If the ranges are the same, just return one of them as the merge
        if (a == b)
        {
            merged = a;
            return true;
        }

        bool startWithin = a.IsInRange(b.Start);
        bool endWithin   = a.IsInRange(b.End);
        switch ((startWithin, endWithin))
        {
            // B is fully contained within A
            case (true, true):
                merged = a;
                return true;

            // B starts within A and ends after it
            case (true, false):
                merged = new IdRange(a.Start, b.End);
                return true;

            // B starts before A and ends within it
            case (false, true):
                merged = new IdRange(b.Start, a.End);
                return true;

            // No overlap
            default:
                merged = default;
                return false;
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (IdRange[], long[]) Convert(string[] rawInput)
    {
        int productEnd   = rawInput.IndexOf(string.Empty);
        IdRange[] ranges = RegexFactory<IdRange>.ConstructObjects(RangeMatcher, rawInput[..productEnd]);
        long[] products  = rawInput[(productEnd + 1)..^1].ConvertAll(long.Parse);
        return (ranges, products);
    }
}
