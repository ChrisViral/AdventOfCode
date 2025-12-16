using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 04
/// </summary>
public sealed partial class Day04 : RegexSolver<Day04.Schedule>
{
    public enum ScheduleAction
    {
        START,
        SLEEP,
        WAKE
    }

    public readonly struct Schedule(string timestamp, string action) : IComparable<Schedule>
    {
        public DateTime Timestamp { get; } = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

        public int GuardID { get; }

        public ScheduleAction Action { get; } = action switch
        {
            "begins shift" => ScheduleAction.START,
            "falls asleep" => ScheduleAction.SLEEP,
            "wakes up"     => ScheduleAction.WAKE,
            _              => throw new ArgumentException("Invalid schedule action", nameof(action))
        };

        public Schedule(string timestamp, int guardId, string action) : this(timestamp, action)
        {
            this.GuardID = guardId;
        }

        /// <inheritdoc />
        public int CompareTo(Schedule other) => this.Timestamp.CompareTo(other.Timestamp);
    }

    [GeneratedRegex(@"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2})\] (?:Guard #(\d+) (begins shift)|(falls asleep)|(wakes up))")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.Data.Sort();
        int[] currentGuard = null!;
        Dictionary<int, int[]> guards = new(100);
        foreach (Schedule schedule in this.Data)
        {
            switch (schedule.Action)
            {
                case ScheduleAction.START:
                    if (!guards.TryGetValue(schedule.GuardID, out currentGuard!))
                    {
                        currentGuard = new int[60];
                        guards.Add(schedule.GuardID, currentGuard);
                    }
                    break;

                case ScheduleAction.SLEEP:
                    currentGuard[schedule.Timestamp.Minute]++;
                    break;

                case ScheduleAction.WAKE:
                    currentGuard[schedule.Timestamp.Minute]--;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(schedule.Action), (int)schedule.Action, typeof(ScheduleAction));
            }
        }

        // Accumulate the prefix array
        foreach (int[] guard in guards.Values)
        {
            int value = guard[0];
            foreach (int i in 1..guard.Length)
            {
                value   += guard[i];
                guard[i] =  value;
            }
        }

        (int id, int[] time) = guards.MaxBy(g => g.Value.Sum());
        int minuteIndex = time.Select((m, i) => (m, i))
                              .MaxBy(value => value.m).i;
        AoCUtils.LogPart1(id * minuteIndex);

        (id, (_, minuteIndex)) = guards.Select(g => (id: g.Key,
                                                     maxMin: g.Value
                                                              .Select((m, i) => (m, i))
                                                              .MaxBy(value => value.m)))
                                       .MaxBy(g => g.maxMin.m);
        AoCUtils.LogPart2(id * minuteIndex);
    }
}
