using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Enumerables;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 03
/// </summary>
[Solver(2018, 3)]
public sealed partial class Day03 : RegexSolver<Day03.FabricArea>
{
    [GeneratedRegex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)")]
    protected override partial Regex Matcher { get; }

    public sealed class FabricArea(int id, int offsetX, int offsetY, int width, int height)
    {
        public int ID { get; } = id;

        public Vector2<int> Offset { get; } = new(offsetX, offsetY);

        public Vector2<int> Dimensions { get; } = new(width, height);

        public void SetClaim(Counter<Vector2<int>> claims)
        {
            this.Dimensions.Enumerate().ForEach(p => claims[p + this.Offset]++);
        }

        public bool Overlaps(Counter<Vector2<int>> claims)
        {
            return this.Dimensions.Enumerate().Any(p => claims[p + this.Offset] > 1);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Counter<Vector2<int>> claims = new(1000);
        this.Data.ForEach(a => a.SetClaim(claims));
        int overlaps = claims.Counts.Count(c => c >= 2);
        LogAnswer(overlaps);

        FabricArea notOverlapping = this.Data.First(a => !a.Overlaps(claims));
        LogAnswer(notOverlapping.ID);
    }
}
