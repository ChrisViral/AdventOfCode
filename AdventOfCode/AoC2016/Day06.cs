using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 6
/// </summary>
[Solver(2016, 6)]
public sealed partial class Day06 : ArraySolver<string>
{

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
        LogAnswer(mostLikely.ToString());
        LogAnswer(leastLikely.ToString());
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
