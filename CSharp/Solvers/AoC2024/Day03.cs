﻿using System;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 03
/// </summary>
public partial class Day03 : Solver<string>
{
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do(?:n't)?\(\)", RegexOptions.Compiled)]
    private static partial Regex MulRegex { get; }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day03(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int result = 0;
        int conditionalResult = 0;
        bool flag = true;
        foreach (Match match in MulRegex.Matches(this.Data))
        {
            switch (match.ValueSpan)
            {
                case "do()":
                    flag = true;
                    break;

                case "don't()":
                    flag = false;
                    break;

                default:
                    int x = int.Parse(match.Groups[1].ValueSpan);
                    int y = int.Parse(match.Groups[2].ValueSpan);
                    int value = x * y;
                    result += value;
                    if (flag)
                    {
                        conditionalResult += value;
                    }
                    break;
            }
        }

        AoCUtils.LogPart1(result);
        AoCUtils.LogPart2(conditionalResult);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] rawInput) => string.Join(string.Empty, rawInput);
    #endregion
}