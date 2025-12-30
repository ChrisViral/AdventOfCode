using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 22
/// </summary>
public sealed class Day22 : Solver<SparseGrid<Day22.Infection>>
{
    public enum Infection
    {
        CLEAN,
        WEAKENED,
        INFECTED,
        FLAGGED
    }

    private const char INFECTED = '#';
    private const int PART1 = 10_000;
    private const int PART2 = 10_000_000;

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        SparseGrid<Infection> map = new(this.Data);
        int infections = 0;
        Vector2<int> position = Vector2<int>.Zero;
        Direction direction = Direction.UP;
        foreach (int _ in ..PART1)
        {
            bool isInfected = map[position] is Infection.INFECTED;
            if (isInfected)
            {
                direction = direction.TurnRight();
                map[position] = Infection.CLEAN;
            }
            else
            {
                direction = direction.TurnLeft();
                map[position] = Infection.INFECTED;
                infections++;
            }

            position += direction;
        }
        AoCUtils.LogPart1(infections);

        infections = 0;
        position = Vector2<int>.Zero;
        direction = Direction.UP;
        map = new SparseGrid<Infection>(this.Data);
        foreach (int _ in ..PART2)
        {
            Infection infectionState = map[position];
            switch (infectionState)
            {
                case Infection.CLEAN:
                    direction = direction.TurnLeft();
                    map[position] = Infection.WEAKENED;
                    break;

                case Infection.WEAKENED:
                    map[position] = Infection.INFECTED;
                    infections++;
                    break;

                case Infection.INFECTED:
                    direction = direction.TurnRight();
                    map[position] = Infection.FLAGGED;
                    break;

                case Infection.FLAGGED:
                    direction = direction.Invert();
                    map[position] = Infection.CLEAN;
                    break;
            }

            position += direction;
        }
        AoCUtils.LogPart2(infections);
    }

    /// <inheritdoc />
    protected override SparseGrid<Infection> Convert(string[] rawInput)
    {
        int width = rawInput[0].Length;
        int height = rawInput.Length;
        SparseGrid<Infection> grid = new(width * height, Infection.CLEAN);
        Vector2<int> center = new(width / 2, height / 2);
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(width, height))
        {
            grid[position - center] = rawInput[position.Y][position.X] is INFECTED ? Infection.INFECTED : Infection.CLEAN;
        }
        return grid;
    }
}
