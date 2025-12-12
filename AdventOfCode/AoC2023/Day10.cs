using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 10
/// </summary>
public sealed class Day10 : GridSolver<Day10.Pipe>
{
    public enum Pipe
    {
        GROUND     = '.',
        VERTICAL   = '|',
        HORIZONTAL = '-',
        BEND_NE    = 'L',
        BEND_NW    = 'J',
        BEND_SW    = '7',
        BEND_SE    = 'F',
        START      = 'S'
    }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> start = this.Data.PositionOf(Pipe.START);
        List<(Vector2<int> pos, Direction dir)> heads = [];

        foreach (Direction dir in Direction.CardinalDirections)
        {
            Vector2<int> targetPos = start + dir;
            if (!this.Data.WithinGrid(targetPos)) continue;
            Pipe target = this.Data[targetPos];

            switch (dir)
            {
                case Direction.UP when target is Pipe.VERTICAL or Pipe.BEND_SW or Pipe.BEND_SE:
                    heads.Add((start, dir));
                    break;
                case Direction.DOWN when target is Pipe.VERTICAL or Pipe.BEND_NW or Pipe.BEND_NE:
                    heads.Add((start, dir));
                    break;
                case Direction.LEFT when target is Pipe.HORIZONTAL or Pipe.BEND_NE or Pipe.BEND_SE:
                    heads.Add((start, dir));
                    break;
                case Direction.RIGHT when target is Pipe.HORIZONTAL or Pipe.BEND_NW or Pipe.BEND_SW:
                    heads.Add((start, dir));
                    break;
            }
        }

        ReplaceStart(start, heads.Select(h => h.dir));
        int distance = 0;
        Grid<bool> path = new(this.Data.Width, this.Data.Height) { [start] = true };
        do
        {
            foreach (int i in ..heads.Count)
            {
                (Vector2<int> pos, Direction dir) = heads[i];
                pos += dir;
                dir = GetNewDirection(dir, this.Data[pos]);
                heads[i] = (pos, dir);
                path[pos] = true;
            }

            distance++;
        }
        while (heads[0].pos != heads[1].pos);

        AoCUtils.LogPart1(distance);

        int total = 0;
        foreach (int y in ..this.Data.Height)
        {
            bool isInLoop = false;
            foreach (int x in ..this.Data.Width)
            {
                Vector2<int> pos = new(x, y);
                if (path[pos])
                {
                    Pipe pipe = this.Data[pos];
                    if (pipe is Pipe.VERTICAL or Pipe.BEND_NE or Pipe.BEND_NW)
                    {
                        isInLoop = !isInLoop;
                    }
                }
                else if (isInLoop)
                {
                    total++;
                }
            }
        }

        AoCUtils.LogPart2(total);
    }

    public Direction GetNewDirection(Direction facing, Pipe junction)
    {
        if (junction is Pipe.HORIZONTAL or Pipe.VERTICAL) return facing;

        return facing switch
        {
            Direction.UP    => junction is Pipe.BEND_SE ? Direction.RIGHT : Direction.LEFT,
            Direction.DOWN  => junction is Pipe.BEND_NE ? Direction.RIGHT : Direction.LEFT,
            Direction.LEFT  => junction is Pipe.BEND_NE ? Direction.UP    : Direction.DOWN,
            Direction.RIGHT => junction is Pipe.BEND_NW ? Direction.UP    : Direction.DOWN,
            _                => throw new UnreachableException("Invalid direction detected")
        };
    }

    public void ReplaceStart(Vector2<int> start, IEnumerable<Direction> directions)
    {
        HashSet<Direction> dirs = new(directions);
        if (dirs.Contains(Direction.UP))
        {
            if (dirs.Contains(Direction.DOWN))
            {
                this.Data[start] = Pipe.VERTICAL;
            }
            else if (dirs.Contains(Direction.LEFT))
            {
                this.Data[start] = Pipe.BEND_NW;
            }
            else
            {
                this.Data[start] = Pipe.BEND_NE;
            }
        }
        else if (dirs.Contains(Direction.DOWN))
        {
            if (dirs.Contains(Direction.LEFT))
            {
                this.Data[start] = Pipe.BEND_SW;
            }
            else
            {
                this.Data[start] = Pipe.BEND_SE;
            }
        }
        else
        {
            this.Data[start] = Pipe.HORIZONTAL;
        }
    }

    /// <inheritdoc />
    protected override Pipe[] LineConverter(string line) => line.Select(c => (Pipe)c).ToArray();
}
