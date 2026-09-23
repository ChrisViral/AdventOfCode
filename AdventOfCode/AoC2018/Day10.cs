using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Arrays;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 10
/// </summary>
[Solver(2018, 10)]
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
        LogAnswer($"\n{grid}");
        LogAnswer(iterations - 1);
    }

    private (Vector2<int>, Vector2<int>) GetMinMax()
    {
        return (this.Data.Select(l => l.Position).Aggregate(Vector2<int>.Min),
                this.Data.Select(l => l.Position).Aggregate(Vector2<int>.Max));
    }
}
