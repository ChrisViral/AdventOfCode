using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Enumerables;
using Challenge.Utils.Extensions.Enums;
using Challenge.Utils.Extensions.Spans;
using CommunityToolkit.HighPerformance;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 6
/// </summary>
[Solver(2015, 6)]
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
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day06(string input, ILogger logger) : base(input, logger) { }

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
        LogAnswer(lights.Count(ON));

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

        LogAnswer(lights.Sum(l => (int)l));
    }
}
