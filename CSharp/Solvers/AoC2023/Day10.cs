using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 10
/// </summary>
public class Day10 : GridSolver<Day10.Pipe>
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

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Pipe"/> fails</exception>
    public Day10(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Vector2<int> start = this.Data.PositionOf(Pipe.START);
        List<(Vector2<int> pos, Directions dir)> heads = new();

        foreach (Directions dir in DirectionsUtils.AllDirections)
        {
            Vector2<int> targetPos = start + dir;
            if (!this.Data.WithinGrid(targetPos)) continue;
            Pipe target = this.Data[targetPos];

            switch (dir)
            {
                case Directions.UP when target is Pipe.VERTICAL or Pipe.BEND_SW or Pipe.BEND_SE:
                    heads.Add((start, dir));
                    break;
                case Directions.DOWN when target is Pipe.VERTICAL or Pipe.BEND_NW or Pipe.BEND_NE:
                    heads.Add((start, dir));
                    break;
                case Directions.LEFT when target is Pipe.HORIZONTAL or Pipe.BEND_NE or Pipe.BEND_SE:
                    heads.Add((start, dir));
                    break;
                case Directions.RIGHT when target is Pipe.HORIZONTAL or Pipe.BEND_NW or Pipe.BEND_SW:
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
                (Vector2<int> pos, Directions dir) = heads[i];
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

    public Directions GetNewDirection(Directions facing, Pipe junction)
    {
        if (junction is Pipe.HORIZONTAL or Pipe.VERTICAL) return facing;

        return facing switch
        {
            Directions.UP    => junction is Pipe.BEND_SE ? Directions.RIGHT : Directions.LEFT,
            Directions.DOWN  => junction is Pipe.BEND_NE ? Directions.RIGHT : Directions.LEFT,
            Directions.LEFT  => junction is Pipe.BEND_NE ? Directions.UP    : Directions.DOWN,
            Directions.RIGHT => junction is Pipe.BEND_NW ? Directions.UP    : Directions.DOWN,
            _                => throw new UnreachableException("Invalid direction detected")
        };
    }

    public void ReplaceStart(in Vector2<int> start, IEnumerable<Directions> directions)
    {
        HashSet<Directions> dirs = new(directions);
        if (dirs.Contains(Directions.UP))
        {
            if (dirs.Contains(Directions.DOWN))
            {
                this.Data[start] = Pipe.VERTICAL;
            }
            else if (dirs.Contains(Directions.LEFT))
            {
                this.Data[start] = Pipe.BEND_NW;
            }
            else
            {
                this.Data[start] = Pipe.BEND_NE;
            }
        }
        else if (dirs.Contains(Directions.DOWN))
        {
            if (dirs.Contains(Directions.LEFT))
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
    #endregion
}