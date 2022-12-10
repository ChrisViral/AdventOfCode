using System;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
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
        VISITED = '#',
        HEAD = 'H',
        A = '1',
        B = '2',
        C = '3',
        D = '4',
        E = '5',
        F = '6',
        G = '7',
        H = '8',
        I = '9'
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
        ConsoleView<Element> visited = new(SIZE, SIZE, v => (char)v, defaultValue: Element.EMPTY);
        SimulateRope(visited, 2);
        AoCUtils.LogPart1(visited.Count(v => v is Element.VISITED));

        visited = new(SIZE, SIZE, v => (char)v, defaultValue: Element.EMPTY);
        SimulateRope(visited, 10);
        AoCUtils.LogPart2(visited.Count(v => v is Element.VISITED));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Vector2<int> ConvertLine(string line)
    {
        Vector2<int> vector = Vector2<int>.ParseFromDirection(line);
        if (vector == Vector2<int>.Zero)
        {
            Console.WriteLine(line);
        }

        return vector;
    }

    private void SimulateRope(ConsoleView<Element> visited, int knotCount)
    {
        ConsoleView<Element> print = new(SIZE, SIZE, v => (char)v, defaultValue: Element.EMPTY);
        Vector2<int>[] knots = new Vector2<int>[knotCount];
        visited[knots[^1]] = Element.VISITED;
        foreach (Vector2<int> movement in this.Data)
        {
            int length = Math.Max(Math.Abs(movement.X), Math.Abs(movement.Y));
            Vector2<int> direction = movement / length;
            foreach (int _ in ..length)
            {
                knots[0] += direction;
                foreach (int i in ..(knots.Length - 1))
                {
                    ref Vector2<int> head = ref knots[i];
                    ref Vector2<int> tail = ref knots[i + 1];
                    (int x, int y) = head - tail;
                    if (Math.Abs(x) is 2)
                    {
                        if (Math.Abs(y) is 2)
                        {
                            tail += new Vector2<int>(x / 2, y / 2);
                        }
                        else
                        {
                            tail += new Vector2<int>(x / 2, y);
                        }
                    }
                    else if (Math.Abs(y) is 2)
                    {
                        tail += new Vector2<int>(x, y / 2);
                    }
                }

                visited[knots[^1]] = Element.VISITED;
            }
        }
    }
    #endregion
}

