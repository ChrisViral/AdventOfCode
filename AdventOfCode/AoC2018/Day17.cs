using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 17
/// </summary>
public sealed partial class Day17 : Solver<(Grid<bool> map, Vector2<int> offset)>
{
    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    [GeneratedRegex(@"([xy])=(\d+), [xy]=(\d+)\.\.(\d+)")]
    private static partial Regex LineMatcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1("");
        AoCUtils.LogPart2("");
    }

    /// <inheritdoc />
    protected override (Grid<bool>, Vector2<int>) Convert(string[] rawInput)
    {
        HashSet<Vector2<int>> clay = new(1000);
        foreach (string line in rawInput)
        {
            Match lineMatch = LineMatcher.Match(line);
            int value = int.Parse(lineMatch.Groups[2].ValueSpan);
            int from  = int.Parse(lineMatch.Groups[3].ValueSpan);
            int to    = int.Parse(lineMatch.Groups[4].ValueSpan);

            if (lineMatch.Groups[1].ValueSpan[0] is 'x')
            {
                foreach (int y in from..^to)
                {
                    clay.Add(new Vector2<int>(value, y));
                }
            }
            else
            {
                foreach (int x in from..^to)
                {
                    clay.Add(new Vector2<int>(x, value));
                }
            }
        }

        Vector2<int> min = clay.Aggregate(Vector2<int>.Min);
        Vector2<int> max = clay.Aggregate(Vector2<int>.Max);
        Vector2<int> size = max - min + Vector2<int>.One;
        Grid<bool> map = new(size.X, size.Y, v => v ? "#" : ".");
        clay.ForEach(c => map[c - min] = true);
        return (map, min);
    }
}
