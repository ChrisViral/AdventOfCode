using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 20
/// </summary>
public sealed class Day20 : Solver<(Grid<bool> racetrack, Vector2<int> start, Vector2<int> end)>
{
    private static readonly SearchValues<char> Markers = SearchValues.Create('S', 'E');

    private const int MIN_SAVE = 100;
    private const int PART1_DISTANCE = 2;
    private const int PART2_DISTANCE = 20;

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get full path
        Vector2<int>[] tempPath = SearchUtils.Search(this.Data.start, this.Data.end, null, Neighbours, MinSearchComparer<int>.Comparer, out _)!;
        Span<Vector2<int>> path = stackalloc Vector2<int>[tempPath.Length + 1];
        path[0] = this.Data.start;
        tempPath.AsSpan().CopyTo(path[1..]);

        // Setup path position => index map
        Dictionary<Vector2<int>, int> indicesTemp = new(path.Length);
        foreach (int i in ..path.Length)
        {
            indicesTemp[path[i]] = i;
        }
        FrozenDictionary<Vector2<int>, int> indices = indicesTemp.ToFrozenDictionary();

        // Calculate valid cheats
        ReadOnlySpan<Vector2<int>> searchPath = path[..^3];
        int validCheats = searchPath.Sum(p => GetValidCheats(p, PART1_DISTANCE, indices));
        AoCUtils.LogPart1(validCheats);

        validCheats = searchPath.Sum(p => GetValidCheats(p, PART2_DISTANCE, indices));
        AoCUtils.LogPart2(validCheats);
    }

    private IEnumerable<MoveData<Vector2<int>, int>> Neighbours(Vector2<int> node)
    {
        foreach (Vector2<int> adjacent in node.AsAdjacentEnumerable())
        {
            if (this.Data.racetrack.TryGetPosition(adjacent, out bool isWall) && !isWall)
            {
                yield return new MoveData<Vector2<int>, int>(adjacent, 1);
            }
        }
    }

    // ReSharper disable once CognitiveComplexity
    private int GetValidCheats(Vector2<int> startPosition, int time, FrozenDictionary<Vector2<int>, int> indices)
    {
        int validCheats = 0;
        int startIndex = indices[startPosition];
        for (int y = -time; y <= time; y++)
        {
            // Make sure we're within the racetrack
            int yPos = y + startPosition.Y;
            if (yPos < 0 || yPos >= this.Data.racetrack.Height) continue;

            int yDist = Math.Abs(y);
            int maxX = time - yDist;
            for (int x = -maxX; x <= maxX; x++)
            {
                // Make sure we're within the racetrack
                int xPos = x + startPosition.X;
                if (xPos < 0 || xPos >= this.Data.racetrack.Width) continue;

                // Make sure we've gone far enough for it to be an actual shortcut
                int distance = Math.Abs(x) + yDist;
                if (distance <= 1) continue;

                // Check if we're within the grid in an empty spot
                Vector2<int> endPosition = startPosition + (x, y);
                if (this.Data.racetrack[endPosition]) continue;

                // Make sure we end up somewhere better
                int endIndex = indices[endPosition];
                if (endIndex < startIndex) continue;

                // Check cheat time save
                int cheatTimeSave = endIndex - startIndex - distance;
                if (cheatTimeSave >= MIN_SAVE)
                {
                    validCheats++;
                }
            }
        }

        return validCheats;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Grid<bool>, Vector2<int>, Vector2<int>) Convert(string[] rawInput)
    {
        Grid<bool> racetrack = new(rawInput[0].Length, rawInput.Length, rawInput,
                                   l => l.AsSpan().Select(c => c is '#').ToArray(),
                                   b => b ? "#" : ".");

        Vector2<int> start = Vector2<int>.Zero;
        Vector2<int> end = Vector2<int>.Zero;
        foreach (int y in ..rawInput.Length)
        {
            string line = rawInput[y];
            int x = line.AsSpan().IndexOfAny(Markers);
            if (x is not -1)
            {
                switch (line[x])
                {
                    case 'S':
                        start = (x, y);
                        break;

                    case 'E':
                        end = (x, y);
                        break;

                    default:
                        throw new UnreachableException("Invalid character");
                }
            }
        }

        return (racetrack, start, end);
    }
}
