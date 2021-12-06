using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 06
/// </summary>
public class Day06 : Solver<int[]>
{
    #region Constants
    private const int DAYS = 80;
    private const int LONG_DAYS = 256;
    private static readonly Dictionary<int, long> cache = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver for 2021 - 06 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day06(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        long count = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(DAYS - fish - 1));
        AoCUtils.LogPart1(count);

        count = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(LONG_DAYS - fish - 1));
        AoCUtils.LogPart2(count);
    }

    /// <summary>
    /// Calculates how many descendants a fish will have
    /// </summary>
    /// <param name="timeRemaining">Amount of time remaining to final count date</param>
    /// <returns>The amount of descendants a fish will have</returns>
    private static long CalculateDescendantsCount(int timeRemaining)
    {
        if (timeRemaining < 0L) return 0L;
        if (cache.TryGetValue(timeRemaining, out long children)) return children;

        int spawned = (timeRemaining / 7) + 1;
        children = spawned;
        for (int timer = timeRemaining - 9; timer >= 0; timer -= 7)
        {
            children += CalculateDescendantsCount(timer);
        }

        cache.Add(timeRemaining, children);
        return children;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split(',').ConvertAll(int.Parse);
    #endregion
}
