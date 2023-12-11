﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 08
/// </summary>
public class Day08 : Solver<(Directions[] directions, Dictionary<string, (string left, string right)> map)>
{
    private const string NODE_PATTERN = @"([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)";
    private const string START = "AAA";
    private const string END   = "ZZZ";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day08(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int steps = CalculateSteps(START, n => n is END);
        AoCUtils.LogPart1(steps);

        HashSet<string> endNodes = new(this.Data.map.Keys.Where(n => n[^1] is 'Z'));
        long totalSteps = this.Data.map.Keys
            .Where(n => n[^1] is 'A')
            .Select(s => (long)CalculateSteps(s, endNodes.Contains))
            .Aggregate(MathUtils.LCM);
        AoCUtils.LogPart2(totalSteps);
    }

    public int CalculateSteps(string start, Predicate<string> reachedEnd)
    {
        int i = 0;
        int steps = 0;
        string current = start;
        do
        {
            Directions dir = this.Data.directions[i];
            i = (i + 1).Mod(this.Data.directions.Length);
            (string left, string right) = this.Data.map[current];
            current = dir is Directions.LEFT ? left : right;
            steps++;
        }
        while (!reachedEnd(current));

        return steps;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Directions[], Dictionary<string, (string, string)>) Convert(string[] rawInput)
    {
        Directions[] directions = rawInput[0].ToCharArray().ConvertAll(DirectionsUtils.Parse);
        (string label, string left, string right)[] nodes = RegexFactory<(string, string, string)>.ConstructObjects(NODE_PATTERN, rawInput[1..], RegexOptions.Compiled);
        Dictionary<string, (string, string)> map = new(nodes.Length);

        map.AddRange(nodes.Select(n => new KeyValuePair<string, (string, string)>(n.label, (n.left, n.right))));
        return (directions, map);
    }
    #endregion
}