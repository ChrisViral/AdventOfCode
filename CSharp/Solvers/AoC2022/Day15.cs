using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 15
/// </summary>
public sealed class Day15 : ArraySolver<(Vector2<int> sensor, int distance)>
{
    /// <summary> Search level for part 1 </summary>
    private const int LEVEL = 2000000;
    /// <summary> Axis limit for part 2 </summary>
    private const int LIMIT = 4000000;
    /// <summary> Input parsing pattern </summary>
    private static readonly Regex pattern = new(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)", RegexOptions.Compiled);

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver for 2022 - 15 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<int> invalids = [];
        foreach ((Vector2<int> sensor, int distance) in this.Data)
        {
            int verticalDistance = Math.Abs(sensor.Y - LEVEL);
            int range = distance - verticalDistance;
            int limit = sensor.X + range;
            for (int x = sensor.X - range; x < limit; x++)
            {
                invalids.Add(x);
            }
        }

        AoCUtils.LogPart1(invalids.Count);

        //Parallel.For(0, LIMIT + 1, () => new int[LIMIT + 1], CheckRow, null);

        long frequency = 0L;
        foreach ((Vector2<int> sensor, int distance) in this.Data)
        {
            if (FindLocation(sensor, distance, ref frequency)) break;
        }

        AoCUtils.LogPart2(frequency);
    }

    /* I'm leaving this brute force brilliance in for the sole reason that it actually fucking worked lmao
    private int[] CheckRow(int y, ParallelLoopState state, int[] space)
    {
        if (state.IsStopped) return space;

        foreach ((Vector2<int> sensor, int distance) in this.Data)
        {
            int verticalDistance = Math.Abs(sensor.Y - y);
            int range = distance - verticalDistance;
            if (range <= 0) continue;

            int start = Math.Clamp(sensor.X - range, 0, LIMIT);
            int limit = Math.Clamp(sensor.X + range, 0, LIMIT);
            space[start]++;
            space[limit]--;
        }

        int running = 0;
        foreach (int x in ..SIZE)
        {
            bool wasZero = running is 0;
            running += space[x];
            space[x] = 0;
            if (wasZero && running is 0)
            {
                found = new(x, y);
                state.Stop();
                return space;
            }
        }

        int count = Interlocked.Increment(ref this.rowsChecked);
        if (count.IsMultiple(1000)) AoCUtils.Log(count);

        return space;
    }
    */

    private bool FindLocation(Vector2<int> sensor, int distance, ref long result)
    {
        // Look one out from each sensor max distance
        foreach (Vector2<int> position in Vector2<int>.EnumerateAtDistance(sensor, distance + 1))
        {
            // Make sure we're within bounds
            if (position.X is < 0 or > LIMIT || position.Y is < 0 or > LIMIT) continue;

            // Check that all sensors are out of bounds
            if (this.Data.TrueForAll(t => Vector2<int>.ManhattanDistance(t.sensor, position) > t.distance))
            {
                result = (position.X * (long)LIMIT) + position.Y;
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector2<int>, int) ConvertLine(string line)
    {
        int[] values = pattern.Match(line).CapturedGroups
                              .Select(g => int.Parse(g.ValueSpan))
                              .ToArray();
        Vector2<int> sensor = new(values[0], values[1]);
        int distance = Vector2<int>.ManhattanDistance(sensor, new Vector2<int>(values[2], values[3]));
        return (sensor, distance);
    }
}
