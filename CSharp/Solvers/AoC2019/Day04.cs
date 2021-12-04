using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 04
/// </summary>
public class Day04 : Solver<Range>
{
    #region Constants
    private static readonly Regex adjacentMatch = new(@"^\d*(\d)\1+\d*$", RegexOptions.Compiled);
    private static readonly Regex adjacentPairMatch = new(@"(?:^|(\d)(?!\1))(\d)\2(?!\2)", RegexOptions.Compiled);
    private static readonly Regex increasingMatch = new(@"^1*2*3*4*5*6*7*8*9*$", RegexOptions.Compiled);
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Range"/> fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        List<string> valid = this.Data.AsEnumerable()
                                 .Select(p => p.ToString())
                                 .Where(s => adjacentMatch.IsMatch(s) && increasingMatch.IsMatch(s))
                                 .ToList();
        AoCUtils.LogPart1(valid.Count);
        AoCUtils.LogPart2(valid.Count(s => adjacentPairMatch.IsMatch(s)));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Range Convert(string[] rawInput)
    {
        string[] splits = rawInput[0].Split('-', StringSplitOptions.TrimEntries);
        return int.Parse(splits[0])..int.Parse(splits[1]);
    }
    #endregion
}