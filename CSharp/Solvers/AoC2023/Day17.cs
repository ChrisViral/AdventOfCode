using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 17
/// </summary>
public class Day17 : GridSolver<int>
{
    public interface ICrucible<T> : IEquatable<T> where T : ICrucible<T>
    {
        int Loss { get; }

        static abstract IEnumerable<MoveData<T, double>> Neighbours(T current);

        static abstract bool IsGoal(T current, T goal);

        static abstract double Heuristic(T current);
    }

    public readonly struct CruciblePath(Grid<int> grid, Vector2<int> position, Direction direction, int currentSteps) : ICrucible<CruciblePath>
    {
        public const int MAX_STEPS = 3;

        public readonly Grid<int> grid        = grid;
        public readonly Vector2<int> position = position;
        public readonly Direction direction  = direction;
        public readonly int currentSteps      = currentSteps;

        public int Loss { get; } = grid[position];

        public CruciblePath(Grid<int> grid) : this(grid, Vector2<int>.Zero, Direction.RIGHT, 0) { }

        public CruciblePath(Grid<int> grid, Vector2<int> position) : this(grid, position, Direction.RIGHT, 0) { }

        public override string ToString() => $"{position} - {this.direction}";

        public static IEnumerable<MoveData<CruciblePath, double>> Neighbours(CruciblePath current)
        {
            if (current.currentSteps < MAX_STEPS
             && TryGenerateSearchNode(current, current.direction, current.currentSteps + 1, out MoveData<CruciblePath, double>? node))
            {
                yield return node!.Value;
            }
            if (TryGenerateSearchNode(current, current.direction.TurnRight(), 1, out node))
            {
                yield return node!.Value;
            }
            if (TryGenerateSearchNode(current, current.direction.TurnLeft(), 1, out node))
            {
                yield return node!.Value;
            }
        }

        private static bool TryGenerateSearchNode(in CruciblePath current, Direction newDirection, int newSteps, out MoveData<CruciblePath, double>? node)
        {
            Vector2<int> newPosition = current.position + newDirection;
            if (!current.grid.WithinGrid(newPosition))
            {
                node = null;
                return false;
            }

            CruciblePath newPath = new(current.grid, newPosition, newDirection, newSteps);
            node = new MoveData<CruciblePath, double>(newPath, newPath.Loss);
            return true;
        }

        public static bool IsGoal(CruciblePath current, CruciblePath goal) => current.position == goal.position;

        public static double Heuristic(CruciblePath current) => Vector2<int>.Distance(current.position,
                                                                                      new Vector2<int>(current.grid.Width - 1, current.grid.Height - 1));

        /// <inheritdoc />
        public bool Equals(CruciblePath other) => this.position     == other.position
                                               && this.direction    == other.direction
                                               && this.currentSteps == other.currentSteps;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CruciblePath other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(this.position, (int)this.direction, this.currentSteps);
        }

    public readonly struct UltraCruciblePath(Grid<int> grid, Vector2<int> position, Direction direction, int currentSteps, int loss) : ICrucible<UltraCruciblePath>
    {
        public const int MIN_STEPS = 4;
        public const int MAX_STEPS = 10;

        public readonly Grid<int> grid        = grid;
        public readonly Vector2<int> position = position;
        public readonly Direction direction  = direction;
        public readonly int currentSteps      = currentSteps;

        public int Loss { get; } = loss;

        public UltraCruciblePath(Grid<int> grid) : this(grid, Vector2<int>.Zero, Direction.RIGHT, 0, 0) { }

        public UltraCruciblePath(Grid<int> grid, Vector2<int> position) : this(grid, position, Direction.RIGHT, 0, 0) { }

        public override string ToString() => $"{position} - {this.direction}";

        public static IEnumerable<MoveData<UltraCruciblePath, double>> Neighbours(UltraCruciblePath current)
        {
            if (current.currentSteps < MAX_STEPS
             && TryGenerateSearchNode(current, current.direction, 1, current.currentSteps + 1, out MoveData<UltraCruciblePath, double>? node))
            {
                yield return node!.Value;
            }
            if (TryGenerateSearchNode(current, current.direction.TurnRight(), MIN_STEPS, MIN_STEPS, out node))
            {
                yield return node!.Value;
            }
            if (TryGenerateSearchNode(current, current.direction.TurnLeft(), MIN_STEPS, MIN_STEPS, out node))
            {
                yield return node!.Value;
            }
        }

        private static bool TryGenerateSearchNode(in UltraCruciblePath current, Direction newDirection, int travelDistance, int newSteps, out MoveData<UltraCruciblePath, double>? node)
        {
            Vector2<int> newPosition = current.position + newDirection.ToVector(travelDistance);
            if (!current.grid.WithinGrid(newPosition))
            {
                node = null;
                return false;
            }

            int incurredLoss = current.grid[newPosition];
            if (travelDistance is not 1)
            {
                for (Vector2<int> p = current.position + newDirection; p != newPosition; p += newDirection)
                {
                    incurredLoss += current.grid[p];
                }
            }

            UltraCruciblePath newPath = new(current.grid, newPosition, newDirection, newSteps, incurredLoss);
            node = new MoveData<UltraCruciblePath, double>(newPath, incurredLoss);
            return true;
        }

        public static bool IsGoal(UltraCruciblePath current, UltraCruciblePath goal) => current.position == goal.position;

        public static double Heuristic(UltraCruciblePath current) => Vector2<int>.Distance(current.position,
                                                                                           new Vector2<int>(current.grid.Width - 1, current.grid.Height - 1));

        /// <inheritdoc />
        public bool Equals(UltraCruciblePath other) => this.position     == other.position
                                                    && this.direction    == other.direction
                                                    && this.currentSteps == other.currentSteps;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is UltraCruciblePath other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(this.position, (int)this.direction, this.currentSteps);
        }

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> endPosition = new(this.Data.Width - 1, this.Data.Height - 1);
        int heatLoss = GetMinLoss<CruciblePath>(new CruciblePath(this.Data), new CruciblePath(this.Data, endPosition));
        AoCUtils.LogPart1(heatLoss);

        heatLoss = GetMinLoss<UltraCruciblePath>(new UltraCruciblePath(this.Data), new UltraCruciblePath(this.Data, endPosition));
        AoCUtils.LogPart2(heatLoss);
    }

    public int GetMinLoss<T>(T start, T goal) where T : ICrucible<T>
    {
        T[]? path = SearchUtils.Search(start, goal,
                                       T.Heuristic,
                                       T.Neighbours,
                                       MinSearchComparer<double>.Comparer,
                                       T.IsGoal);

        return path!.Sum(p => p.Loss);
    }

    /// <inheritdoc />
    protected override int[] LineConverter(string line) => line.Select(c => c - '0').ToArray();
}
