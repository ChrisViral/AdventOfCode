using System.ComponentModel;
using System.Diagnostics;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2025;

/// <summary>
/// Solver for 2025 Day 07
/// </summary>
public sealed class Day07 : GridSolver<Day07.ManifoldElement>
{
    public enum ManifoldElement
    {
        NONE     = 0,
        EMPTY    = '.',
        SPLITTER = '^',
        START    = 'S'
    }

    [DebuggerDisplay("Position: {Position}")]
    private sealed class Splitter(Vector2<int> position)
    {
        private Splitter? left;
        private Splitter? right;

        public Vector2<int> Position { get; } = position;

        private long? timelinesCache;
        public long Timelines => timelinesCache ??= (this.left?.Timelines ?? 1L) + (this.right?.Timelines ?? 1L);

        public void AddDownstream(Splitter splitter, Direction direction)
        {
            switch (direction)
            {
                case Direction.LEFT:
                    this.left = splitter;
                    break;

                case Direction.RIGHT:
                    this.right = splitter;
                    break;

                case Direction.UP:
                case Direction.DOWN:
                case Direction.NONE:
                    throw new InvalidOperationException("Invalid splitter direction detected");

                default:
                    throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction));
            }
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        List<Vector2<int>> visited = new(16);
        Dictionary<Vector2<int>, Splitter?> knownBeams  = new(100);
        Queue<(Splitter? source, Direction splitDirection)> splitQueue = new(50);

        Vector2<int> start = this.Grid.PositionOf(ManifoldElement.START);
        splitQueue.Enqueue((null, Direction.NONE));

        int splitters = 0;
        while (splitQueue.TryDequeue(out (Splitter?, Direction) current))
        {
            Splitter? downstream;
            (Splitter? upstream, Direction direction) = current;
            Vector2<int> position = upstream is not null ? upstream.Position + direction : start;
            do
            {
                // Check if we've been here before and know what's downstream
                if (knownBeams.TryGetValue(position, out downstream)) break;

                // If we're not at a splitter, continue
                visited.Add(position);
                if (this.Grid[position] is not ManifoldElement.SPLITTER) continue;

                // Create new splitter and enqueue its split pathes
                splitters++;
                downstream = new Splitter(position);
                splitQueue.Enqueue((downstream, Direction.LEFT));
                splitQueue.Enqueue((downstream, Direction.RIGHT));
                break;
            }
            while (this.Grid.TryMoveWithinGrid(position, Direction.DOWN, out position));

            // Link splitters
            if (downstream is not null && upstream is not null)
            {
                upstream.AddDownstream(downstream, direction);
            }

            // Mark beam pathes
            visited.ForEach(v => knownBeams.Add(v, downstream));
            visited.Clear();
        }
        AoCUtils.LogPart1(splitters);
        AoCUtils.LogPart2(knownBeams[start]!.Timelines);
    }

    /// <inheritdoc />
    protected override ManifoldElement[] LineConverter(string line) => line.Select(c => (ManifoldElement)c).ToArray();
}
