using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 17
/// </summary>
public class Day17 : Solver<Direction[]>
{
    /// <summary>
    /// Rock struct
    /// </summary>
    /// <param name="Anchor">Rock anchor point</param>
    /// <param name="Points">Rock chunks points</param>
    /// <param name="Bounds">Maximum rock bounds</param>
    private record struct Rock(Vector2<int> Anchor, Vector2<int>[] Points, Vector2<int> Bounds)
    {
        /// <summary>Rock overlap buffer</summary>
        private static readonly HashSet<Vector2<int>> checkBuffer = new(5);

        /// <summary>
        /// Rock top limit
        /// </summary>
        public int TopPoint => this.Anchor.Y;

        /// <summary>
        /// Rock bottom limit
        /// </summary>
        public int BottomPoint => this.Anchor.Y + this.Bounds.Y;

        /// <summary>
        /// Rock left limit
        /// </summary>
        public int LeftPoint => this.Anchor.X;

        /// <summary>
        /// Rock right limit
        /// </summary>
        public int RightPoint => this.Anchor.X + this.Bounds.X;

        /// <summary>
        /// Pushes a rock in a direction, if possible
        /// </summary>
        /// <param name="direction">Direction to push the rock in</param>
        /// <param name="rocks">Other rocks in the pit</param>
        public void Push(Direction direction, List<Rock> rocks)
        {
            // Check walls first
            if ((direction is not Direction.LEFT || this.LeftPoint <= 0)
             && (direction is not Direction.RIGHT || this.RightPoint >= 6)) return;

            // Make sure we're not intersecting with another rock
            Vector2<int> original = this.Anchor;
            this.Anchor += direction;
            if (rocks.Exists(IntersectsWith))
            {
                this.Anchor = original;
            }
        }

        /// <summary>
        /// Try and move the rock downwards
        /// </summary>
        /// <param name="rocks">Other rocks in the pit</param>
        /// <returns><see langword="true"/> if the rock successfully moved, otherwise <see langword="false"/></returns>
        public bool MoveDown(List<Rock> rocks)
        {
            // Make sure we're not hitting the floor
            if (this.BottomPoint <= 0) return false;

            // Check for intersections with other rocks
            Vector2<int> original = this.Anchor;
            this.Anchor += Vector2<int>.Up;     // This is actually (0, -1), which is what we want
            if (!rocks.Exists(IntersectsWith)) return true;

            this.Anchor = original;
            return false;

        }

        /// <summary>
        /// Intersection test with other rock
        /// </summary>
        /// <param name="other">Rock to test intersection for</param>
        /// <returns><see langword="true"/> if the rocks intersect, otherwise <see langword="false"/></returns>
        private bool IntersectsWith(Rock other)
        {
            // Check if there is a box overlap
            if (this.LeftPoint > other.RightPoint || this.RightPoint < other.LeftPoint
             || this.TopPoint < other.BottomPoint || this.BottomPoint > other.TopPoint) return false;

            // Validate all points
            checkBuffer.AddRange(GetOffsetPoints());
            bool overlaps = checkBuffer.Overlaps(other.GetOffsetPoints());
            checkBuffer.Clear();
            return overlaps;
        }

        /// <summary>
        /// Get the chucks of this rock in world space
        /// </summary>
        /// <returns>Enumerable of all the world space locations for this rock</returns>
        private IEnumerable<Vector2<int>> GetOffsetPoints()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Vector2<int> point in this.Points)
            {
                yield return this.Anchor + point;
            }
        }
    }

    /// <summary>Rocks limit for the first part</summary>
    private const int  FIRST_LIMIT  = 3000;     // Larger to ensure we include a cycle
    /// <summary>Rocks limit for the first part</summary>
    private const long SECOND_LIMIT = 1000000000000L;
    /// <summary>Horizontal bar rock shape</summary>
    private static readonly Vector2<int>[] horizontal =
    [
        new(0, 0),
        new(1, 0),
        new(2, 0),
        new(3, 0)
    ];
    /// <summary>Cross rock shape</summary>
    private static readonly Vector2<int>[] cross =
    [
        new(0, -1),
        new(1, 0),
        new(1, -1),
        new(1, -2),
        new(2, -1)
    ];
    /// <summary>Corner rock shape</summary>
    private static readonly Vector2<int>[] corner =
    [
        new(0, -2),
        new(1, -2),
        new(2, 0),
        new(2, -1),
        new(2, -2)
    ];
    /// <summary>Vertical bar rock shape</summary>
    private static readonly Vector2<int>[] vertical =
    [
        new(0, 0),
        new(0, -1),
        new(0, -2),
        new(0, -3)
    ];
    /// <summary>Cube rock shape</summary>
    private static readonly Vector2<int>[] cube =
    [
        new(0, 0),
        new(0, -1),
        new(1, 0),
        new(1, -1)
    ];
    /// <summary>Rock shapes array</summary>
    private static readonly (Vector2<int>[] shape, Vector2<int> bounds)[] shapes =
    [
        (horizontal, new Vector2<int>(3, 0)),
        (cross, new Vector2<int>(2, -2)),
        (corner, new Vector2<int>(2, -2)),
        (vertical, new Vector2<int>(0, -3)),
        (cube, new Vector2<int>(1, -1))
    ];

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver for 2022 - 17 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        int shapeIndex = 0;
        int jetsIndex = 0;
        int height = 0;
        int heightAt2022 = 0;
        List<(int shape, int jet, int gain)> states = new(FIRST_LIMIT);
        List<Rock> rocks = new(FIRST_LIMIT);
        foreach (int i in ..FIRST_LIMIT)
        {
            // Save answer height
            if (i is 2022) heightAt2022 = height;

            // Create new rock
            (int shape, int jet, int gain) state = (shapeIndex, jetsIndex, 0);
            (Vector2<int>[] points, Vector2<int> bounds) = shapes[shapeIndex++];
            Rock rock = new(new Vector2<int>(2, height - bounds.Y + 3), points, bounds);
            shapeIndex %= shapes.Length;

            do
            {
                // Move according to jets as long as can move down
                Direction direction = this.Data[jetsIndex++];
                rock.Push(direction, rocks);
                jetsIndex %= this.Data.Length;
            }
            while (rock.MoveDown(rocks));

            // Save rock data and current stack state
            rocks.Add(rock);
            int previous = height;
            height = Math.Max(height, rock.TopPoint + 1);
            state.gain = height - previous;
            states.Add(state);
        }

        AoCUtils.LogPart1(heightAt2022);

        // Find a cycle of at least length 50
        int cycleStart = 0;
        int cycleEnd   = 0;
        foreach (int i in ..states.Count)
        {
            // Find the next index that matches
            int next = i + 1;
            int other = states.FindIndex(next, states.Count - next, s => states[i] == s);
            if (other is not -1 && (..50).AsEnumerable()
                                         .All(j => states[i + j] == states[other + j]))
            {
                cycleStart = i;
                cycleEnd   = other;
                break;
            }
        }

        // Calculate the size of the tower from the cycle
        (int shape, int jet, int gain)[] statesArray = states.ToArray();
        int cycleLength     = cycleEnd - cycleStart;
        int heightAtStart   = statesArray[..cycleStart].Sum(s => s.gain);
        int cycleHeight     = statesArray[cycleStart..cycleEnd].Sum(s => s.gain);
        long cycles         = Math.DivRem(SECOND_LIMIT - cycleStart, cycleLength, out long remainder);
        long cyclesHeight   = cycles * cycleHeight;
        int remainderHeight = statesArray[cycleStart..(cycleStart + (int)remainder)].Sum(s => s.gain);
        long totalHeight    = heightAtStart + cyclesHeight + remainderHeight;
        AoCUtils.LogPart2(totalHeight);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Direction[] Convert(string[] lines) => lines[0].Select(c => c is '<' ? Direction.LEFT : Direction.RIGHT)
                                                                       .ToArray();
}
