using System;
using System.Linq;
using System.Numerics;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 06
/// </summary>
public class Day06 : Solver<(int time, int record)[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T, T}"/>[] fails</exception>
    public Day06(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {

        int result = this.Data.Aggregate(1, (r, c) => r * CountRecordBreaks(c.time, c.record));
        AoCUtils.LogPart1(result);

        long time   = long.Parse(this.Data.Select(d => d.time.ToString())
            .Aggregate((a, b) => a + b));
        long record = long.Parse(this.Data.Select(d => d.record.ToString())
            .Aggregate((a, b) => a + b));
        long total = CountRecordBreaks(time, record);
        AoCUtils.LogPart2(total);
    }

    public static T CountRecordBreaks<T>(T time, T record) where T : IBinaryInteger<T>
    {
        T total = T.Zero;
        for (T held = T.One; held < time; held++)
        {
            T distance = held * (time - held);
            if (distance > record)
            {
                total++;
            }
        }

        return total;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (int, int)[] Convert(string[] rawInput)
    {
        int[] times   = rawInput[0][5..].Split(' ', DEFAULT_OPTIONS).ConvertAll(int.Parse);
        int[] records = rawInput[1][9..].Split(' ', DEFAULT_OPTIONS).ConvertAll(int.Parse);

        (int, int)[] data = new (int, int)[times.Length];
        foreach (int i in ..data.Length)
        {
            data[i] = (times[i], records[i]);
        }
        return data;
    }
    #endregion
}
