using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 01
/// </summary>
public sealed class Day01 : Solver<int[]>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int floor = this.Data.Sum();
        AoCUtils.LogPart1(floor);

        floor = 0;
        foreach (int i in ..this.Data.Length)
        {
            floor += this.Data[i];
            if (floor < 0)
            {
                AoCUtils.LogPart2(i + 1);
                return;
            }
        }
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].AsValueEnumerable()
                                                                      .Select(c => c is '(' ? 1 : -1)
                                                                      .ToArray();
}
