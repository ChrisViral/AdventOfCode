using Challenge.Solvers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 1
/// </summary>
[Solver(2015, 1)]
public sealed class Day01 : Solver<int[]>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int floor = this.Data.Sum();
        LogAnswer(floor);

        floor = 0;
        foreach (int i in ..this.Data.Length)
        {
            floor += this.Data[i];
            if (floor < 0)
            {
                LogAnswer(i + 1);
                return;
            }
        }
    }

    /// <inheritdoc />
    protected override int[] Convert(string[] rawInput) => rawInput[0].AsValueEnumerable()
                                                                      .Select(c => c is '(' ? 1 : -1)
                                                                      .ToArray();
}
