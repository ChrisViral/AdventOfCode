using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 13
/// </summary>
public class Day13 : Solver<Grid<bool>[]>
{
    private const char ROCK = '#';
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day13(string input) : base(input, options: StringSplitOptions.TrimEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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

    public int FindSmudgedReflection(Grid<bool> grid, int ignoredLine)
    {
        foreach (Vector2<int> pos in Vector2<int>.Enumerate(grid.Width, grid.Height))
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

    public bool GetReflectionColumn(Grid<bool> grid, out int reflection, int ignore = -1)
    {
        Span<bool> left  = stackalloc bool[grid.Height];
        Span<bool> right = stackalloc bool[grid.Height];

        Span<bool> rRight = stackalloc bool[grid.Height];
        Span<bool> rLeft  = stackalloc bool[grid.Height];
        grid.GetColumnNoAlloc(0, ref right);

        for (int i = 1; i < grid.Width; i++)
        {
            AoCUtils.SwapSpans(ref left, ref right);
            grid.GetColumnNoAlloc(i, ref right);
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
                grid.GetColumnNoAlloc(a, ref l);
                grid.GetColumnNoAlloc(b, ref r);
                if (!l.SequenceEqual(r)) return false;
            }

            return true;
        }
    }

    public bool GetReflectionRow(Grid<bool> grid, out int reflection, int ignore = -1)
    {
        Span<bool> up   = stackalloc bool[grid.Width];
        Span<bool> down = stackalloc bool[grid.Width];

        Span<bool> rUp    = stackalloc bool[grid.Width];
        Span<bool> rDown  = stackalloc bool[grid.Width];
        grid.GetRowNoAlloc(0, ref down);

        for (int i = 1; i < grid.Height; i++)
        {
            AoCUtils.SwapSpans(ref up, ref down);
            grid.GetRowNoAlloc(i, ref down);
            if (i == ignore) continue;

            if (!up.SequenceEqual(down) || !IsReflected(i, ref rDown, ref rUp)) continue;

            reflection = i;
            return true;
        }

        reflection = -1;
        return false;

        bool IsReflected(int i, ref Span<bool> u, ref Span<bool> d)
        {
            for (int a = i - 2, b = i + 1; a >= 0 && b < grid.Height; a--, b++)
            {
                grid.GetRowNoAlloc(a, ref u);
                grid.GetRowNoAlloc(b, ref d);
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
            grids.Add(new(width, height, input, l => l.Select(c => c is ROCK).ToArray(), b => b ? "#" : "."));
            rawInput = rawInput[(end + 1)..];
            end = rawInput.IndexOf(string.Empty);
        }
        return grids.ToArray();
    }
    #endregion
}
