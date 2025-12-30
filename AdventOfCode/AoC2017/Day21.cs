using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 21
/// </summary>
public sealed partial class Day21 : Solver<FrozenDictionary<Grid<bool>, Grid<bool>>>
{
    private sealed class PatternComparer : IEqualityComparer<Grid<bool>>
    {
        public static PatternComparer Instance { get; } = new();

        private PatternComparer() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Grid<bool>? x, Grid<bool>? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            if (x.Width != y.Width || x.Height != y.Height) return false;
            return x.SequenceEqual(y.AsValueEnumerable());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(Grid<bool> obj) => obj.Aggregate(0, HashCode.Combine);
    }

    private const char ON  = '#';
    private const int PART1 = 5;
    private const int PART2 = 18;

    [GeneratedRegex("([.#/]+) => ([.#/]+)")]
    private static partial Regex PatternMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> current = new(3, 3, b => b ? "#" : ".")
        {
            [1, 0] = true,
            [2, 1] = true,
            [0, 2] = true,
            [1, 2] = true,
            [2, 2] = true
        };

        foreach (int _ in ..PART1)
        {
            UpdateArt(ref current);
        }
        AoCUtils.LogPart1(current.AsValueEnumerable().Count(true));

        foreach (int _ in PART1..PART2)
        {
            UpdateArt(ref current);
        }
        AoCUtils.LogPart2(current.AsValueEnumerable().Count(true));
    }

    private void UpdateArt(ref Grid<bool> current)
    {
        int chunkSize = current.Width.IsEven ? 2 : 3;
        int chunkCount = current.Width / chunkSize;
        int combinedSize = chunkCount * (chunkSize + 1);
        using PooledArray<Grid<bool>> chunks = current.Chunk(chunkSize)
                                                      .Select(c => this.Data[c])
                                                      .ToArrayPool();
        current = Grid<bool>.CombineChunks(chunks.ArraySegment, combinedSize);
    }

    /// <inheritdoc />
    protected override FrozenDictionary<Grid<bool>, Grid<bool>> Convert(string[] rawInput)
    {
        (string, string)[] rawPatterns = RegexFactory<(string, string)>.ConstructObjects(PatternMatcher, rawInput);
        Dictionary<Grid<bool>, Grid<bool>> patterns = new(rawInput.Length, PatternComparer.Instance);
        foreach ((string fromPattern, string toPattern) in rawPatterns)
        {
            string[] lines = fromPattern.Split('/');
            int size = lines.Length;
            Grid<bool> from = new(size, size, lines, s => s.Select(c => c is ON).ToArray(), b => b ? "#" : ".");

            lines = toPattern.Split('/');
            size = lines.Length;
            Grid<bool> to = new(size, size, lines, s => s.Select(c => c is ON).ToArray(), b => b ? "#" : ".");

            // Add from pattern in all rotations
            patterns.Add(from, to);
            patterns.TryAdd(from.RotateRight(), to);
            patterns.TryAdd(from.RotateHalf(), to);
            patterns.TryAdd(from.RotateLeft(), to);

            // Flip horizontally, then add in all rotations as well
            Grid<bool> fromFlipped = from.FlipHorizontal();
            patterns.TryAdd(fromFlipped, to);
            patterns.TryAdd(fromFlipped.RotateRight(), to);
            patterns.TryAdd(fromFlipped.RotateHalf(), to);
            patterns.TryAdd(fromFlipped.RotateLeft(), to);
        }
        return patterns.ToFrozenDictionary(PatternComparer.Instance);
    }
}
