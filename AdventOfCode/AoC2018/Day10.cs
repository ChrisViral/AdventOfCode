using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 10
/// </summary>
public sealed partial class Day10 : RegexSolver<Day10.Light>
{
    public sealed class Light(Vector2<int> position, Vector2<int> velocity)
    {
        public Vector2<int> Position { get; private set; } = position;

        public Vector2<int> Velocity { get; } = velocity;

        public void Update() => this.Position += this.Velocity;

        public void Revert() => this.Position -= this.Velocity;
    }

    [GeneratedRegex(@"position=<([\d\-, ]+)> velocity=<([\d\-, ]+)>")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int iterations = 0;
        long previousArea;
        long currentArea = long.MaxValue;
        Vector2<int> min, max;
        do
        {
            previousArea = currentArea;
            this.Data.ForEach(l => l.Update());
            (min, max) = GetMinMax();
            currentArea = Vector2<int>.Area<long>(min, max);
            iterations++;
        }
        while (currentArea < previousArea);

        this.Data.ForEach(l => l.Revert());
        (min, max) = GetMinMax();
        Vector2<int> size = Vector2<int>.Abs(max - min) + Vector2<int>.One;
        Grid<bool> grid = new(size.X, size.Y, b => b ? "▓" : " ");
        this.Data.ForEach(l => grid[l.Position - min] = true);
        AoCUtils.LogPart1($"\n{grid}");
        AoCUtils.LogPart2(iterations - 1);
    }

    private (Vector2<int>, Vector2<int>) GetMinMax()
    {
        return (this.Data.Select(l => l.Position).Aggregate(Vector2<int>.Min),
                this.Data.Select(l => l.Position).Aggregate(Vector2<int>.Max));
    }
}
