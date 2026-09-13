using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Regexes;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 25
/// </summary>
public sealed partial class Day25 : Solver<(int row, int column)>
{
    private const long FIRST_CODE = 20151125L;
    private const long MULTIPLIER = 252533L;
    private const long MODULO     = 33554393L;

    [GeneratedRegex(@"Enter the code at row (\d+), column (\d+).")]
    private static partial Regex RowColumnMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int position = (this.Data.column + this.Data.row - 1).Triangular
                     - (this.Data.row - 1).Triangular
                     + (this.Data.row - 2).Triangular;

        long value = FIRST_CODE;
        foreach (int _ in 1..position)
        {
            value = (value * MULTIPLIER) % MODULO;
        }
        AoCUtils.LogPart1(value);
    }

    /// <inheritdoc />
    protected override (int, int) Convert(string[] rawInput)
    {
        Span<int> matches = stackalloc int[2];
        RowColumnMatcher.Match(rawInput[0])
                     .CapturedGroups
                     .Select(g => int.Parse(g.ValueSpan))
                     .CopyTo(matches);
        return (matches[0], matches[1]);
    }
}
