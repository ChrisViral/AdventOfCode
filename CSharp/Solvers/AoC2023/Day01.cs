using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 01
/// </summary>
public class Day01 : Solver
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

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    public Day01(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int total = 0;
        foreach (ReadOnlySpan<char> value in this.Data)
        {
            // Get the first match values on both ends
            total += (value[value.IndexOfAny(digits)] - '0') * 10;
            total += value[value.LastIndexOfAny(digits)] - '0';
        }
        AoCUtils.LogPart1(total);

        total = this.Data.Sum(GetCalibrationValue);
        AoCUtils.LogPart2(total);
    }

    private int GetCalibrationValue(string data)
    {
        ReadOnlySpan<char> value = data;
        int calibration = 0;
        bool first = false, last = false;

        // Check from both ends of the string, moving inwards
        foreach (int i in ..value.Length)
        {
            // Check each possible number value
            foreach ((string s, int n) in numberValues)
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
    #endregion
}