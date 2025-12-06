using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 19
/// </summary>
public sealed class Day19 : Solver<(string[] towels, string[] designs)>
{
    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1,T2}"/> fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Keep a set of the valid and invalid designs we found
        HashSet<string> validDesigns = new(this.Data.designs.Length);
        HashSet<string> invalidDesigns = new(this.Data.designs.Length);
        int valid = this.Data.designs.Count(d => TestDesign(d, validDesigns.GetAlternateLookup<ReadOnlySpan<char>>(),
                                                            invalidDesigns.GetAlternateLookup<ReadOnlySpan<char>>()));
        AoCUtils.LogPart1(valid);

        // Remove designs not part of the original ones
        validDesigns.IntersectWith(this.Data.designs);
        // Keep a map of the arrangement count per design
        Dictionary<string, long> arrangements = new(validDesigns.Count + invalidDesigns.Count);
        // Add all designs we know are invalid as having 0 arrangements
        foreach (string invalid in invalidDesigns)
        {
            arrangements.Add(invalid, 0L);
        }

        long possibleDesigns = validDesigns.Sum(d => CountDesigns(d, arrangements.GetAlternateLookup<ReadOnlySpan<char>>()));
        AoCUtils.LogPart2(possibleDesigns);
    }

    private bool TestDesign(ReadOnlySpan<char> design,
                            in HashSet<string>.AlternateLookup<ReadOnlySpan<char>> validDesigns,
                            in HashSet<string>.AlternateLookup<ReadOnlySpan<char>> invalidDesigns)
    {
        if (validDesigns.Contains(design)) return true;
        if (invalidDesigns.Contains(design)) return false;

        foreach (ReadOnlySpan<char> towel in this.Data.towels)
        {
            if (design.StartsWith(towel) && (towel.Length == design.Length || TestDesign(design[towel.Length..], validDesigns, invalidDesigns)))
            {
                validDesigns.Add(design);
                return true;
            }
        }

        invalidDesigns.Add(design);
        return false;
    }

    private long CountDesigns(ReadOnlySpan<char> design, in Dictionary<string, long>.AlternateLookup<ReadOnlySpan<char>> arrangements)
    {
        if (arrangements.TryGetValue(design, out long count)) return count;

        foreach (ReadOnlySpan<char> towel in this.Data.towels)
        {
            if (!design.StartsWith(towel)) continue;

            if (towel.Length == design.Length)
            {
                count++;
            }
            else
            {
                count += CountDesigns(design[towel.Length..], arrangements);
            }
        }

        arrangements[design] = count;
        return count;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string[], string[]) Convert(string[] rawInput)
    {
        string[] towels = rawInput[0].Split(", ");
        towels.Sort((a, b) => b.Length.CompareTo(a.Length));
        string[] designs = rawInput[1..];
        return (towels, designs);
    }
}
