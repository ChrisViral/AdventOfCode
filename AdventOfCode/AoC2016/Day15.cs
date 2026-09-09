using System.Text.RegularExpressions;
using AdventOfCode.Maths;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 15
/// </summary>
public sealed partial class Day15 : RegexSolver<Day15.Disc>
{
    public readonly record struct Disc(int Index, int Positions, int Start);

    /// <inheritdoc />
    [GeneratedRegex(@"Disc #(\d+) has (\d+) positions; at time=0, it is at position (\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<int> remainders = stackalloc int[this.Data.Length + 1];
        Span<int> moduli     = stackalloc int[remainders.Length];
        for (int i = 0; i < this.Data.Length; i++)
        {
            Disc disc = this.Data[i];
            remainders[i] = -disc.Start - disc.Index;
            moduli[i] = disc.Positions;
        }

        int time = MathUtils.ChineseRemainder(remainders[..^1], moduli[..^1]);
        AoCUtils.LogPart1(time);

        // Add final disc and evaluate again
        remainders[^1] = this.Data.Length - 1;
        moduli[^1]     = 11;
        time = MathUtils.ChineseRemainder(remainders, moduli);
        AoCUtils.LogPart2(time);
    }
}
