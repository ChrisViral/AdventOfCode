using AdventOfCode.Collections;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 06
/// </summary>
public sealed class Day06 : ArraySolver<string>
{
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
        Counter<char> frequencies = new(26);
        Span<char> mostLikely  = stackalloc char[this.Data[0].Length];
        Span<char> leastLikely = stackalloc char[mostLikely.Length];
        foreach (int i in ..mostLikely.Length)
        {
            foreach (string line in this.Data)
            {
                frequencies.Add(line[i]);
            }

            mostLikely[i]  = frequencies.AsValueEnumerable().MaxBy(p => p.Value).Key;
            leastLikely[i] = frequencies.AsValueEnumerable().MinBy(p => p.Value).Key;
            frequencies.Clear();
        }
        AoCUtils.LogPart1(mostLikely.ToString());
        AoCUtils.LogPart2(leastLikely.ToString());
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
