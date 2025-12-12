using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 06
/// </summary>
public sealed class Day06 : Solver<int[]>
{
    /// <summary>Part 1 days</summary>
    private const int DAYS = 80;
    /// <summary>Part 2 days</summary>
    private const int LONG_DAYS = 256;
    /// <summary>Fish spawn cache</summary>
    private static readonly Dictionary<int, long> Cache = new();

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver for 2021 - 06 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long count = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(DAYS - fish - 1));
        AoCUtils.LogPart1(count);

        count      = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(LONG_DAYS - fish - 1));
        AoCUtils.LogPart2(count);
    }

    /// <summary>
    /// Calculates how many descendants a fish will have
    /// </summary>
    /// <param name="timeRemaining">Amount of time remaining to final count date</param>
    /// <returns>The amount of descendants a fish will have</returns>
    private static long CalculateDescendantsCount(int timeRemaining)
    {
        // Return if no time is left
        if (timeRemaining < 0L) return 0L;
        // Try to get from cache if possible
        if (Cache.TryGetValue(timeRemaining, out long children)) return children;

        // Get spawned amount during lifetime
        int spawned = (timeRemaining / 7) + 1;
        children = spawned;
        for (int timer = timeRemaining - 9; timer >= 0; timer -= 7)
        {
            // Get all descendants count for each child
            children += CalculateDescendantsCount(timer);
        }

        // Cache result
        Cache.Add(timeRemaining, children);
        return children;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split(',').ConvertAll(int.Parse);
}
