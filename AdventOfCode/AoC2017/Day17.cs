using Challenge.Solvers;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 17
/// </summary>
[Solver(2017, 17)]
public sealed class Day17 : Solver<int>
{
    private const int PART1 = 2017;
    private const int PART2 = 50_000_000;

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int position = 0;
        List<int> buffer = new(PART1 + 1) { 0 };
        foreach (int value in 1..^PART1)
        {
            position = (position + this.Data) % buffer.Count;
            buffer.Insert(position + 1, value);
            position++;
        }
        int result = buffer[(position + 1) % buffer.Count];
        LogAnswer(result);

        int size = PART1 + 1;
        foreach (int value in ^PART1..^PART2)
        {
            position = (position + this.Data) % size;
            if (position is 0)
            {
                result = value;
            }

            position++;
            size++;
        }
        LogAnswer(result);
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
