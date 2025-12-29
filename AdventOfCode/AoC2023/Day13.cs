using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 13
/// </summary>
public sealed class Day13 : Solver<Grid<bool>[]>
{
    private const char ROCK = '#';

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input) : base(input, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        int[] lines = new int[this.Data.Length];
        foreach (int i in ..this.Data.Length)
        {
            Grid<bool> grid = this.Data[i];
            if (!GetReflectionColumn(grid, out int reflection))
            {
                GetReflectionRow(grid, out reflection);
                reflection *= 100;
            }

            total += reflection;
            lines[i] = reflection;
        }
        AoCUtils.LogPart1(total);

        total = 0;
        foreach (int i in ..this.Data.Length)
        {
            total += FindSmudgedReflection(this.Data[i], lines[i]);
        }
        AoCUtils.LogPart2(total);
    }

    private static int FindSmudgedReflection(Grid<bool> grid, int ignoredLine)
    {
        foreach (Vector2<int> pos in Vector2<int>.EnumerateOver(grid.Width, grid.Height))
        {
            grid[pos] = !grid[pos];

            if (GetReflectionColumn(grid, out int reflection, ignoredLine))
            {
                return reflection;
            }

            if (GetReflectionRow(grid, out reflection, ignoredLine / 100))
            {
                return reflection * 100;
            }

            grid[pos] = !grid[pos];
        }

        return -1;
    }

    private static bool GetReflectionColumn(Grid<bool> grid, out int reflection, int ignore = -1)
    {
        Span<bool> left  = stackalloc bool[grid.Height];
        Span<bool> right = stackalloc bool[grid.Height];

        Span<bool> rRight = stackalloc bool[grid.Height];
        Span<bool> rLeft  = stackalloc bool[grid.Height];
        grid.GetColumn(0, right);

        for (int i = 1; i < grid.Width; i++)
        {
            AoCUtils.SwapSpans(ref left, ref right);
            grid.GetColumn(i, right);
            if (i == ignore) continue;

            if (!left.SequenceEqual(right) || !IsReflected(i, ref rLeft, ref rRight)) continue;

            reflection = i;
            return true;
        }

        reflection = -1;
        return false;

        bool IsReflected(int i, ref Span<bool> l, ref Span<bool> r)
        {
            for (int a = i - 2, b = i + 1; a >= 0 && b < grid.Width; a--, b++)
            {
                grid.GetColumn(a, l);
                grid.GetColumn(b, r);
                if (!l.SequenceEqual(r)) return false;
            }

            return true;
        }
    }

    private static bool GetReflectionRow(Grid<bool> grid, out int reflection, int ignore = -1)
    {
        ReadOnlySpan<bool> up   = Span<bool>.Empty;
        ReadOnlySpan<bool> down = grid[0];

        for (int i = 1; i < grid.Height; i++)
        {
            AoCUtils.SwapSpans(ref up, ref down);
            down = grid[i];
            if (i == ignore) continue;

            if (!up.SequenceEqual(down) || !IsReflected(i)) continue;

            reflection = i;
            return true;
        }

        reflection = -1;
        return false;

        bool IsReflected(int i)
        {
            for (int a = i - 2, b = i + 1; a >= 0 && b < grid.Height; a--, b++)
            {
                ReadOnlySpan<bool> u = grid[a];
                ReadOnlySpan<bool> d = grid[b];
                if (!u.SequenceEqual(d)) return false;
            }

            return true;
        }
    }

    /// <inheritdoc />
    protected override Grid<bool>[] Convert(string[] rawInput)
    {
        List<Grid<bool>> grids = [];
        int end = rawInput.IndexOf(string.Empty);
        while (!rawInput.IsEmpty)
        {
            string[] input = rawInput[..end];
            int width = input[0].Length;
            int height = input.Length;
            grids.Add(new Grid<bool>(width, height, input, l => l.Select(c => c is ROCK).ToArray(), b => b ? "#" : "."));
            rawInput = rawInput[(end + 1)..];
            end = rawInput.IndexOf(string.Empty);
        }
        return grids.ToArray();
    }
}
