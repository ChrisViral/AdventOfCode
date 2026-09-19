using Challenge.Maths;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Numbers;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 20
/// </summary>
[Solver(2015, 20)]
public sealed class Day20 : Solver<int>
{
    private const int HOUSE_LIMIT = 50;

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int house = 3;
        for (int gifts = GetGiftsAtHouse(house); gifts < this.Data; gifts = GetGiftsAtHouse(++house));
        LogAnswer(house);

        house = 3;
        for (int gifts = GetGiftsAtHouseLimited(house, HOUSE_LIMIT); gifts < this.Data; gifts = GetGiftsAtHouseLimited(++house, HOUSE_LIMIT));
        LogAnswer(house);
    }

    private static int GetGiftsAtHouse(int house)
    {
        int gifts = 1 + house;
        int upperBound = MathUtils.FloorToInt<int, double>(Math.Sqrt(house));
        for (int i = 2; i <= upperBound; i++)
        {
            if (house.IsMultiple(i))
            {
                gifts += i;
                gifts += house / i;
            }
        }
        return gifts * 10;
    }

    private static int GetGiftsAtHouseLimited(int house, int limit)
    {
        int gifts = 0;
        int upperBound = MathUtils.FloorToInt<int, double>(Math.Sqrt(house));
        for (int i = 1; i <= upperBound; i++)
        {
            if (house.IsMultiple(i))
            {
                if (house <= i * limit)
                {
                    gifts += i;
                }

                int other = house / i;
                if (house <= other * limit)
                {
                    gifts += other;
                }
            }
        }
        return gifts * 11;
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
