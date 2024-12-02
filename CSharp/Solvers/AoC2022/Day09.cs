using System;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 09
/// </summary>
public class Day09 : ArraySolver<Vector2<int>>
{
    private enum Element
    {
        EMPTY   = '.',
        VISITED = '#'
    }

    private const int SIZE = 500;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver for 2022 - 09 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        AoCUtils.LogPart1(SimulateRope(2));
        AoCUtils.LogPart2(SimulateRope(10));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Vector2<int> ConvertLine(string line) => Vector2<int>.ParseFromDirection(line);

    private int SimulateRope(int knotCount)
    {
        ConsoleView<Element> visited = new(SIZE, SIZE, v => (char)v, defaultValue: Element.EMPTY);
        Vector2<int>[] knots = new Vector2<int>[knotCount];
        visited[knots[^1]] = Element.VISITED;
        foreach (Vector2<int> movement in this.Data)
        {
            int length = Math.Max(Math.Abs(movement.X), Math.Abs(movement.Y));
            Vector2<int> direction = movement / length;
            foreach (int _ in ..length)
            {
                knots[0] += direction;
                ref Vector2<int> head = ref knots[0];
                foreach (int i in 1..knots.Length)
                {
                    ref Vector2<int> tail = ref knots[i];
                    tail += (head - tail) switch
                    {
                        var diff when Math.Abs(diff.X) is 2
                                   && Math.Abs(diff.Y) is 2 => diff / 2,
                        var diff when Math.Abs(diff.X) is 2 => new(diff.X / 2, diff.Y),
                        var diff when Math.Abs(diff.Y) is 2 => new(diff.X, diff.Y / 2),
                        _                                   => Vector2<int>.Zero
                    };

                    head = ref tail;
                }

                visited[knots[^1]] = Element.VISITED;
            }
        }

        return visited.Count(v => v is Element.VISITED);
    }
    #endregion
}
