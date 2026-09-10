using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 02
/// </summary>
public sealed partial class Day02 : RegexSolver<Day02.Box>
{
    public readonly record struct Box(int Length, int Width, int Height)
    {
        public int TopArea { get; } = Length * Width;

        public int FrontArea { get; } = Width * Height;

        public int SideArea { get; } = Height * Length;

        public int Area { get; } = ((Length * Width) + (Width * Height) + (Height * Length)) * 2;

        public int Perimeter { get; } = (Length + Width + Height) * 2;

        public int Volume { get; } = (Length * Width * Height);
    }

    /// <inheritdoc />
    [GeneratedRegex(@"(\d+)x(\d+)x(\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day02(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int totalArea = this.Data.Sum(b => b.Area + int.Min(b.TopArea, b.FrontArea, b.SideArea));
        AoCUtils.LogPart1(totalArea);

        int totalLength = this.Data.Sum(b => b.Volume + b.Perimeter - (int.Max(b.Length, b.Width, b.Height) * 2));
        AoCUtils.LogPart2(totalLength);
    }
}
