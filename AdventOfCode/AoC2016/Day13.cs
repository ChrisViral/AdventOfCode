using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 13
/// </summary>
public sealed class Day13 : Solver<int>
{
    private const int STEPS = 50;

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Grid<bool> maze = new(STEPS, STEPS, b => b ? "." : "#");
        foreach (Vector2<int> position in maze.Dimensions.Enumerate())
        {
            int value = position.X * position.X;
            value += position.X * 3;
            value += position.X * position.Y * 2;
            value += position.Y;
            value += position.Y * position.Y;
            value += this.Data;
            value = int.PopCount(value);
            maze[position] = value.IsEven;
        }

        Vector2<int> start = Vector2<int>.One;
        Vector2<int> end   = (31, 39);
        int pathLength = SearchUtils.GetPathLengthBFS(start, end, p => Neighbours(p, maze))!.Value;
        AoCUtils.LogPart1(pathLength);

        HashSet<Vector2<int>> visited = new(100);
        Queue<Vector2<int>> visiting   = new(100);
        Queue<Vector2<int>> visitNext = new(100);
        visiting.Enqueue(start);
        foreach (int _ in ..STEPS)
        {
            while (visiting.TryDequeue(out Vector2<int> position))
            {
                foreach (Vector2<int> neighbour in Neighbours(position, maze))
                {
                    if (visited.Add(neighbour))
                    {
                        visitNext.Enqueue(neighbour);
                    }
                }
            }

            AoCUtils.Swap(ref visiting, ref visitNext);
        }
        AoCUtils.LogPart2(visited.Count);
    }

    private static IEnumerable<Vector2<int>> Neighbours(Vector2<int> node, Grid<bool> maze)
    {
        return node.AsAdjacentEnumerable()
                   .Where(p => maze.TryGetPosition(p, out bool isEmpty) && isEmpty);
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
