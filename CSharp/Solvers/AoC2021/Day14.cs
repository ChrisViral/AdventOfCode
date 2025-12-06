using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 14
/// </summary>
public sealed class Day14 : Solver<(string start, Dictionary<(char, char), char> rules)>
{
    /// <summary>Cycles for the first part</summary>
    private const int CYCLES      = 10;
    /// <summary>Cycles for the second part</summary>
    private const int LONG_CYCLES = 40;

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver for 2021 - 14 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup counter dictionaries
        Dictionary<char, long> counter       = new();
        Dictionary<(char, char), long> pairs = new();

        // Initialize all possible characters and pairs
        foreach ((char first, char second) pair in this.Data.rules.Keys)
        {
            counter[pair.first]  = 0L;
            counter[pair.second] = 0L;
            pairs[pair]          = 0L;
        }

        // Add the starting amount of characters and pairs
        char previous = this.Data.start[0];
        counter[previous]++;
        foreach (int i in 1..this.Data.start.Length)
        {
            char current = this.Data.start[i];
            counter[current]++;
            pairs[(previous, current)]++;
            previous = current;
        }

        foreach (int _ in ..CYCLES)
        {
            GetNextPairs(ref pairs, counter);
        }

        long diff = counter.Values.Max() - counter.Values.Min();
        AoCUtils.LogPart1(diff);

        foreach (int _ in CYCLES..LONG_CYCLES)
        {
            GetNextPairs(ref pairs, counter);
        }

        diff = counter.Values.Max() - counter.Values.Min();
        AoCUtils.LogPart2(diff);
    }

    /// <summary>
    /// Update pairs for the next cycle
    /// </summary>
    /// <param name="pairs">Current pairs dictionary</param>
    /// <param name="counter">Character counter</param>
    private void GetNextPairs(ref Dictionary<(char, char), long> pairs, IDictionary<char, long> counter)
    {
        // Copy old pairs data and clear counts
        Dictionary<(char, char), long> old = pairs;
        pairs = old.ToDictionary(p => p.Key, _ => 0L);
        foreach (((char first, char second) pair, long count) in old)
        {
            // Add the amount of inserted characters and pairs
            char insert = this.Data.rules[pair];
            counter[insert] += count;
            pairs[(pair.first, insert)]  += count;
            pairs[(insert, pair.second)] += count;
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string, Dictionary<(char, char), char>) Convert(string[] rawInput)
    {
        Dictionary<(char, char), char> rules = new(rawInput.Length - 1);
        foreach (string line in rawInput[1..])
        {
            string[] splits = line.Split(" -> ");
            rules.Add((splits[0][0], splits[0][1]), splits[1][0]);
        }
        return (rawInput[0], rules);
    }
}
