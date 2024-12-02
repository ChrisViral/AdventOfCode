﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 10
/// </summary>
public class Day11 : GridSolver<bool>
{
    private const char GALAXY = '#';
    private const int OLD_EXPANSION = 1_000_000;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day11(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Vector2<int>[] galaxies = Vector2<int>.Enumerate(this.Data.Width, this.Data.Height)
                                              .Where(p => this.Data[p])
                                              .ToArray();

        bool[] buffer = new bool[this.Data.Height];
        HashSet<int> emptyColumns = new();
        foreach (int x in ..this.Data.Width)
        {
            this.Data.GetColumnNoAlloc(x, buffer);
            if (buffer.Exists(b => b)) continue;

            emptyColumns.Add(x);
        }

        buffer = new bool[this.Data.Width];
        HashSet<int> emptyRows = new();
        foreach (int y in ..this.Data.Height)
        {
            this.Data.GetRowNoAlloc(y, buffer);
            if (buffer.Exists(b => b)) continue;

            emptyRows.Add(y);
        }

        long total = GetTotalDistances(galaxies, emptyRows, emptyColumns);
        AoCUtils.LogPart1(total);

        total = GetTotalDistances(galaxies, emptyRows, emptyColumns, OLD_EXPANSION);
        AoCUtils.LogPart2(total);
    }

    public long GetTotalDistances(Vector2<int>[] galaxies, HashSet<int> emptyRows, HashSet<int> emptyColumns, int emptyExpansion = 2)
    {
        long total = 0L;

        foreach (int i in ..(galaxies.Length - 1))
        {
            Vector2<int> first = galaxies[i];
            foreach (int j in ^i..galaxies.Length)
            {
                Vector2<int> second = galaxies[j];
                int distance = CalculateDistance(first.X, second.X, emptyColumns, emptyExpansion);
                distance += CalculateDistance(first.Y, second.Y, emptyRows, emptyExpansion);
                total += distance;
            }
        }

        return total;
    }

    public int CalculateDistance(int x1, int x2, HashSet<int> empty, int emptyExpansion)
    {
        if (x1 == x2) return 0;

        if (x1 > x2)
        {
            AoCUtils.Swap(ref x1, ref x2);
        }

        int distance = 0;
        for (int x = x1 + 1; x <= x2; x++)
        {
            distance += empty.Contains(x) ? emptyExpansion : 1;
        }

        return distance;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.Select(c => c is GALAXY).ToArray();
    #endregion
}