using System.Text.RegularExpressions;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 07
/// </summary>
public sealed partial class Day07 : Solver<Day07.Step[]>
{
    public sealed record Step(char ID, HashSet<char> Requirements);

    public sealed class Worker
    {
        public char ID { get; set; }

        public int TimeRemaining { get; set; }

        public bool IsFree => this.TimeRemaining is 0;
    }

    private const int WORKER_COUNT = 5;

    [GeneratedRegex(@"Step ([A-Z]) must be finished before step ([A-Z]) can begin\.")]
    private static partial Regex StepMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<char> order = stackalloc char[StringUtils.LETTER_COUNT];
        HashSet<char> completed = new(order.Length);
        foreach (int i in ..order.Length)
        {
            foreach (int j in ..this.Data.Length)
            {
                Step step = this.Data[j];
                if (completed.Contains(step.ID)) continue;

                if (step.Requirements.IsSubsetOf(completed))
                {
                    order[i] = step.ID;
                    completed.Add(step.ID);
                    break;
                }
            }
        }

        AoCUtils.LogPart1(order.ToString());

        int time = 0;
        completed.Clear();
        HashSet<char> available = new(StringUtils.ALPHABET_UPPER);

        List<Worker> freeWorkers = Enumerable.Range(0, WORKER_COUNT)
                                             .Select(_ => new Worker())
                                             .ToList();
        List<Worker> busyWorkers = new(freeWorkers.Count);
        while (completed.Count is not StringUtils.LETTER_COUNT)
        {
            // Increment time
            time++;

            // Assign work
            for (int i = 0; i < this.Data.Length && !freeWorkers.IsEmpty; i++)
            {
                // Check if the step is ready to be assigned
                Step step = this.Data[i];
                if (!available.Contains(step.ID) || !step.Requirements.IsSubsetOf(completed)) continue;

                // Assign it to any free worker
                Worker worker = freeWorkers[^1];
                worker.ID = step.ID;
                worker.TimeRemaining = step.ID - StringUtils.ALPHABET_UPPER[0] + 61;
                busyWorkers.Add(worker);
                freeWorkers.RemoveAt(freeWorkers.Count - 1);
                available.Remove(step.ID);
            }

            // Process workers
            for (int i = 0; i < busyWorkers.Count; i++)
            {
                // Decrement workers' timer
                Worker worker = busyWorkers[i];
                worker.TimeRemaining--;
                if (!worker.IsFree) continue;

                // Move worker back to free list
                completed.Add(worker.ID);
                worker.ID = char.MinValue;
                freeWorkers.Add(worker);
                busyWorkers.RemoveAt(i--);
            }
        }

        AoCUtils.LogPart2(time);
    }

    /// <inheritdoc />
    protected override Step[] Convert(string[] rawInput)
    {
        Step[] steps = new Step[StringUtils.LETTER_COUNT];
        foreach (int i in ..steps.Length)
        {
            steps[i] = new Step(StringUtils.ALPHABET_UPPER[i], []);
        }

        char offset = StringUtils.ALPHABET_UPPER[0];
        foreach (int i in ..rawInput.Length)
        {
            Match match = StepMatcher.Match(rawInput[i]);
            Step step = steps[match.Groups[2].ValueSpan[0] - offset];
            step.Requirements.Add(match.Groups[1].ValueSpan[0]);
        }

        return steps;
    }
}
