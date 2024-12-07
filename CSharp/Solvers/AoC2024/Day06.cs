using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 06
/// </summary>
public class Day06 : Solver<(Grid<bool> grid, Vector2<int> startPosition)>
{
    /// <summary>
    /// Simulation thread data container
    /// </summary>
    /// <param name="StartLocation">Guard start location</param>
    /// <param name="Grid">Environment grid</param>
    /// <param name="Visited">Visited locations/direction pairs</param>
    private record SimulationData(Vector2<int> StartLocation, Grid<bool> Grid, HashSet<(Direction, Vector2<int>)> Visited)
    {
        /// <summary>
        /// How many looping configurations have been found
        /// </summary>
        public int LoopsCount { get; set; }
    }

    /// <summary>
    /// Wall character
    /// </summary>
    private const char WALL  = '#';
    /// <summary>
    /// Guard start character
    /// </summary>
    private const char START = '^';

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to (<see cref="Grid{T}"/> <see cref="Vector2{T}"/>) fails</exception>
    public Day06(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Setup visited set
        HashSet<Vector2<int>> visited = new(100) { this.Data.startPosition };

        // Initial state
        Direction direction   = Direction.UP;
        Vector2<int> position = this.Data.startPosition;

        // Iterate until we leave the grid
        while (this.Data.grid.TryMoveWithinGrid(position, direction, out Vector2<int> newPosition))
        {
            // If we hit a wall, cancel movement and turn right
            if (this.Data.grid[newPosition])
            {
                direction = direction.TurnRight();
                continue;
            }

            // Update the position and add to visited set
            position = newPosition;
            visited.Add(position);
        }
        AoCUtils.LogPart1(visited.Count);

        // We can't place an obstacle on the start position
        visited.Remove(this.Data.startPosition);

        // Parallelize checking each visited location
        int totalLoops = 0;
        Parallel.ForEach(visited,
                         () => new SimulationData(this.Data.startPosition,
                                                  new Grid<bool>(this.Data.grid),
                                                  new HashSet<(Direction, Vector2<int>)>(100)),
                         CheckIfObstacleCausesLoop,
                         data => Interlocked.Add(ref totalLoops, data.LoopsCount));
        AoCUtils.LogPart2(totalLoops);
    }

    /// <summary>
    /// Simulates placing a new obstacle and checks if it causes the guard to loop
    /// </summary>
    /// <param name="obstaclePosition">Where a new obstacle should be placed</param>
    /// <param name="state">Parallel loop state</param>
    /// <param name="data">Simulation thread data</param>
    /// <returns>The updated simulation thread data</returns>
    private static SimulationData CheckIfObstacleCausesLoop(Vector2<int> obstaclePosition, ParallelLoopState state, SimulationData data)
    {
        // Setup
        Direction direction   = Direction.UP;
        Vector2<int> position = data.StartLocation;
        data.Visited.Add((direction, position));
        data.Grid[obstaclePosition] = true;

        // Iterate until we leave the grid
        while (data.Grid.TryMoveWithinGrid(position, direction, out Vector2<int> newPosition))
        {
            if (data.Grid[newPosition])
            {
                // If we hit a wall, cancel movement and turn right
                direction = direction.TurnRight();
            }
            else
            {
                // Else we update the position
                position = newPosition;
            }

            // If we hit the same position/direction combo, *now* we've hit a loop
            // ReSharper disable once InvertIf
            if (!data.Visited.Add((direction, position)))
            {
                // Increment the hits and break out
                data.LoopsCount++;
                break;
            }
        }

        // Cleanup
        data.Grid[obstaclePosition] = false;
        data.Visited.Clear();
        return data;
    }

    /// <inheritdoc />
    protected override (Grid<bool> grid, Vector2<int> startPosition) Convert(string[] rawInput)
    {
        // Parse grid
        int width  = rawInput[0].Length;
        int height = rawInput.Length;
        Grid<bool> grid = new(width, height, rawInput, line => line.AsSpan().Select(c => c is WALL).ToArray(), e => e ? "#" : ".");

        // Get start position
        foreach (int y in ..height)
        {
            int x = rawInput[y].IndexOf(START);
            if (x is not -1)
            {
                return (grid, (x, y));
            }
        }

        // Invalid
        throw new InvalidOperationException("Starting position not found");
    }
    #endregion
}
