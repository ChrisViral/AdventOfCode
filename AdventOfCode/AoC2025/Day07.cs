using System.Diagnostics;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Enums;
using ZLinq;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 7
/// </summary>
[Solver(2025, 7)]
public sealed partial class Day07 : GridSolver<Day07.ManifoldElement>
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
        public long Timelines => this.timelinesCache ??= (this.left?.Timelines ?? 1L) + (this.right?.Timelines ?? 1L);

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
                    direction.ThrowInvalid();
                    return;
            }
        }
    }

    /// <inheritdoc />
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
        LogAnswer(splitters);
        LogAnswer(knownBeams[start]!.Timelines);
    }

    /// <inheritdoc />
    protected override ManifoldElement[] LineConverter(string line) => line.Select(c => (ManifoldElement)c).ToArray();
}
