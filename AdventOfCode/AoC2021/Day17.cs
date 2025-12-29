using System.Text.RegularExpressions;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 17
/// </summary>
public sealed partial class Day17 : Solver<(Day17.Range xRange, Day17.Range yRange)>
{
    public readonly record struct Range(int From, int To)
    {
        public bool IsInRange(int value) => value >= this.From && value <= this.To;
    }

    [GeneratedRegex(@"target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)")]
    private static partial Regex RangeMatcher { get; }

    /// <summary>7
    /// Creates a new <see cref="Day17"/> Solver for 2021 - 17 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<int> validY = [];
        for (int y = this.Data.yRange.From; y <= -this.Data.yRange.From; y++)
        {
            for (int yPos = 0, ySpeed = -y; yPos >= this.Data.yRange.From; ySpeed--, yPos += ySpeed)
            {
                if (this.Data.yRange.IsInRange(yPos))
                {
                    validY.Add(y);
                    break;
                }
            }
        }

        AoCUtils.LogPart1(validY[^1].Triangular);

        int minX = (1..^this.Data.xRange.From).AsEnumerable()
                                              .First(n => n.Triangular >= this.Data.xRange.From);

        int count = 0;
        foreach (int y in validY)
        {
            foreach (int x in minX..^this.Data.xRange.To)
            {
                Vector2<int> speed = (x, y);
                Vector2<int> position = Vector2<int>.Zero;
                while (position.Y >= this.Data.yRange.From)
                {
                    position += speed;
                    speed = (Math.Max(0, speed.X - 1), speed.Y - 1);
                    if (this.Data.xRange.IsInRange(position.X) && this.Data.yRange.IsInRange(position.Y))
                    {
                        count++;
                        break;
                    }
                }
            }
        }
        AoCUtils.LogPart2(count);
    }

    /// <inheritdoc />
    protected override (Range, Range) Convert(string[] rawInput)
    {
        (int aX, int bX, int aY, int bY) = new RegexFactory<(int, int, int, int)>(RangeMatcher).ConstructObject(rawInput[0]);
        return (new Range(aX, bX), new Range(aY, bY));
    }
}
