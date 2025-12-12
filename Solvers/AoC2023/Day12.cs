using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 12
/// </summary>
public sealed class Day12 : ArraySolver<(string condition, int[] groups)>
{
    private sealed class CacheEqualityComparer : IEqualityComparer<(string, ArraySegment<int>)>
    {
        public static CacheEqualityComparer Comparer { get; } = new();

        private CacheEqualityComparer() { }

        public bool Equals((string, ArraySegment<int>) x, (string, ArraySegment<int>) y)
        {
            return x.Item1 == y.Item1 && x.Item2.SequenceEqual(y.Item2);
        }

        public int GetHashCode((string, ArraySegment<int>) obj)
        {
            return HashCode.Combine(obj.Item1, obj.Item2);
        }
    }

    private const char UNKNOWN       = '?';
    private const char DAMAGED       = '#';
    private const char OPERATIONAL   = '.';
    private const int EXPANDED_COUNT = 5;

    private readonly (string condition, int[] groups)[] expandedData;
    private readonly Dictionary<(string, ArraySegment<int>), long> cache = new(CacheEqualityComparer.Comparer);


    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input)
    {
        this.expandedData = new (string, int[])[this.Data.Length];
    }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long total = this.Data.Sum(d => CountMatches(d.condition, d.groups));
        AoCUtils.LogPart1(total);

        foreach (int i in ..this.Data.Length)
        {
            (string oldCondition, int[] oldGroup) = this.Data[i];
            string newCondition  = string.Join(UNKNOWN, Enumerable.Repeat(oldCondition, EXPANDED_COUNT));
            int[] newGroup       = Enumerable.Repeat(oldGroup, EXPANDED_COUNT).SelectMany(g => g).ToArray();
            this.expandedData[i] = (newCondition, newGroup);
        }

        total = this.expandedData.Sum(d => CountMatches(d.condition, d.groups));
        AoCUtils.LogPart2(total);
    }

    // ReSharper disable once CognitiveComplexity
    public long CountMatches(string condition, ArraySegment<int> groups)
    {
        if (this.cache.TryGetValue((condition, groups), out long total)) return total;

        ReadOnlySpan<char> conditionSpan = condition;
        if (groups.Count is 0)
        {
            return conditionSpan.Contains(DAMAGED) ? 0 : 1;
        }

        total = 0L;
        int groupSize = groups[0];
        foreach (int i in ..conditionSpan.Length)
        {
            if (i + groupSize <= conditionSpan.Length
             && !conditionSpan.Slice(i, groupSize).Contains(OPERATIONAL)
             && (i is 0 || conditionSpan[i - 1] is not DAMAGED)
             && (i + groupSize == conditionSpan.Length || conditionSpan[i + groupSize] is not DAMAGED))
            {
                int nextStart = i + groupSize + 1;
                if (nextStart < conditionSpan.Length)
                {
                    total += CountMatches(condition[nextStart..], groups[1..]);
                }
                else
                {
                    total += groups.Count is 1 ? 1 : 0;
                }
            }

            if (conditionSpan[i] is DAMAGED) break;
        }

        this.cache.Add((condition, groups), total);
        return total;
    }

    /// <inheritdoc />
    protected override (string, int[]) ConvertLine(string line)
    {
        string[] splits = line.Split(' ', DEFAULT_OPTIONS);
        string springs  = splits[0];
        int[] groups    = splits[1].Split(',', DEFAULT_OPTIONS).ConvertAll(int.Parse);
        return (springs, groups);
    }
}
