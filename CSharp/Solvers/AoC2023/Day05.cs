using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 05
/// </summary>
public class Day05 : Solver<(long[] seeds, Dictionary<string, Day05.Map> maps)>
{
    public record struct MapRange(long Destination, long Source, long Length)
    {
        public bool IsInSourceRange(long value) => value >= this.Source && value < this.Source + this.Length;

        public long MapValue(long value) => (value - Source) + Destination;
    }

    public readonly struct Map
    {
        private const string MAP_PATTERN = "([a-z]+)-to-([a-z]+) map:";
        private static readonly Regex mapMatcher = new(MAP_PATTERN, RegexOptions.Compiled);

        private const string RANGE_PATTERN = @"(\d+) (\d+) (\d+)";

        public readonly string from;
        public readonly string to;
        public readonly MapRange[] ranges;

        public Map(string map, string[] ranges)
        {
            string[] identifiers = mapMatcher.Match(map).CapturedGroups.Select(g => g.Value).ToArray();
            this.from = identifiers[0];
            this.to   = identifiers[1];

            this.ranges = RegexFactory<MapRange>.ConstructObjects(RANGE_PATTERN, ranges, RegexOptions.Compiled);
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

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day05(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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

    /// <inheritdoc cref="Solver{T}.Convert"/>
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

            map = new(indicator, rawInput[start..current]);
            maps.Add(map.from, map);
            indicator = rawInput[current++];
            start = current;
        }

        map = new(indicator, rawInput[start..current]);
        maps.Add(map.from, map);
        return (seeds, maps);
    }
    #endregion
}
