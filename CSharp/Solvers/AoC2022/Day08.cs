using System;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 08
/// </summary>
public class Day08 : GridSolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver for 2022 - 08 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int visibleCount = 0;
        Grid<bool> visibilities = new(this.Data.Width, this.Data.Height, value => value ? "1" : "0");

        foreach (int y in ..this.Data.Height)
        {
            int maxHeight = -1 ;
            foreach (int x in ..this.Data.Width)
            {
                if (SetVisibility(visibilities, new Vector2<int>(x, y), ref maxHeight, ref visibleCount)) break;
            }

            maxHeight = -1;
            foreach (int x in ^this.Data.Width..)
            {
                if (SetVisibility(visibilities, new Vector2<int>(x, y), ref maxHeight, ref visibleCount)) break;
            }
        }

        foreach (int x in ..this.Data.Width)
        {
            int maxHeight = -1;
            foreach (int y in ..this.Data.Height)
            {
                if (SetVisibility(visibilities, new Vector2<int>(x, y), ref maxHeight, ref visibleCount)) break;
            }

            maxHeight = -1;
            foreach (int y in ^this.Data.Height..)
            {
                if (SetVisibility(visibilities, new Vector2<int>(x, y), ref maxHeight, ref visibleCount)) break;
            }
        }

        AoCUtils.LogPart1(visibleCount);

        int scenicScore = 0;
        foreach (Vector2<int> position in Vector2<int>.Enumerate(this.Data.Width - 1, this.Data.Height - 1)
                                                      .Where(p => p.X is not 0 && p.Y is not 0))
        {
            int currentScore = 1;
            int currentHeight = this.Data[position];
            visibleCount = 0;
            foreach (int y in (position.Y + 1)..this.Data.Height)
            {
                visibleCount++;
                if (this.Data[position.X, y] >= currentHeight) break;
            }

            currentScore *= visibleCount;
            visibleCount = 0;
            foreach (int y in (position.Y - 1)..)
            {
                visibleCount++;
                if (this.Data[position.X, y] >= currentHeight) break;
            }

            currentScore *= visibleCount;
            visibleCount = 0;
            foreach (int x in (position.X + 1)..this.Data.Width)
            {
                visibleCount++;
                if (this.Data[x, position.Y] >= currentHeight) break;
            }

            currentScore *= visibleCount;
            visibleCount = 0;
            foreach (int x in (position.X - 1)..)
            {
                visibleCount++;
                if (this.Data[x, position.Y] >= currentHeight) break;
            }

            currentScore *= visibleCount;
            scenicScore = Math.Max(scenicScore, currentScore);
        }

        // Part 2 answer
        AoCUtils.LogPart2(scenicScore);
    }

    private bool SetVisibility(Grid<bool> visibilities, Vector2<int> position, ref int maxHeight, ref int count)
    {
        int currentHeight = this.Data[position];
        if (maxHeight < currentHeight)
        {
            if (!visibilities[position])
            {
                visibilities[position] = true;
                count++;
            }

            maxHeight = currentHeight;
        }

        return maxHeight is 9;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] LineConverter(string line)
    {
        int[] row = new int[line.Length];
        foreach (int i in ..row.Length)
        {
            row[i] = line[i] - '0';
        }

        return row;
    }
}
