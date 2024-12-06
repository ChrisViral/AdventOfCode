﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 06
/// </summary>
public class Day06 : GridSolver<Day06.Element>
{
    /// <summary>
    /// Grid elements
    /// </summary>
    public enum Element
    {
        EMPTY = '.',
        WALL  = '#',
        GUARD = '^'
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Element"/> fails</exception>
    public Day06(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Get starting position
        Vector2<int> startPosition = this.Grid.PositionOf(Element.GUARD);
        this.Grid[startPosition] = Element.EMPTY;
        HashSet<Vector2<int>> visited = new(100) { startPosition };

        // Current state
        Direction direction = Direction.UP;
        Vector2<int> position = startPosition;

        // Iterate until we leave the grid
        while (this.Grid.TryMoveWithinGrid(position, direction, out Vector2<int> newPosition))
        {
            // If we hit a wall, cancel movement and turn right
            if (this.Grid[newPosition] is Element.WALL)
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
        visited.Remove(startPosition);
        // Create hashset for direction-bound visits
        int loops = 0;
        HashSet<(Direction, Vector2<int>)> visitedWithObstacle = new(100);
        // Try placing an obstacle on any visited position
        foreach (Vector2<int> visitedPosition in visited)
        {
            visitedWithObstacle.Add((Direction.UP, visitedPosition));
            if (CheckIfObstructionCausesLoop(startPosition, visitedPosition, visitedWithObstacle))
            {
                loops++;
            }
            visitedWithObstacle.Clear();
        }
        AoCUtils.LogPart2(loops);
    }

    private bool CheckIfObstructionCausesLoop(in Vector2<int> startPosition, in Vector2<int> obstaclePosition, HashSet<(Direction, Vector2<int>)> visited)
    {
        // Place obstacle
        this.Grid[obstaclePosition] = Element.WALL;

        // Start data
        Direction direction = Direction.UP;
        Vector2<int> position = startPosition;

        // Iterate until we leave the grid
        while (this.Grid.TryMoveWithinGrid(position, direction, out Vector2<int> newPosition))
        {
            // If we hit a wall, cancel movement and turn right
            if (this.Grid[newPosition] is Element.WALL)
            {
                direction = direction.TurnRight();
            }
            else
            {
                // Else we update the position
                position = newPosition;
            }

            // If we hit the same position/direction combo, *now* we've hit a loop
            // ReSharper disable once InvertIf
            if (!visited.Add((direction, position)))
            {
                this.Grid[obstaclePosition] = Element.EMPTY;
                return true;
            }
        }

        // If we left the grid, we haven't formed a loop
        this.Grid[obstaclePosition] = Element.EMPTY;
        return false;
    }

    /// <inheritdoc />
    protected override Element[] LineConverter(string line) => line.Select(c => (Element)c).ToArray();
    #endregion
}