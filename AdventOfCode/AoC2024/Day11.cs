using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 11
/// </summary>
public sealed class Day11 : Solver<(long value, Day11.Stone stone)[]>
{
    public sealed class Stone
    {
        private readonly long blinkResult;
        private readonly long blinkSplit;
        private readonly bool hasSplit;

        public Stone(in long original)
        {
            switch (original)
            {
                case 0L:
                    this.blinkResult = 1L;
                    this.hasSplit    = false;
                    break;

                case 1L:
                    this.blinkResult = 2024L;
                    this.hasSplit    = false;
                    break;

                default:
                    int digits = original.DigitCount;
                    if (digits.IsEven)
                    {
                        (this.blinkResult, this.blinkSplit) = SplitNumber(original, digits);
                        this.hasSplit                       = true;
                    }
                    else
                    {
                        this.blinkResult = original * 2024L;
                        this.hasSplit    = false;
                    }
                    break;
            }
        }

        public long GetCountAtDepth(in int depth, Dictionary<long, Stone> stoneCache, Dictionary<(long, int), long> countCache)
        {
            // Bottom out case
            if (depth is 1) return this.hasSplit ? 2L : 1L;

            // Get count for both results as needed
            long count = GetCountForStone(this.blinkResult, depth, stoneCache, countCache);
            if (this.hasSplit)
            {
                count += GetCountForStone(this.blinkSplit, depth, stoneCache, countCache);
            }

            return count;
        }

        private static long GetCountForStone(in long stoneValue, in int depth, Dictionary<long, Stone> stoneCache, Dictionary<(long, int), long> countCache)
        {
            // Get cached count
            if (countCache.TryGetValue((stoneValue, depth), out long cachedCount)) return cachedCount;

            // Get cached stone
            if (!stoneCache.TryGetValue(stoneValue, out Stone? stone))
            {
                // Make and store new stone
                stone = new Stone(stoneValue);
                stoneCache.Add(stoneValue, stone);
            }

            // Get count from lower depth, and cache it
            long count = stone.GetCountAtDepth(depth - 1, stoneCache, countCache);
            countCache.Add((stoneValue, depth), count);
            return count;
        }

        private static (long, long) SplitNumber(in long n, int digits)
        {
            digits /= 2;
            int pow = digits.Pow10;
            long leftHalf  = n / pow;
            long rightHalf = n - (leftHalf * pow);
            return (leftHalf, rightHalf);
        }
    }

    private const int PART1_BLINKS = 25;
    private const int PART2_BLINKS = 75;

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Dictionary<long, Stone> stonesCache = this.Data.ToDictionary();
        stonesCache.EnsureCapacity(4000);
        Dictionary<(long, int), long> countCache = new(200000);

        long count = 0L;
        foreach ((_, Stone stone) in this.Data)
        {
            count += stone.GetCountAtDepth(PART1_BLINKS, stonesCache, countCache);
        }
        AoCUtils.LogPart1(count);

        count = 0L;
        foreach ((_, Stone stone) in this.Data)
        {
            count += stone.GetCountAtDepth(PART2_BLINKS, stonesCache, countCache);
        }
        AoCUtils.LogPart2(count);
    }

    /// <inheritdoc />
    protected override (long, Stone)[] Convert(string[] rawInput)
    {
        Span<Range> splits = stackalloc Range[10];
        ReadOnlySpan<char> line = rawInput[0].AsSpan();
        int count = line.Split(splits, ' ');
        (long, Stone)[] stones = new (long, Stone)[count];
        foreach (int i in ..count)
        {
            long stone = long.Parse(line[splits[i]]);
            stones[i] = (stone, new Stone(stone));
        }
        return stones;
    }
}
