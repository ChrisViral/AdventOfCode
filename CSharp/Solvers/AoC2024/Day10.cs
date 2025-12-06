using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 10
/// </summary>
public sealed class Day10 : GridSolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector2<int>> trailheadCache = new(100);
        Vector2<int> scores = this.Grid.Dimensions.EnumerateOver()
                                  .Where(p => this.Grid[p] is 0)
                                  .Sum(p => CalculateTrailheadScore(p, trailheadCache));
        AoCUtils.LogPart1(scores.X);
        AoCUtils.LogPart2(scores.Y);
    }

    private Vector2<int> CalculateTrailheadScore(in Vector2<int> startPosition, HashSet<Vector2<int>> trailheads)
    {
        int FindTrailheads(in Vector2<int> currentPosition)
        {
            int totalScore = 0;
            int targetHeight = this.Grid[currentPosition] + 1;
            foreach (Vector2<int> move in currentPosition.Adjacent())
            {
                if (!this.Grid.WithinGrid(move)) continue;

                int moveHeight = this.Grid[move];
                if (moveHeight != targetHeight) continue;

                if (moveHeight is 9)
                {
                    totalScore++;
                    trailheads.Add(move);
                }
                else
                {
                    totalScore += FindTrailheads(move);
                }
            }
            return totalScore;
        }

        int distinctScore = FindTrailheads(startPosition);
        int normalScore = trailheads.Count;
        trailheads.Clear();
        return (normalScore, distinctScore);
    }

    /// <inheritdoc />
    protected override int[] LineConverter(string line) => line.AsSpan().Select(c => c - '0').ToArray();
}
