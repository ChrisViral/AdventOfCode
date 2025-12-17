using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 17
/// </summary>
public sealed partial class Day17 : Solver<(Grid<Day17.Element> map, Vector2<int> offset)>
{
    public enum Element
    {
        NONE       = 0,
        EMPTY      = '.',
        WATER_FLOW = '░',
        WATER_FILL = '▒',
        CLAY       = '▓'
    }

    private class UniqueQueue<T> where T : IEquatable<T>
    {
        private readonly HashSet<T> inQueue = new(100);
        private readonly Queue<T> queue = new(100);

        public void Enqueue(T element)
        {
            if (this.inQueue.Add(element))
            {
                this.queue.Enqueue(element);
            }
        }

        public bool TryDequeue([NotNullWhen(true)] out T? element)
        {
            if (this.queue.TryDequeue(out element))
            {
                this.inQueue.Remove(element);
                return true;
            }

            return false;
        }
    }

    private const int SPRING_X = 500;

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    [GeneratedRegex(@"([xy])=(\d+), [xy]=(\d+)\.\.(\d+)")]
    private static partial Regex LineMatcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> start = new(SPRING_X - this.Data.offset.X, 0);
        UniqueQueue<Vector2<int>> flowQueue = new();
        flowQueue.Enqueue(start);
        while (flowQueue.TryDequeue(out Vector2<int> current))
        {
            this.Data.map[current] = Element.WATER_FLOW;
            if (!this.Data.map.TryMoveWithinGrid(current, Direction.DOWN, out Vector2<int> moved)) continue;

            Element movedTo = this.Data.map[moved];
            switch (movedTo)
            {
                case Element.WATER_FLOW:
                    continue;

                case Element.EMPTY:
                    flowQueue.Enqueue(moved);
                    continue;

                case Element.NONE:
                case Element.WATER_FILL:
                case Element.CLAY:
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(movedTo), (int)movedTo, typeof(Element));
            }

            bool hasFlow = false;
            if (FlowInDirection(current, Direction.LEFT, out Vector2<int> flowEndLeft))
            {
                flowQueue.Enqueue(flowEndLeft);
                hasFlow = true;
            }
            if (FlowInDirection(current, Direction.RIGHT, out Vector2<int> flowEndRight))
            {
                flowQueue.Enqueue(flowEndRight);
                hasFlow = true;
            }

            if (hasFlow)
            {
                FillInDirection(current, Direction.LEFT, flowEndLeft, Element.WATER_FLOW);
                FillInDirection(current, Direction.RIGHT, flowEndRight, Element.WATER_FLOW);
            }
            else
            {
                flowQueue.Enqueue(current + Vector2<int>.Up);
                FillInDirection(current, Direction.LEFT, flowEndLeft, Element.WATER_FILL);
                FillInDirection(current, Direction.RIGHT, flowEndRight, Element.WATER_FILL);
            }
        }

        int water = 0;
        int filled = 0;
        foreach (Element element in this.Data.map)
        {
            switch (element)
            {
                case Element.WATER_FLOW:
                    water++;
                    break;

                case Element.WATER_FILL:
                    water++;
                    filled++;
                    break;

                case Element.NONE:
                case Element.EMPTY:
                case Element.CLAY:
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(element), (int)element, typeof(Element));
            }
        }
        AoCUtils.LogPart1(water);
        AoCUtils.LogPart2(filled);
    }

    private bool FlowInDirection(Vector2<int> flowStart, Direction direction, out Vector2<int> flowEnd)
    {
        Vector2<int> position = flowStart;
        do
        {
            position += direction;
            if (this.Data.map[position] is Element.CLAY or Element.WATER_FILL)
            {
                flowEnd = position - direction;
                return false;
            }
        }
        while (!this.Data.map.TryMoveWithinGrid(position, Direction.DOWN, out Vector2<int> moved)
            || this.Data.map[moved] is Element.CLAY or Element.WATER_FILL) ;

        flowEnd = position;
        return true;
    }

    private void FillInDirection(Vector2<int> start, Direction direction, Vector2<int> end, Element waterType)
    {
        Vector2<int> position = start;
        this.Data.map[start] = waterType;
        while (position != end)
        {
            position += direction;
            this.Data.map[position] = waterType;
        }
    }

    /// <inheritdoc />
    protected override (Grid<Element>, Vector2<int>) Convert(string[] rawInput)
    {
        HashSet<Vector2<int>> clay = new(1000);
        foreach (string line in rawInput)
        {
            Match lineMatch = LineMatcher.Match(line);
            int value = int.Parse(lineMatch.Groups[2].ValueSpan);
            int from  = int.Parse(lineMatch.Groups[3].ValueSpan);
            int to    = int.Parse(lineMatch.Groups[4].ValueSpan);

            if (lineMatch.Groups[1].ValueSpan[0] is 'x')
            {
                foreach (int y in from..^to)
                {
                    clay.Add(new Vector2<int>(value, y));
                }
            }
            else
            {
                foreach (int x in from..^to)
                {
                    clay.Add(new Vector2<int>(x, value));
                }
            }
        }

        Vector2<int> min = clay.Aggregate(Vector2<int>.Min) + Vector2<int>.Left;
        Vector2<int> max = clay.Aggregate(Vector2<int>.Max) + Vector2<int>.Right;
        Vector2<int> size = max - min + Vector2<int>.One;
        Grid<Element> map = new(size.X, size.Y, e => new string((char)e, 1));
        map.Fill(Element.EMPTY);
        clay.ForEach(c => map[c - min] = Element.CLAY);
        return (map, min);
    }
}
