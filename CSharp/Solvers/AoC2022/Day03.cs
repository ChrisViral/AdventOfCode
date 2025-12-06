using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 03
/// </summary>
public sealed class Day03 : Solver<string[]>
{
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver for 2022 - 03 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        foreach (string line in this.Data)
        {
            ReadOnlySpan<char> rucksack = line;
            int half = rucksack.Length / 2;
            ReadOnlySpan<char> first  = rucksack[..half];
            ReadOnlySpan<char> second = rucksack[half..];
            int index = first.IndexOfAny(second);
            total += GetPriority(first[index]);
        }

        AoCUtils.LogPart1(total);

        total = 0;
        for (int i = 0; i < this.Data.Length; /*i += 3*/)
        {
            string first  = this.Data[i++];
            string second = this.Data[i++];
            string third  = this.Data[i++];
            char match = first.First(item => second.Contains(item) && third.Contains(item));
            total += GetPriority(match);
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string[] Convert(string[] lines) => lines;

    /// <summary>
    /// Get the associated priority for an item
    /// </summary>
    /// <param name="item">Item to get the priority for</param>
    /// <returns>1-26 for a-z, 27-52 for A-Z</returns>
    private static int GetPriority(char item) => char.IsLower(item) ? item - 'a' + 1 : item - 'A' + 27;
}
