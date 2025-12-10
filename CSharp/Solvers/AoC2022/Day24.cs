using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 24
/// </summary>
public sealed class Day24 : Solver<(Vector2<int> start, Vector2<int> end, Day24.Blizzard[] blizzards)>
{
    /// <summary>
    /// Blizzard object
    /// </summary>
    public sealed class Blizzard
    {
        /// <summary>Blizzard move direction</summary>
        private readonly Vector2<int> direction;

        /// <summary>
        /// Blizzard's position
        /// </summary>
        private Vector2<int> Position { get; set; }

        /// <summary>
        /// Creates a new Blizzard at a given position, moving in the given direction
        /// </summary>
        /// <param name="position">Blizzard position</param>
        /// <param name="direction">Blizzard direction</param>
        public Blizzard(Vector2<int> position, char direction)
        {
            this.Position  = position;
            this.direction = (direction switch
            {
                '<' => Direction.LEFT,
                '>' => Direction.RIGHT,
                '^' => Direction.UP,
                'v' => Direction.DOWN,
                _   => throw new UnreachableException("Unknown direction")
            }).ToVector<int>();
        }

        /// <summary>
        /// Updates the position of this blizzard, while staying within the bounds given
        /// </summary>
        /// <param name="limit">Maximum bounds to stay within</param>
        /// <returns>The new position of the Blizzard</returns>
        public Vector2<int> UpdatePosition(Vector2<int> limit)
        {
            // Update the position while wrapping to the limits
            this.Position  = new Vector2<int>((this.Position.X + this.direction.X).Mod(limit.X),
                                              (this.Position.Y + this.direction.Y).Mod(limit.Y));
            return this.Position;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver for 2022 - 24 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create limits and find path time
        Vector2<int> limit = (this.Data.end.X + 1, this.Data.end.Y);
        int time = FindPathTime(this.Data.start, this.Data.end, limit);
        AoCUtils.LogPart1(time);

        // Go back to the start, and then return
        time += FindPathTime(this.Data.end, this.Data.start, limit);
        time += FindPathTime(this.Data.start, this.Data.end, limit);
        AoCUtils.LogPart2(time);
    }

    /// <summary>
    /// Finds the path time to go from the start to the end
    /// </summary>
    /// <param name="start">Starting point</param>
    /// <param name="end">Ending point</param>
    /// <param name="limit">Map limits</param>
    /// <returns>The total time taken to reach the end from the start</returns>
    /// ReSharper disable once CognitiveComplexity
    private int FindPathTime(Vector2<int> start, Vector2<int> end, Vector2<int> limit)
    {
        // Create necessary structures
        HashSet<Vector2<int>> blizzardPositions = [], nextSearch = [];
        Stack<Vector2<int>> search = new();
        bool pathFound = false;
        int time = 0;
        search.Push(start);
        do
        {
            // Setup blizzard positions
            blizzardPositions.Clear();
            blizzardPositions.AddRange(this.Data.blizzards.Select(b => b.UpdatePosition(limit)));

            // Loop through all current search nodes
            while (!pathFound && search.TryPop(out Vector2<int> position))
            {
                // Look through possible moves
                foreach (Vector2<int> move in position.Adjacent(includeSelf: true).Where(m => m == start
                                                                                           || m == end
                                                                                           || ((..limit.X).IsInRange(m.X)
                                                                                            && (..limit.Y).IsInRange(m.Y))))
                {
                    // If at end, mark that we have reached it
                    if (move == end)
                    {
                        pathFound = true;
                        break;
                    }

                    // Only add to search if no blizzard is there
                    if (!blizzardPositions.Contains(move))
                    {
                        nextSearch.Add(move);
                    }
                }
            }

            // Update the search stack
            nextSearch.ForEach(search.Push);
            nextSearch.Clear();
            time++;
        }
        while (!pathFound);

        return time;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector2<int>, Vector2<int>, Blizzard[]) Convert(string[] lines)
    {
        // get start and end positions
        Vector2<int> start = new(0, -1);
        Vector2<int> end   = new(lines[^1].IndexOf('.') - 1, lines.Length - 2);
        // Create blizzards
        Blizzard[] blizzards = Vector2<int>.MakeEnumerable(lines[0].Length, lines.Length)
                                           .Select(p => (pos: p - Vector2<int>.One, dir: lines[p.Y][p.X]))
                                           .Where(b  => b.dir is '<' or '>' or '^' or 'v')
                                           .Select(b => new Blizzard(b.pos, b.dir))
                                           .ToArray();
        return (start, end, blizzards);
    }
}
