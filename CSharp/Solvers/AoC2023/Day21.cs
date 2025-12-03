using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 21
/// </summary>
public class Day21 : Solver<(Grid<bool> garden, Vector2<int> start)>
{
    private const int STEPS = 64;
    private const int LONG_STEPS = 26501365;

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Stack<Vector2<int>> currentPositions   = [];
        Stack<Vector2<int>> nextPositions      = [];
        Dictionary<Vector2<int>, bool> visited = [];

        currentPositions.Push(this.Data.start);
        bool parity = false;
        foreach (int _ in ..STEPS)
        {
            while (currentPositions.TryPop(out Vector2<int> plot))
            {
                foreach (Vector2<int> adjacent in plot.Adjacent())
                {
                    if (visited.ContainsKey(adjacent) || !this.Data.garden[adjacent]) continue;

                    visited.Add(adjacent, parity);
                    nextPositions.Push(adjacent);
                }
            }

            AoCUtils.Swap(ref currentPositions, ref nextPositions);
            parity = !parity;
        }

        int current = visited.Values.Count(v => v == STEPS.IsEven);
        AoCUtils.LogPart1(current);

        int width = this.Data.garden.Width;
        int radius = width / 2;
        int end = radius + (width * 2);
        List<int> points = [];
        foreach (int n in ^STEPS..^end)
        {
            while (currentPositions.TryPop(out Vector2<int> plot))
            {
                foreach (Vector2<int> adjacent in plot.Adjacent())
                {
                    if (visited.ContainsKey(adjacent) || !CheckValidInfinite(adjacent)) continue;

                    visited.Add(adjacent, parity);
                    nextPositions.Push(adjacent);
                }
            }

            AoCUtils.Swap(ref currentPositions, ref nextPositions);
            parity = !parity;

            if ((n - radius).IsMultiple(width))
            {
                points.Add(visited.Values.Count(v => v == n.IsEven));
            }
        }

        if (points is not [int y0, int y1, int y2]) throw new UnreachableException("Invalid regression points");

        /*
         * Solving y = ax² + bx + c
         * Given we have x0 = 0, x1 = 1, x2 = 2, we get the following equations
         * y0 = c
         * y1 = a + b + c
         * y2 = 4a + 2b + c
         * By calculating y2 - 4y1, we can get the following equations for a, b, and c:
         * c = y0
         * b = (4y1 - y2 - 3c) / 2
         * a = y1 - b - c
         */
        int c = y0;
        int b = ((4 * y1) - y2 - (3 * c)) / 2;
        int a = y1 - b - c;

        long x = (LONG_STEPS - radius) / width;
        long final = (a * x * x) + (b * x) + c;
        AoCUtils.LogPart2(final);
    }

    public bool CheckValidInfinite(in Vector2<int> plot) => this.Data.garden[plot.X.Mod(this.Data.garden.Width),
                                                                             plot.Y.Mod(this.Data.garden.Height)];

    /// <inheritdoc />
    protected override (Grid<bool> garden, Vector2<int> start) Convert(string[] rawInput)
    {
        Vector2<int> start = Vector2<int>.Zero;
        foreach (int y in ..rawInput.Length)
        {
            int x = rawInput[y].IndexOf('S');
            if (x is not -1)
            {
                start = new Vector2<int>(x, y);
                break;
            }
        }

        Grid<bool> garden = new(rawInput[0].Length, rawInput.Length,
                                rawInput, line => line.Select(c => c is not '#').ToArray(),
                                b => b ? "." : "#");

        return (garden, start);
    }
}
