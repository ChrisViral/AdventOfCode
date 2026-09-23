using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Utils;
using Challenge.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 12
/// </summary>
[Solver(2025, 12)]
public sealed partial class Day12 : Solver<(Grid<bool>[] presents, Day12.Region[] regions)>
{
    private const int PRESENT_COUNT = 6;
    private const int PRESENT_SIZE = 3;
    private const char BLOCK = '#';

    [GeneratedRegex(@"(\d+)x(\d+): (\d+) (\d+) (\d+) (\d+) (\d+) (\d+)")]
    private static partial Regex RegionMatcher { get; }

    public sealed class Region(int width, int height, int a, int b, int c, int d, int e, int f)
    {
        public int Area { get; } = width * height;

        public int[] Quantities { get; } = [a, b, c, d, e, f];
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int[] presentSizes = this.Data.presents
                                 .Select(p => p.Count(b => b))
                                 .ToArray();
        int canFit = this.Data.regions
                         .Select(r => (a: r.Area,
                                       p: r.Quantities
                                           .Select((q, i) => q * presentSizes[i])
                                           .Sum()))
                         .Count(d => d.a > d.p);
        LogAnswer(canFit);
    }

    /// <inheritdoc />
    protected override (Grid<bool>[], Region[]) Convert(string[] rawInput)
    {
        Grid<bool>[] presents = new Grid<bool>[PRESENT_COUNT];
        foreach (int i in ..PRESENT_COUNT)
        {
            int start = (i * (PRESENT_SIZE + 1)) + 1;
            int end = start + PRESENT_SIZE;
            presents[i] = new Grid<bool>(PRESENT_SIZE, PRESENT_SIZE, rawInput[start..end], line => line.Select(c => c is BLOCK).ToArray());
        }

        const int REGIONS_START = (PRESENT_SIZE + 1) * PRESENT_COUNT;
        string[] regionData = rawInput[REGIONS_START..];
        Region[] regions = RegexFactory<Region>.ConstructObjects(RegionMatcher, regionData);
        return (presents, regions);
    }
}
