using System.Text.RegularExpressions;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Regexes;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 05
/// </summary>
public sealed partial class Day05 : Solver<(long[] seeds, Dictionary<string, Day05.Map> maps)>
{
    public record struct MapRange(long Destination, long Source, long Length)
    {
        public bool IsInSourceRange(long value) => value >= this.Source && value < this.Source + this.Length;

        public long MapValue(long value) => (value - this.Source) + this.Destination;
    }

    public readonly partial struct Map
    {
        [GeneratedRegex("([a-z]+)-to-([a-z]+) map:")]
        private static partial Regex MapMatcher { get; }

        [GeneratedRegex(@"(\d+) (\d+) (\d+)")]
        private static partial Regex RangeMatcher { get; }

        public readonly string from;
        public readonly string to;
        private readonly MapRange[] ranges;

        public Map(string map, string[] ranges)
        {
            string[] identifiers = MapMatcher.Match(map).CapturedGroups.Select(g => g.Value).ToArray();
            this.from = identifiers[0];
            this.to   = identifiers[1];

            this.ranges = RegexFactory<MapRange>.ConstructObjects(RangeMatcher, ranges);
        }

        public long MapValue(long value)
        {
            foreach (MapRange range in this.ranges)
            {
                if (range.IsInSourceRange(value))
                {
                    return range.MapValue(value);
                }
            }

            return value;
        }

    }

    private readonly Safe<long> minSeed = new(long.MaxValue);

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long min = long.MaxValue;
        foreach (int i in ..this.Data.seeds.Length)
        {
            long value = this.Data.seeds[i];
            Map map = this.Data.maps["seed"];
            for (bool hasNextMap = true; hasNextMap; hasNextMap = this.Data.maps.TryGetValue(map.to, out map))
            {
                value = map.MapValue(value);
            }

            min = Math.Min(value, min);
        }

        AoCUtils.LogPart1(min);

        // CBA to optimize it, running it in parallel takes less time to write and runs in less than a minute
        ParallelLoopResult result = Parallel.For(0, this.Data.seeds.Length / 2, ParallelFindMin);
        while (!result.IsCompleted)
        {
            Thread.Sleep(1000);
        }

        AoCUtils.LogPart2(this.minSeed);
    }

    public void ParallelFindMin(int i)
    {
        int id = i + 1;
        i *= 2;
        long min = long.MaxValue;
        long current = this.Data.seeds[i++];
        long end = current + this.Data.seeds[i];
        for (; current < end; current++)
        {
            long value = current;
            Map map = this.Data.maps["seed"];
            for (bool hasNextMap = true; hasNextMap; hasNextMap = this.Data.maps.TryGetValue(map.to, out map))
            {
                value = map.MapValue(value);
            }

            min = Math.Min(value, min);
        }

        this.minSeed.Update(c => Math.Min(min, c));
        AoCUtils.Log($"Task {id} finished");
    }

    /// <inheritdoc />
    protected override (long[] seeds, Dictionary<string, Map> maps) Convert(string[] rawInput)
    {
        long[] seeds = rawInput[0][7..].Split(' ', DEFAULT_OPTIONS).ConvertAll(long.Parse);
        Dictionary<string, Map> maps = new();

        Map map;
        string indicator = rawInput[1];
        int current = 2;
        int start   = current;
        for (; current < rawInput.Length; current++)
        {
            if (char.IsNumber(rawInput[current][0])) continue;

            map = new Map(indicator, rawInput[start..current]);
            maps.Add(map.from, map);
            indicator = rawInput[current++];
            start = current;
        }

        map = new Map(indicator, rawInput[start..current]);
        maps.Add(map.from, map);
        return (seeds, maps);
    }
}
