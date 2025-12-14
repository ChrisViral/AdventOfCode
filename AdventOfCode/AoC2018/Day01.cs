using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2025 Day 01
/// </summary>
public sealed class Day01 : ArraySolver<int>
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
        int frequency = 0;
        HashSet<int> frequencies = new(100) { 0 };
        foreach (int n in this.Data)
        {
            frequency += n;
            frequencies.Add(frequency);
        }
        AoCUtils.LogPart1(frequency);

        int i = 0;
        do
        {
            frequency += this.Data[i++];
            i %= this.Data.Length;
        }
        while (frequencies.Add(frequency));
        AoCUtils.LogPart2(frequency);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
