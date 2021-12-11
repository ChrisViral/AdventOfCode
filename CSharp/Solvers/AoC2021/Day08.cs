using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 08
/// </summary>
public class Day08 : ArraySolver<(string[] signals, string[] outputs)>
{
    private static readonly char[] separatorSplit = { '|' };
    private static readonly char[] segmentSplit   = { ' ' };

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver for 2021 - 08 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day08(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int count = this.Data.SelectMany(data => data.outputs).Count(segments => segments.Length is 2 or 3 or 4 or 7);
        AoCUtils.LogPart1(count);

        long total = 0L;
        HashSet<char>[] values = new HashSet<char>[10];
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
            //values[2] = new(signals.Find(signal => signal.Length is 5 && !values[3].SetEquals(signal) && signal != values[5])!);

            int final = values.FindIndex(value => value.Count == outputs[0].Length && outputs[0].All(value.Contains)) * 1000;
            final    += values.FindIndex(value => value.Count == outputs[1].Length && outputs[1].All(value.Contains)) * 100;
            final    += values.FindIndex(value => value.Count == outputs[2].Length && outputs[2].All(value.Contains)) * 10;
            final    += values.FindIndex(value => value.Count == outputs[3].Length && outputs[3].All(value.Contains));
            total    += final;
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
