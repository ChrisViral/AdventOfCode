using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 16
/// </summary>
public sealed class Day16 : GridSolver<Day16.Element>
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

    private struct State(Vector2<int> position, Direction direction) : IEquatable<State>
    {
        public Vector2<int> position  = position;
        public Direction   direction = direction;

        /// <inheritdoc />
        public bool Equals(State other) => this.position == other.position
                                        && this.direction == other.direction;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is State other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(this.position, (int)this.direction);
        }

    private readonly Grid<bool> energized;
    private readonly HashSet<State> visited = [];

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input)
    {
        this.energized = new Grid<bool>(this.Data.Width, this.Data.Height, toString: e => e ? "#" : ".");
    }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int count = EnergizeGrid(Vector2<int>.Zero, Direction.RIGHT);
        AoCUtils.LogPart1(count);

        int max = this.Data.Width - 1;
        int maxCount = Math.Max(count, EnergizeGrid(new Vector2<int>(max, 0), Direction.LEFT));
        foreach (int y in 1..this.Data.Height)
        {
            maxCount = Math.Max(maxCount, EnergizeGrid(new Vector2<int>(0, y), Direction.RIGHT));
            maxCount = Math.Max(maxCount, EnergizeGrid(new Vector2<int>(max, y), Direction.LEFT));
        }

        max = this.Data.Height - 1;
        foreach (int x in ..this.Data.Width)
        {
            maxCount = Math.Max(maxCount, EnergizeGrid(new Vector2<int>(x, 0), Direction.DOWN));
            maxCount = Math.Max(maxCount, EnergizeGrid(new Vector2<int>(x, max), Direction.UP));
        }

        AoCUtils.LogPart2(maxCount);
    }

    public int EnergizeGrid(Vector2<int> startPosition, Direction startDirection)
    {
        State state = new(startPosition, startDirection);
        this.visited.Add(state);
        PropagateBeam(state);
        this.visited.Clear();
        int count = 0;
        foreach (Vector2<int> pos in Vector2<int>.EnumerateOver(this.energized.Width, this.energized.Height))
        {
            if (!this.energized[pos]) continue;

            count++;
            this.energized[pos] = false;
        }
        return count;
    }

    // ReSharper disable once CognitiveComplexity
    private void PropagateBeam(State state)
    {
        bool CanContinue(ref State s)
        {
            // Check if the move within the grid is valid, and that there has not been an identical state seen before
            return this.Data.TryMoveWithinGrid(s.position, s.direction, out s.position)
                && this.visited.Add(s);
        }

        do
        {
            this.energized[state.position] = true;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (this.Data[state.position])
            {
                // Reflected -> turns left
                case Element.RMIRROR when state.direction is Direction.LEFT or Direction.RIGHT:
                case Element.LMIRROR when state.direction is Direction.DOWN or Direction.UP:
                    state.direction = state.direction.TurnLeft();
                    break;

                // Reflected -> turns right
                case Element.RMIRROR when state.direction is Direction.DOWN or Direction.UP:
                case Element.LMIRROR when state.direction is Direction.LEFT or Direction.RIGHT:
                    state.direction = state.direction.TurnRight();
                    break;

                // Split -> turns left, spawns child turns right
                case Element.HSPLITTER when state.direction is Direction.DOWN or Direction.UP:
                case Element.VSPLITTER when state.direction is Direction.LEFT or Direction.RIGHT:
                    State splitState = new(state.position, state.direction.TurnRight());
                    state.direction = state.direction.TurnLeft();

                    if (CanContinue(ref splitState))
                    {
                        PropagateBeam(splitState);
                    }
                    break;

                // Do nothing
                case Element.HSPLITTER when state.direction is Direction.LEFT or Direction.RIGHT:
                case Element.VSPLITTER when state.direction is Direction.DOWN or Direction.UP:
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
}
