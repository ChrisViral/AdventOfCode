using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 6
/// </summary>
[Solver(2021, 6)]
public sealed partial class Day06 : Solver<int[]>
{
    /// <summary>Part 1 days</summary>
    private const int DAYS = 80;
    /// <summary>Part 2 days</summary>
    private const int LONG_DAYS = 256;
    /// <summary>Fish spawn cache</summary>
    private static readonly Dictionary<int, long> Cache = new();

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long count = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(DAYS - fish - 1));
        LogAnswer(count);

        count      = this.Data.Length + this.Data.Sum(fish => CalculateDescendantsCount(LONG_DAYS - fish - 1));
        LogAnswer(count);
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

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].Split(',').ConvertAll(int.Parse);
}
