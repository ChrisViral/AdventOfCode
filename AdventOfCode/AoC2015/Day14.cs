using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 14
/// </summary>
public sealed partial class Day14 : RegexSolver<Day14.Reindeer>
{
    public readonly record struct Reindeer(string Name, int Speed, int Active, int Rest)
    {
        public int CycleDistance { get; } = Speed * Active;

        public int CycleTime { get; } = Active + Rest;
    }

    private const int RACE_TIME = 2503;

    /// <inheritdoc />
    [GeneratedRegex(@"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int maxDistance = this.Data.Max(r => DistanceAtTime(RACE_TIME, r));
        AoCUtils.LogPart1(maxDistance);

        SpanList<int> leaders = stackalloc int[this.Data.Length];
        Span<int> points      = stackalloc int[this.Data.Length];
        foreach (int t in 1..^RACE_TIME)
        {
            leaders.Add(0);
            maxDistance = DistanceAtTime(t, this.Data[0]);
            foreach (int i in 1..this.Data.Length)
            {
                int distance = DistanceAtTime(t, this.Data[i]);
                if (maxDistance == distance)
                {
                    leaders.Add(i);
                }
                else if (maxDistance < distance)
                {
                    maxDistance = distance;
                    leaders.Clear();
                    leaders.Add(i);
                }
            }

            foreach (int i in leaders)
            {
                points[i]++;
            }

            leaders.Clear();
        }
        AoCUtils.LogPart2(points.Max());
    }

    private static int DistanceAtTime(int time, in Reindeer reindeer)
    {
        (int cycles, int timeLeft) = Math.DivRem(time, reindeer.CycleTime);
        return (cycles * reindeer.CycleDistance)
             + (timeLeft >= reindeer.Active ? reindeer.CycleDistance : timeLeft * reindeer.Speed);
    }
}
