using AdventOfCode.AoC2017.Common;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 14
/// </summary>
public sealed class Day14 : Solver<string>
{
    private const int SIZE = 128;

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
        // Setup for hashing
        Span<char> line = stackalloc char[this.Data.Length + 4];
        this.Data.CopyTo(line);
        line[^4] = '-';

        int used = 0;
        Grid<bool> disk = new(SIZE, SIZE);
        foreach (int i in ..SIZE)
        {
            i.TryFormat(line[^3..], out int written);
            BitVector128 hash = Knot.Hash(line[..^(3 - written)]);
            used += (int)UInt128.PopCount(hash.Data);
            hash.CopyTo(disk[i]);
        }
        AoCUtils.LogPart1(used);

        int groups = 0;
        HashSet<Vector2<int>> positions = Vector2<int>.EnumerateOver(SIZE, SIZE).ToHashSet();
        while (!positions.IsEmpty)
        {
            if (RemoveGroup(positions, disk))
            {
                groups++;
            }
        }
        AoCUtils.LogPart2(groups);
    }

    private static bool RemoveGroup(HashSet<Vector2<int>> unexplored, Grid<bool> disk)
    {
        Vector2<int> start = unexplored.First();
        unexplored.Remove(start);
        if (!disk[start]) return false;

        using Pooled<Queue<Vector2<int>>> toCheck = QueueObjectPool<Vector2<int>>.Shared.Get();
        toCheck.Ref.Enqueue(start);
        while (toCheck.Ref.TryDequeue(out Vector2<int> current))
        {
            foreach (Vector2<int> adjacent in current.Adjacent())
            {
                if (!unexplored.Remove(adjacent)) continue;

                if (disk[adjacent])
                {
                    toCheck.Ref.Enqueue(adjacent);
                }
            }
        }
        return true;
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
