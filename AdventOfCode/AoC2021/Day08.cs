using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 08
/// </summary>
public sealed class Day08 : ArraySolver<(string[] signals, string[] outputs)>
{
    /// <summary>Signal/output separator</summary>
    private static readonly char[] SeparatorSplit = ['|'];
    /// <summary>Segments separator</summary>
    private static readonly char[] SegmentSplit   = [' '];

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver for 2021 - 08 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Count outputs with known segments length
        int count = this.Data.SelectMany(data => data.outputs).Count(segments => segments.Length is 2 or 3 or 4 or 7);
        AoCUtils.LogPart1(count);

        // Create sets for every digit
        long total = 0L;
        HashSet<char>[] values = new HashSet<char>[10];
        values.Fill(() => new HashSet<char>(7));
        foreach ((string[] signals, string[] outputs) in this.Data)
        {
            // Simple values
            values[1] = new HashSet<char>(signals.Find(signal => signal.Length is 2)!);
            values[4] = new HashSet<char>(signals.Find(signal => signal.Length is 4)!);
            values[7] = new HashSet<char>(signals.Find(signal => signal.Length is 3)!);
            values[8] = new HashSet<char>(signals.Find(signal => signal.Length is 7)!);

            // More complex deductions
            values[3] = new HashSet<char>(signals.Find(signal => signal.Length is 5 && values[1].IsProperSubsetOf(signal))!);
            values[9] = new HashSet<char>(signals.Find(signal => signal.Length is 6 && values[4].IsProperSubsetOf(signal))!);
            values[0] = new HashSet<char>(signals.Find(signal => signal.Length is 6 && !values[9].SetEquals(signal) && values[1].IsProperSubsetOf(signal))!);
            values[6] = new HashSet<char>(signals.Find(signal => signal.Length is 6 && !values[9].SetEquals(signal) && !values[0].SetEquals(signal))!);
            values[5] = new HashSet<char>(signals.Find(signal => signal.Length is 5 && !values[3].SetEquals(signal) && values[9].Count(signal.Contains) is 5)!);
            values[2] = new HashSet<char>(signals.Find(signal => signal.Length is 5 && !values[3].SetEquals(signal) && !values[5].SetEquals(signal))!);

            // Create output value
            int final = values.FindIndex(value => value.SetEquals(outputs[0])) * 1000;
            final    += values.FindIndex(value => value.SetEquals(outputs[1])) * 100;
            final    += values.FindIndex(value => value.SetEquals(outputs[2])) * 10;
            final    += values.FindIndex(value => value.SetEquals(outputs[3]));
            total    += final;

            // Clear all
            values.ForEach(value => value.Clear());
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc />
    protected override (string[], string[]) ConvertLine(string line)
    {
        string[] splits = line.Split(SeparatorSplit, DEFAULT_OPTIONS);
        return (splits[0].Split(SegmentSplit, DEFAULT_OPTIONS), splits[1].Split(SegmentSplit, DEFAULT_OPTIONS));
    }
}
