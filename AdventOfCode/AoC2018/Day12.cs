using System.Collections.Frozen;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 12
/// </summary>
public sealed partial class Day12 : Solver<(StringBuilder plants, FrozenDictionary<string, char> rules)>
{
    private const char PLANT = '#';
    private const char EMPTY = '.';

    private const int WINDOW_SIZE = 5;
    private const int PART1_GENERATIONS  = 20;
    private const long PART2_GENERATIONS = 50000000000L;
    private const int DIFFS_THRESHOLD = 5;

    [GeneratedRegex("initial state: ([#.]+)")]
    private static partial Regex InitialStateMatcher { get; }

    [GeneratedRegex("([#.]{5}) => ([#.])")]
    private static partial Regex RuleMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        ReadOnlySpan<char> padding = [EMPTY, EMPTY, EMPTY, EMPTY];
        this.Data.plants.Insert(0, padding);
        this.Data.plants.Append(padding);
        int startOffset = -4;

        Counter<int> diffs = new();
        int currentDiff;
        int previousPots = PotsValue(startOffset);
        foreach (int _ in ..PART1_GENERATIONS)
        {
            ApplyGeneratation(ref startOffset);
            int currentPots = PotsValue(startOffset);
            currentDiff = currentPots - previousPots;
            diffs.Add(currentDiff);
            previousPots = currentPots;
        }
        AoCUtils.LogPart1(previousPots);

        int generation = PART1_GENERATIONS;
        do
        {
            generation++;
            ApplyGeneratation(ref startOffset);
            int currentPots = PotsValue(startOffset);
            currentDiff = currentPots - previousPots;
            diffs.Add(currentDiff);
            previousPots = currentPots;
        }
        while (diffs[currentDiff] < DIFFS_THRESHOLD);

        int maxDiff = diffs.AsDictionary().MaxBy(p => p.Value).Key;
        long result = previousPots + ((PART2_GENERATIONS - generation) * maxDiff);
        AoCUtils.LogPart2(result);
    }

    private void ApplyGeneratation(ref int startOffset)
    {
        // Add padding as required
        if (this.Data.plants[3] is PLANT)
        {
            this.Data.plants.Insert(0, EMPTY);
            startOffset--;
        }
        if (this.Data.plants[^4] is PLANT)
        {
            this.Data.plants.Append(EMPTY);
        }

        Span<char> buffer = stackalloc char[this.Data.plants.Length];
        this.Data.plants.CopyTo(0, buffer, buffer.Length);
        FrozenDictionary<string, char>.AlternateLookup<ReadOnlySpan<char>> lookup = this.Data.rules.GetAlternateLookup<ReadOnlySpan<char>>();
        foreach (int i in ..(buffer.Length - WINDOW_SIZE))
        {
            ReadOnlySpan<char> window = buffer.Slice(i, WINDOW_SIZE);
            this.Data.plants[i + 2] = lookup[window];
        }
    }

    private int PotsValue(int startOffset)
    {
        int pots = 0;
        foreach (int i in ..this.Data.plants.Length)
        {
            if (this.Data.plants[i] is PLANT)
            {
                pots += i + startOffset;
            }
        }
        return pots;
    }

    /// <inheritdoc />
    protected override (StringBuilder, FrozenDictionary<string, char>) Convert(string[] rawInput)
    {
        StringBuilder plants = new(InitialStateMatcher.Match(rawInput[0]).Groups[1].Value);

        Dictionary<string, char> rules = new(rawInput.Length - 1);
        foreach (string ruleDef in rawInput.AsSpan(1))
        {
            Match rule = RuleMatcher.Match(ruleDef);
            rules.Add(rule.Groups[1].Value, rule.Groups[2].ValueSpan[0]);
        }
        return (plants, rules.ToFrozenDictionary());
    }
}
