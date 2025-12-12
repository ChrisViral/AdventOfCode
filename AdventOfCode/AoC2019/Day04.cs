using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 04
/// </summary>
public sealed class Day04 : Solver<Range>
{
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<int> valid   = new(500);
        Span<char> buffer = stackalloc char[6];
        foreach (int code in this.Data)
        {
            bool hasDouble    = false;
            bool isIncreasing = true;
            code.TryFormat(buffer, out _);
            char previous = buffer[0];
            for (int i = 1; isIncreasing && i < 6; i++)
            {
                // Check if two characters are repeated and if they are increasing or equal
                char current  = buffer[i];
                hasDouble    |= previous == current;
                isIncreasing &= previous <= current;
                previous      = current;
            }

            // Valid codes have both conditions met
            if (hasDouble && isIncreasing)
            {
                valid.Add(code);
            }
        }
        AoCUtils.LogPart1(valid.Count);

        int fullyValid = 0;
        foreach (int code in valid)
        {
            code.TryFormat(buffer, out _);
            int runningTotal = 1;
            char previous = buffer[0];
            for (int i = 1; i < 6; i++)
            {
                char current  = buffer[i];
                if (current == previous)
                {
                    // If we are still matching a character, increment the running total
                    runningTotal++;
                }
                else if (runningTotal is 2)
                {
                    // If we no longer match and had a group of 2, the code is valid
                    break;
                }
                else
                {
                    // Else rest the total to 1
                    runningTotal = 1;
                }

                previous = current;
            }

            // If the last running total is 2, it's valid
            if (runningTotal == 2)
            {
                fullyValid++;
            }
        }

        AoCUtils.LogPart2(fullyValid);
    }

    /// <inheritdoc />
    protected override Range Convert(string[] rawInput)
    {
        ReadOnlySpan<char> line = rawInput[0];
        int from = int.Parse(line[..6]);
        int to   = int.Parse(line[7..]);
        return from..^to;
    }
}
