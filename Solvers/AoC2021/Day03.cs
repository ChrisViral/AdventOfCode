using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using static System.Convert;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 03
/// </summary>
public sealed class Day03 : Solver
{
    /// <summary>Mask for Epsilon and Gamma (only twelve binary digits used)</summary>
    private const int MASK = 0xFFF;

    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver for 2021 - 03 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Find the count on each binary digit
        int[] counts = new int[Data[0].Length];
        foreach (string report in Data)
        {
            foreach (int i in ..report.Length)
            {
                counts[i] += report[i] is '1' ? 1 : -1;
            }
        }

        // Create gama from the data we have
        int gamma = 0;
        foreach (int i in ..counts.Length)
        {
            gamma <<= 1;
            if (counts[i] > 0)
            {
                gamma |= 1;
            }
        }

        // Get epsilon from Gamma
        int epsilon = ~gamma & MASK;
        AoCUtils.LogPart1(gamma * epsilon);

        // Create a copy of the counts
        int[] countsCopy = counts.Copy();
        // Get oxygen generator and CO2 scrubber values
        int generator = ToInt32(GetRating(counts,     '1', '0'), 2);
        int scrubber  = ToInt32(GetRating(countsCopy, '0', '1'), 2);
        AoCUtils.LogPart2(generator * scrubber);
    }

    /// <summary>
    /// Gets the rating value while filtering with the given positive and negative characters
    /// </summary>
    /// <param name="counts">Pop count for each binary digit</param>
    /// <param name="positive">Character to keep when positive</param>
    /// <param name="negative">Character to keep when negative</param>
    /// <returns>The resulting report</returns>
    private string GetRating(IList<int> counts, char positive, char negative)
    {
        HashSet<string> valid   = new(Data);
        HashSet<string> invalid = [];
        foreach (int i in ..counts.Count)
        {
            // Find value to discard from the working set
            char toDiscard = counts[i] >= 0 ? negative : positive;
            foreach (string report in valid.Where(r => r[i] == toDiscard))
            {
                // Add to discard set
                invalid.Add(report);
                foreach (int j in ..report.Length)
                {
                    // Remove count
                    counts[j] -= report[j] is '1' ? 1 : -1;
                }
            }

            valid.ExceptWith(invalid);
            invalid.Clear();

            if (valid.Count == 1)
            {
                break;
            }
        }

        return valid.First();
    }
}
