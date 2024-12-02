using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 08
/// </summary>
public class Day08 : ArraySolver<(string[] signals, string[] outputs)>
{
    #region Constants
    /// <summary>Signal/output separator</summary>
    private static readonly char[] separatorSplit = { '|' };
    /// <summary>Segments separator</summary>
    private static readonly char[] segmentSplit   = { ' ' };
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver for 2021 - 08 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day08(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Count outputs with known segments length
        int count = this.Data.SelectMany(data => data.outputs).Count(segments => segments.Length is 2 or 3 or 4 or 7);
        AoCUtils.LogPart1(count);

        // Create sets for every digit
        long total = 0L;
        HashSet<char>[] values = new HashSet<char>[10];
        values.Fill(() => new(7));
        foreach ((string[] signals, string[] outputs) in this.Data)
        {
            // Simple values
            values[1] = new(signals.Find(signal => signal.Length is 2)!);
            values[4] = new(signals.Find(signal => signal.Length is 4)!);
            values[7] = new(signals.Find(signal => signal.Length is 3)!);
            values[8] = new(signals.Find(signal => signal.Length is 7)!);

            // More complex deductions
            values[3] = new(signals.Find(signal => signal.Length is 5 && values[1].IsProperSubsetOf(signal))!);
            values[9] = new(signals.Find(signal => signal.Length is 6 && values[4].IsProperSubsetOf(signal))!);
            values[0] = new(signals.Find(signal => signal.Length is 6 && !values[9].SetEquals(signal) && values[1].IsProperSubsetOf(signal))!);
            values[6] = new(signals.Find(signal => signal.Length is 6 && !values[9].SetEquals(signal) && !values[0].SetEquals(signal))!);
            values[5] = new(signals.Find(signal => signal.Length is 5 && !values[3].SetEquals(signal) && values[9].Count(signal.Contains) is 5)!);
            values[2] = new(signals.Find(signal => signal.Length is 5 && !values[3].SetEquals(signal) && !values[5].SetEquals(signal))!);

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

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string[], string[]) ConvertLine(string line)
    {
        string[] splits = line.Split(separatorSplit, DEFAULT_OPTIONS);
        return (splits[0].Split(segmentSplit, DEFAULT_OPTIONS), splits[1].Split(segmentSplit, DEFAULT_OPTIONS));
    }
    #endregion
}
