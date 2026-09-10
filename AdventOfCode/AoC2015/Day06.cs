using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Spans;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 06
/// </summary>
public sealed partial class Day06 : RegexSolver<Day06.Instruction>
{
    public enum Change
    {
        ON,
        OFF,
        TOGGLE
    }

    public readonly record struct Instruction(Change Change, Vector2<int> From, Vector2<int> To);

    private const byte ON = 1;
    private const byte OFF = 0;
    private const int GRID_SIZE = 1000;

    /// <inheritdoc />
    [GeneratedRegex(@"(?:turn )?(on|off|toggle) (\d+,\d+) through (\d+,\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day06(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<byte> lights = new(GRID_SIZE, GRID_SIZE);
        foreach (Instruction instruction in this.Data)
        {
            Range xRange = instruction.From.X..(instruction.To.X + 1);
            Range yRange = instruction.From.Y..(instruction.To.Y + 1);
            Span2D<byte> region = lights[xRange, yRange];

            switch (instruction.Change)
            {
                case Change.ON:
                    region.Fill(ON);
                    break;

                case Change.OFF:
                    region.Fill(OFF);
                    break;

                case Change.TOGGLE:
                    region.Apply(l => l is ON ? OFF : ON);
                    break;

                default:
                    throw instruction.Change.Invalid();
            }
        }
        AoCUtils.LogPart1(lights.Count(ON));

        lights.Clear();
        foreach (Instruction instruction in this.Data)
        {
            Range xRange = instruction.From.X..(instruction.To.X + 1);
            Range yRange = instruction.From.Y..(instruction.To.Y + 1);
            Span2D<byte> region = lights[xRange, yRange];

            switch (instruction.Change)
            {
                case Change.ON:
                    region.Apply(l => (byte)(l + 1));
                    break;

                case Change.OFF:
                    region.Apply(l => (byte)(l is not 0 ? l - 1 : 0));
                    break;

                case Change.TOGGLE:
                    region.Apply(l => (byte)(l + 2));
                    break;

                default:
                    throw instruction.Change.Invalid();
            }
        }

        AoCUtils.LogPart2(lights.Sum(l => (int)l));
    }
}
