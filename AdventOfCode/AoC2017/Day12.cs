using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 12
/// </summary>
public sealed partial class Day12 : Solver<FrozenDictionary<int, Day12.Program>>
{
    public sealed class Program(int id, string pipes)
    {
        public int ID { get; } = id;

        public ImmutableArray<int> Pipes { get; } = [..pipes.Split(", ").ConvertAll(int.Parse)];
    }

    [GeneratedRegex(@"(\d+) <-> ([\d, ]+)")]
    private static partial Regex ProgramMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<int> ungrouped = new(this.Data.Keys);
        RemoveGrouped(0, ungrouped);
        AoCUtils.LogPart1(this.Data.Count - ungrouped.Count);

        int groups = 1;
        while (!ungrouped.IsEmpty)
        {
            RemoveGrouped(ungrouped.First(), ungrouped);
            groups++;
        }
        AoCUtils.LogPart2(groups);
    }

    private void RemoveGrouped(int rootID, HashSet<int> ungrouped)
    {
        ungrouped.Remove(rootID);
        using Pooled<Queue<int>> toCheck = QueueObjectPool<int>.Shared.Get();
        toCheck.Ref.Enqueue(rootID);
        while (toCheck.Ref.TryDequeue(out int current))
        {
            foreach (int connection in this.Data[current].Pipes)
            {
                if (ungrouped.Remove(connection))
                {
                    toCheck.Ref.Enqueue(connection);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override FrozenDictionary<int, Program> Convert(string[] rawInput)
    {
        Program[] programs = RegexFactory<Program>.ConstructObjects(ProgramMatcher, rawInput);
        return programs.ToFrozenDictionary(p => p.ID, p => p);
    }
}
