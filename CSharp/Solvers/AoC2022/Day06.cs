using System;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 06
/// </summary>
public class Day06 : Solver<string>
{
    /// <summary>Character Counter</summary>
    private static readonly Counter<char> characterCounter = new(14);

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver for 2022 - 06 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(FindUniqueSliceOfLength(4));
        AoCUtils.LogPart2(FindUniqueSliceOfLength(14));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] lines)
    {
        return lines[0];
    }

    /// <summary>
    /// Finds the last element index for a slice from the data of <paramref name="length"/> that only contains unique elements
    /// </summary>
    /// <param name="length">Length of the slice</param>
    /// <returns>The index of the last element of the first unique slice of <paramref name="length"/>, otherwise <c>-1</c> if none is found</returns>
    private int FindUniqueSliceOfLength(int length)
    {
        int start = length - 1;
        characterCounter.AddRange(this.Data[..start]);
        foreach (int i in start..this.Data.Length)
        {
            characterCounter.Add(this.Data[i]);
            // If the counter length matches the slice length, all elements are unique
            if (characterCounter.Count == length)
            {
                characterCounter.Clear();
                return i + 1;
            }

            characterCounter.Remove(this.Data[i - start]);
        }

        // Nothing found
        characterCounter.Clear();
        return -1;
    }
}
