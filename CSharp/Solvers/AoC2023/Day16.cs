﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 16
/// </summary>
public class Day16 : GridSolver<Day16.Element>
{
    // ReSharper disable IdentifierTypo
    public enum Element
    {
        EMPTY = '.',
        LMIRROR = '\\',
        RMIRROR = '/',
        VSPLITTER = '|',
        HSPLITTER = '-'
    }
    // ReSharper restore IdentifierTypo

    public struct State(Vector2<int> position, Directions direction) : IEquatable<State>
    {
        public Vector2<int> position  = position;
        public Directions   direction = direction;

        #region Equality members
        /// <inheritdoc />
        public bool Equals(State other) => this.position == other.position
                                        && this.direction == other.direction;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is State other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(this.position, (int)this.direction);
        #endregion
    }

    private readonly Grid<bool> energized;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Element"/> fails</exception>
    public Day16(string input) : base(input)
    {
        this.energized = new(this.Data.Width, this.Data.Height, toString: e => e ? "#" : ".");
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int count = EnergizeGrid(Vector2<int>.Zero, Directions.RIGHT);
        AoCUtils.LogPart1(count);

        int max = this.Data.Width - 1;
        int maxCount = Math.Max(count, EnergizeGrid(new(max, 0), Directions.LEFT));
        foreach (int y in 1..this.Data.Height)
        {
            maxCount = Math.Max(maxCount, EnergizeGrid(new(0, y), Directions.RIGHT));
            maxCount = Math.Max(maxCount, EnergizeGrid(new(max, y), Directions.LEFT));
        }

        max = this.Data.Height - 1;
        foreach (int x in ..this.Data.Width)
        {
            maxCount = Math.Max(maxCount, EnergizeGrid(new(x, 0), Directions.DOWN));
            maxCount = Math.Max(maxCount, EnergizeGrid(new(x, max), Directions.UP));
        }

        AoCUtils.LogPart2(maxCount);
    }

    public int EnergizeGrid(Vector2<int> startPosition, Directions startDirection)
    {
        State state = new(startPosition, startDirection);
        PropagateBeam(state, [state]);
        int count = 0;
        foreach (Vector2<int> pos in Vector2<int>.Enumerate(this.energized.Width, this.energized.Height))
        {
            if (!this.energized[pos]) continue;

            count++;
            this.energized[pos] = false;
        }
        return count;
    }

    public void PropagateBeam(State state, HashSet<State> visited)
    {
        bool CanContinue(ref State s)
        {
            // Check if the move within the grid is valid, and that there has not been an identical state seen before
            return this.Data.TryMoveWithinGrid(s.position, s.direction, out s.position)
                && visited.Add(s);
        }

        do
        {
            this.energized[state.position] = true;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (this.Data[state.position])
            {
                // Reflected -> turns left
                case Element.RMIRROR when state.direction is Directions.LEFT or Directions.RIGHT:
                case Element.LMIRROR when state.direction is Directions.DOWN or Directions.UP:
                    state.direction = state.direction.TurnLeft();
                    break;

                // Reflected -> turns right
                case Element.RMIRROR when state.direction is Directions.DOWN or Directions.UP:
                case Element.LMIRROR when state.direction is Directions.LEFT or Directions.RIGHT:
                    state.direction = state.direction.TurnRight();
                    break;

                // Split -> turns left, spawns child turns right
                case Element.HSPLITTER when state.direction is Directions.DOWN or Directions.UP:
                case Element.VSPLITTER when state.direction is Directions.LEFT or Directions.RIGHT:
                    State splitState = new(state.position, state.direction.TurnRight());
                    state.direction = state.direction.TurnLeft();

                    if (CanContinue(ref splitState))
                    {
                        PropagateBeam(splitState, visited);
                    }
                    break;

                // Do nothing
                case Element.HSPLITTER when state.direction is Directions.LEFT or Directions.RIGHT:
                case Element.VSPLITTER when state.direction is Directions.DOWN or Directions.UP:
                case Element.EMPTY:
                        break;

                default:
                    throw new UnreachableException("Unknown movement configuration encountered");
            }
        }
        while (CanContinue(ref state));
    }

    /// <inheritdoc />
    protected override Element[] LineConverter(string line) => line.Select(c => (Element)c).ToArray();

    /// <inheritdoc />
    protected override string StringConversion(Element obj) => ((char)obj).ToString();
    #endregion
}