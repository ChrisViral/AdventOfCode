using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 6
/// </summary>
[Solver(2022, 6)]
public sealed class Day06 : Solver<string>
{
    /// <summary>Character Counter</summary>
    private static readonly Counter<char> CharacterCounter = new(14);

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver for 2022 - 06 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day06(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        LogAnswer(FindUniqueSliceOfLength(4));
        LogAnswer(FindUniqueSliceOfLength(14));
    }

    /// <inheritdoc />
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
        CharacterCounter.AddRange(this.Data[..start]);
        foreach (int i in start..this.Data.Length)
        {
            CharacterCounter.Add(this.Data[i]);
            // If the counter length matches the slice length, all elements are unique
            if (CharacterCounter.Count == length)
            {
                CharacterCounter.Clear();
                return i + 1;
            }

            CharacterCounter.Remove(this.Data[i - start]);
        }

        // Nothing found
        CharacterCounter.Clear();
        return -1;
    }
}
