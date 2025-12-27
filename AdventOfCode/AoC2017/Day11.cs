using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Enums;
using FastEnumUtility;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 11
/// </summary>
public sealed class Day11 : Solver<Day11.HexDirection[]>
{
    public enum HexDirection
    {
        N,
        S,
        NE,
        NW,
        SE,
        SW
    }

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int distance    = 0;
        int maxDistance = 0;
        Vector2<int> position = Vector2<int>.Zero;
        foreach (HexDirection hexDirection in this.Data)
        {
            position += hexDirection switch
            {
                HexDirection.N  => new Vector2<int>( 0, -2),
                HexDirection.S  => new Vector2<int>( 0,  2),
                HexDirection.NE => new Vector2<int>( 1, -1),
                HexDirection.NW => new Vector2<int>(-1, -1),
                HexDirection.SE => new Vector2<int>( 1,  1),
                HexDirection.SW => new Vector2<int>(-1,  1),
                _               => throw hexDirection.Invalid()
            };

            distance = HexDistance(position);
            maxDistance = Math.Max(maxDistance, distance);
        }

        AoCUtils.LogPart1(distance);
        AoCUtils.LogPart2(maxDistance);
    }

    private static int HexDistance(Vector2<int> position)
    {
        position = Vector2<int>.Abs(position);
        return position.X + Math.Max(0, (position.Y - position.X) / 2);
    }

    /// <inheritdoc />
    protected override HexDirection[] Convert(string[] rawInput) => rawInput[0].Split(',')
                                                                               .ConvertAll(d => FastEnum.Parse<HexDirection>(d, ignoreCase: true));
}
