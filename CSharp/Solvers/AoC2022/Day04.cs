using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 04
/// </summary>
public class Day04 : Solver<((int, int) first, (int, int) second)[]>
{
    /// <summary>Input match pattern</summary>
    private const string PATTERN = @"(\d+)-(\d+),(\d+)-(\d+)";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver for 2022 - 04 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        int fullOverlaps    = 0;
        int partialOverlaps = 0;
        foreach (((int firstStart, int firstEnd), (int secondStart, int secondEnd)) in this.Data)
        {
            if ((firstStart  <= secondStart && firstEnd  >= secondEnd)
             || (secondStart <= firstStart  && secondEnd >= firstEnd))
            {
                fullOverlaps++;
            }
            if ((firstStart  <= secondStart && firstEnd  >= secondStart)
             || (secondStart <= firstStart  && secondEnd >= firstStart))
            {
                partialOverlaps++;
            }
        }

        AoCUtils.LogPart1(fullOverlaps);
        AoCUtils.LogPart2(partialOverlaps);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override ((int, int), (int, int))[] Convert(string[] lines)
    {
        return RegexFactory<(int a, int b, int c, int d)>.ConstructObjects(PATTERN, lines)
                                                         .Select(tuple => ((tuple.a, tuple.b),
                                                                           (tuple.c, tuple.d)))
                                                         .ToArray();
    }
    #endregion
}

