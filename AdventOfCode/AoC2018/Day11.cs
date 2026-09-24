using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 11
/// </summary>
[Solver(2018, 11)]
public sealed class Day11 : Solver<int>
{
    private const int SIZE = 300;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<int> grid = new(SIZE, SIZE, i => $"{i} ");
        foreach (Vector2<int> position in grid.Dimensions.Enumerate())
        {
            int id = position.X + 11;
            int power = id * (position.Y + 1);
            power += this.Data;
            power *= id;
            power /= 100;
            power %= 10;
            power -= 5;
            grid[position] = power;
        }

        (int bestPower, Vector2<int> bestStart) = FindBestStart(grid, 3);
        LogAnswer($"{bestStart.X + 1},{bestStart.Y + 1}");

        int bestSize = 3;
        Lock locker = new();
        Parallel.For(4, SIZE, size =>
        {
            (int power, Vector2<int> start) = FindBestStart(grid, size);
            lock (locker)
            {
                if (bestPower < power)
                {
                    bestPower = power;
                    bestStart = start;
                    bestSize = size;
                }
            }
        });
        LogAnswer($"{bestStart.X + 1},{bestStart.Y + 1},{bestSize}");
    }

    private static (int, Vector2<int>) FindBestStart(Grid<int> grid, int regionSize)
    {
        int bestPower = int.MinValue;
        Vector2<int> bestStart = Vector2<int>.Zero;
        foreach (Vector2<int> start in Vector2<int>.EnumerateOver(grid.Width - regionSize, grid.Height - regionSize))
        {
            int power = 0;
            foreach (Vector2<int> position in Vector2<int>.EnumerateOver(regionSize, regionSize))
            {
                power += grid[start + position];
            }

            if (bestPower < power)
            {
                bestPower = power;
                bestStart = start;
            }
        }
        return (bestPower, bestStart);
    }
    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
