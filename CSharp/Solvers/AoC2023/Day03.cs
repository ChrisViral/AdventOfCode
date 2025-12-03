using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 03
/// </summary>
public class Day03 : GridSolver<char>
{
    private const char EMPTY = '.';
    private const char GEAR = '*';

    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="char"/> fails</exception>
    public Day03(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        HashSet<Vector2<int>> explored = [];
        Dictionary<Vector2<int>, List<int>> gears = new();
        foreach (Vector2<int> pos in Vector2<int>.Enumerate(this.Data.Width, this.Data.Height))
        {
            char value = this.Data[pos];
            if (value is EMPTY || char.IsNumber(value)) continue;

            List<int>? numbers = null;
            if (value is GEAR)
            {
                numbers    = [];
                gears[pos] = numbers;
            }

            foreach (Vector2<int> adjacent in pos.Adjacent(true))
            {
                if (explored.Contains(adjacent) || !this.Data.WithinGrid(adjacent)) continue;

                char c = this.Data[adjacent];
                if (!char.IsNumber(c)) continue;

                Vector2<int> current = adjacent + Vector2<int>.Left;
                while (IsValid(current)) current += Vector2<int>.Left;

                int number = 0;
                current += Vector2<int>.Right;
                do
                {
                    number *= 10;
                    number += this.Data[current] - '0';
                    explored.Add(current);
                    current += Vector2<int>.Right;
                }
                while (IsValid(current));

                total += number;
                numbers?.Add(number);
            }
        }
        AoCUtils.LogPart1(total);

        long gearRatio = gears.Values.Where(n => n.Count is 2).Sum(n => n[0] * n[1]);
        AoCUtils.LogPart2(gearRatio);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValid(Vector2<int> pos) => this.Data.WithinGrid(pos) && char.IsNumber(this.Data[pos]);

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
