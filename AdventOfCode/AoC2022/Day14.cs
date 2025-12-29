using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 14
/// </summary>
public sealed class Day14 : ArraySolver<Vector2<int>[]>
{
    /// <summary>
    /// Cave element enum
    /// </summary>
    private enum CaveElement
    {
        EMPTY  = ' ',
        WALL   = '█',
        SAND   = '░',
        SOURCE = '+'
    }

    /// <summary> Sand source universal position </summary>
    private static readonly Vector2<int> SourcePosition = new(500, 0);
    /// <summary> Double right shift </summary>
    private static readonly Vector2<int> RightShift     = new(2, 0);

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver for 2022 - 14 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Inefficient, but simple
        Vector2<int>[] allPoints = this.Data.SelectMany(l => l).ToArray();
        Vector2<int> topLeft     = new(allPoints.Min(v => v.X) - 201, 0);
        Vector2<int> bottomRight = new(allPoints.Max(v => v.X) + 200, allPoints.Max(v => v.Y) + 2);
        // Create grid
        Vector2<int> size        = (bottomRight - topLeft) + Vector2<int>.One;
        Vector2<int> source      = SourcePosition - topLeft;
        Grid<CaveElement> cave   = new(size.X, size.Y, e => ((char)e).ToString());
        cave.Fill(CaveElement.EMPTY);
        cave[source] = CaveElement.SOURCE;

        // Set up walls
        foreach (Span<Vector2<int>> line in this.Data)
        {
            Vector2<int> current = line[0];
            cave[current - topLeft] = CaveElement.WALL;
            foreach (Vector2<int> next in line[1..])
            {
                Vector2<int> diff = next - current;
                int length = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
                Vector2<int> direction = diff / length;
                do
                {
                    current += direction;
                    cave[current - topLeft] = CaveElement.WALL;
                }
                while (current != next);
            }
        }

        // Initial fill
        int count = 0;
        Vector2<int> position = source;
        while (FillSand(source, cave, ref count, ref position)) { }

        AoCUtils.Log(cave);
        AoCUtils.LogPart1(count);

        // Add bottom wall
        foreach (int x in ..size.X)
        {
            cave[x, bottomRight.Y] = CaveElement.WALL;
        }

        // Second fill
        position = source;
        while (FillSand(source, cave, ref count, ref position)) { }

        AoCUtils.Log(cave);
        AoCUtils.LogPart2(count);
    }

    /// <inheritdoc />
    protected override Vector2<int>[] ConvertLine(string line) => line.Split("->", DEFAULT_OPTIONS).Select(s => Vector2<int>.Parse(s)).ToArray();

    private static bool FillSand(Vector2<int> source, Grid<CaveElement> cave, ref int count, ref Vector2<int> position)
    {
        Vector2<int>? newPosition = cave.MoveWithinGrid(position, Direction.DOWN);
        if (newPosition is null) return false;

        position = newPosition.Value;
        if (cave[position] is CaveElement.EMPTY) return true;

        position += Vector2<int>.Left;
        if (cave[position] is CaveElement.EMPTY) return true;

        position += RightShift;
        if (cave[position] is CaveElement.EMPTY) return true;

        count++;
        position -= Vector2<int>.One;
        cave[position] = CaveElement.SAND;
        if (position == source) return false;

        position = source;
        return true;
    }
}
