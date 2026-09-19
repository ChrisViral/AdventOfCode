using System.Buffers;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 01
/// </summary>
[Solver(2023, 1)]
public sealed class Day01 : Solver
{
    private readonly SearchValues<char> digits = SearchValues.Create("123456789");

    private readonly Dictionary<string, int> numberValues = new()
    {
        ["1"]     = 1,
        ["2"]     = 2,
        ["3"]     = 3,
        ["4"]     = 4,
        ["5"]     = 5,
        ["6"]     = 6,
        ["7"]     = 7,
        ["8"]     = 8,
        ["9"]     = 9,
        ["one"]   = 1,
        ["two"]   = 2,
        ["three"] = 3,
        ["four"]  = 4,
        ["five"]  = 5,
        ["six"]   = 6,
        ["seven"] = 7,
        ["eight"] = 8,
        ["nine"]  = 9
    };

    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    public Day01(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        foreach (ReadOnlySpan<char> value in this.Data)
        {
            // Get the first match values on both ends
            total += (value[value.IndexOfAny(this.digits)] - '0') * 10;
            total += value[value.LastIndexOfAny(this.digits)] - '0';
        }
        LogAnswer(total);

        total = this.Data.Sum(GetCalibrationValue);
        LogAnswer(total);
    }

    // ReSharper disable once CognitiveComplexity
    private int GetCalibrationValue(string data)
    {
        ReadOnlySpan<char> value = data;
        int calibration = 0;
        bool first = false, last = false;

        // Check from both ends of the string, moving inwards
        foreach (int i in ..value.Length)
        {
            // Check each possible number value
            foreach ((string s, int n) in this.numberValues)
            {
                // Check if there is a match at the current start point
                if (!first && value[i..].StartsWith(s))
                {
                    calibration += n * 10;
                    // If the last value has been found already, return
                    if (last) return calibration;

                    first = true;
                }

                // Check if there is a match at the current End point
                if (!last && value[..^i].EndsWith(s))
                {
                    calibration += n;
                    // If the first value has been found already, return
                    if (first) return calibration;

                    last = true;
                }
            }
        }

        return calibration;
    }
}
