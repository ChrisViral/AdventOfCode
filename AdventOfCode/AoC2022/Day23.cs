using AdventOfCode.Collections;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 23
/// </summary>
public sealed class Day23 : Solver<Day23.Elf[]>
{
    /// <summary>
    /// Elf data class
    /// </summary>
    public sealed class Elf
    {
        /// <summary>Movement direction location checks</summary>
        private static readonly (int i, int j, int k, Vector2<int> move)[] DirectionChecks =
        [
            (0, 1, 2, Vector2<int>.Up),     // NW-N-NE
            (5, 6, 7, Vector2<int>.Down),   // SW-S-SE
            (0, 3, 5, Vector2<int>.Left),   // NW-W-SW
            (2, 4, 7, Vector2<int>.Right)   // NE-E-SE
        ];

        private int moveOrderIndex;
        private Vector2<int> nextPosition;

        /// <summary>
        /// Elf position
        /// </summary>
        public Vector2<int> Position { get; private set; }

        /// <summary>
        /// If this elf is moving or not
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Creates a new elf at the given position
        /// </summary>
        /// <param name="position">Position of the elf</param>
        public Elf(Vector2<int> position)
        {
            this.Position     = position;
            this.nextPosition = position;
        }

        /// <summary>
        /// Make the elf decide where he wants to move next
        /// </summary>
        /// <param name="elves">Set of the position of other elves</param>
        /// <param name="plannedMoves">Planned move location counter</param>
        public void ChooseNextLocation(HashSet<Vector2<int>> elves, Counter<Vector2<int>> plannedMoves)
        {
            // Order is NW-N-NE-W-E-SW-S-SE
            bool[] adjacent = this.Position
                                  .AsAdjacentEnumerable(withDiagonals: true)
                                  .Select(elves.Contains)
                                  .ToArray();
            // Check if anything is adjacent at all
            if (adjacent.Exists(b => b))
            {
                // Check all four directions
                foreach (int offset in ..DirectionChecks.Length)
                {
                    // Get the current index from the offset
                    int moveIndex = (this.moveOrderIndex + offset) % DirectionChecks.Length;
                    (int i, int j, int k, Vector2<int> move) = DirectionChecks[moveIndex];
                    // Test the direction
                    if (!adjacent[i] && !adjacent[j] && !adjacent[k])
                    {
                        // Plan move and exit
                        this.nextPosition = this.Position + move;
                        plannedMoves.Add(this.nextPosition);
                        this.IsMoving = true;
                        return;
                    }
                }
            }

            // No movement, plan staying here
            plannedMoves.Add(this.Position);
            this.IsMoving = false;
        }

        /// <summary>
        /// Completes the elf movement, if possible
        /// </summary>
        /// <param name="plannedMoves">Planned elf movement counter</param>
        public void CompleteMove(Counter<Vector2<int>> plannedMoves)
        {
            // No moving needed if not moving at all
            if (this.IsMoving)
            {
                if (plannedMoves[this.nextPosition] is 1)
                {
                    // If moving, check that only one person plans moving there
                    this.Position = this.nextPosition;
                }
                else
                {
                    // Otherwise abort
                    this.nextPosition = this.Position;
                    this.IsMoving = false;
                }
            }

            // Update the movement check start index
            this.moveOrderIndex = (this.moveOrderIndex + 1) % DirectionChecks.Length;
        }
    }

    /// <summary>Number of rounds in the first part</summary>
    private const int ROUNDS = 10;

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver for 2022 - 23 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create the basic data
        HashSet<Vector2<int>> elves = new(this.Data.Select(e => e.Position));
        Counter<Vector2<int>> plannedMoves = new();
        // Simulate movement for
        (..ROUNDS).AsEnumerable().ForEach(_ => SimulateRound(elves, plannedMoves));
        // Get all four bounds
        int top    = this.Data.Min(e => e.Position.Y);
        int bottom = this.Data.Max(e => e.Position.Y);
        int left   = this.Data.Min(e => e.Position.X);
        int right  = this.Data.Max(e => e.Position.X);
        // Calculate bounding box
        Vector2<int> topLeft     = new(left, top);
        Vector2<int> bottomRight = new(right, bottom);
        Vector2<int> size        = bottomRight - topLeft + Vector2<int>.One;
        AoCUtils.LogPart1((size.X * size.Y) - this.Data.Length);

        // Execute more rounds until no moving elf exists
        int rounds;
        for (rounds = ROUNDS; this.Data.Exists(e => e.IsMoving); rounds++)
        {
            SimulateRound(elves, plannedMoves);
        }

        AoCUtils.LogPart2(rounds);
    }

    /// <summary>
    /// Simulate a round of elf movement
    /// </summary>
    /// <param name="elves">Set of the positions of the elves</param>
    /// <param name="plannedMoves">Elf planned movement counter</param>
    private void SimulateRound(HashSet<Vector2<int>> elves, Counter<Vector2<int>> plannedMoves)
    {
        // Choose the next location, then complete the move
        this.Data.ForEach(e => e.ChooseNextLocation(elves, plannedMoves));
        this.Data.ForEach(e => e.CompleteMove(plannedMoves));
        // Update the elves positions, and clear the planned moves
        elves.Clear();
        elves.AddRange(this.Data.Select(e => e.Position));
        plannedMoves.Clear();
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Elf[] Convert(string[] lines) => Vector2<int>.MakeEnumerable(lines[0].Length, lines.Length)
                                                                    .Where(p => lines[p.Y][p.X] is '#')
                                                                    .Select(p => new Elf(p))
                                                                    .ToArray();
}
