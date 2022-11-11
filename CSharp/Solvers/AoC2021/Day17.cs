using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 17
/// </summary>
public class Day17 : Solver<(Day17.Range xRange, Day17.Range yRange)>
{
    public readonly record struct Range(int From, int To)
    {
        public bool IsInRange(int value) => value >= this.From && value <= this.To;
    }

    private const string PATTERN = @"target area: x=(-?\d+)\.\.(-?\d+), y=(-?\d+)\.\.(-?\d+)";

    #region Constructors
    /// <summary>7
    /// Creates a new <see cref="Day17"/> Solver for 2021 - 17 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1, T2}"/> fails</exception>
    public Day17(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        List<int> validY = new();
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

        AoCUtils.LogPart1(validY[^1].Triangular());

        int minX = (1..^this.Data.xRange.From).AsEnumerable()
                                            .First(n => n.Triangular() >= this.Data.xRange.From);

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

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Range, Range) Convert(string[] rawInput)
    {
        (int aX, int bX, int aY, int bY) = new RegexFactory<(int, int, int, int)>(PATTERN).ConstructObject(rawInput[0]);
        return (new(aX, bX), new(aY, bY));
    }
    #endregion
}
