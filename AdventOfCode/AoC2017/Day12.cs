using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Challenge.Collections.Pooling;
using Challenge.Solvers;
using Challenge.Utils;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 12
/// </summary>
[Solver(2017, 12)]
public sealed partial class Day12 : Solver<FrozenDictionary<int, Day12.Program>>
{
    public sealed class Program(int id, string pipes)
    {
        public int ID { get; } = id;

        public ImmutableArray<int> Pipes { get; } = [..pipes.Split(", ").ConvertAll(int.Parse)];
    }

    [GeneratedRegex(@"(\d+) <-> ([\d, ]+)")]
    private static partial Regex ProgramMatcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<int> ungrouped = new(this.Data.Keys);
        RemoveGrouped(0, ungrouped);
        LogAnswer(this.Data.Count - ungrouped.Count);

        int groups = 1;
        while (!ungrouped.IsEmpty)
        {
            RemoveGrouped(ungrouped.First(), ungrouped);
            groups++;
        }
        LogAnswer(groups);
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
