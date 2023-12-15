using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 14
/// </summary>
public class Day14 : GridSolver<Day14.Rock>
{
    public enum Rock
    {
        EMPTY = '.',
        ROUND = 'O',
        CUBE  = '#'
    }

    private const int CYCLES = 1_000_000_000;

    private readonly Dictionary<Directions, Vector2<int>[]> directionOrders = new(4);
    private readonly Dictionary<string, int> states = new();

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day14(string input) : base(input)
    {
        this.directionOrders[Directions.NORTH] = Vector2<int>.Enumerate(this.Data.Width, this.Data.Height).ToArray();
        this.directionOrders[Directions.SOUTH] = this.directionOrders[Directions.UP].Reverse().ToArray();
        this.directionOrders[Directions.WEST]  = Vector2<int>.Enumerate(this.Data.Height, this.Data.Width)
                                                             .Select(p => new Vector2<int>(p.Y, p.X)).ToArray();
        this.directionOrders[Directions.EAST]  = this.directionOrders[Directions.WEST].Reverse().ToArray();
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        SlideReflector(Directions.NORTH);
        int load = CalculateLoad(this.Data);
        AoCUtils.LogPart1(load);

        SlideReflector(Directions.WEST);
        SlideReflector(Directions.SOUTH);
        SlideReflector(Directions.EAST);

        string state = this.Data.ToString();
        this.states.Add(state, 1);

        int current, cycleStart = 0;
        for (current = 2; current <= CYCLES; current++)
        {
            CycleReflector();
            state = this.Data.ToString();
            if (this.states.TryGetValue(state, out cycleStart)) break;

            this.states.Add(state, current);
        }

        int cycleLength = current - cycleStart;
        int offset      = (CYCLES - cycleStart) % cycleLength;
        int end         = cycleStart + offset;

        string[] endState = this.states.First(p => p.Value == end).Key.Split('\n', DEFAULT_OPTIONS);
        Grid<Rock> finalGrid = new(this.Data.Width, this.Data.Height, endState, LineConverter);
        load = CalculateLoad(finalGrid);
        AoCUtils.LogPart2(load);
    }

    public void SlideReflector(Directions direction)
    {
        Vector2<int>[] points = this.directionOrders[direction];
        foreach (Vector2<int> start in points)
        {
            Rock rock = this.Data[start];
            if (rock is not Rock.ROUND) continue;

            Vector2<int> end = start;
            while (this.Data.TryMoveWithinGrid(end, direction, out Vector2<int> slide))
            {
                if (this.Data[slide] is not Rock.EMPTY) break;

                end = slide;
            }

            if (start == end) continue;

            this.Data[end]   = Rock.ROUND;
            this.Data[start] = Rock.EMPTY;
        }
    }

    public void CycleReflector()
    {
        SlideReflector(Directions.NORTH);
        SlideReflector(Directions.WEST);
        SlideReflector(Directions.SOUTH);
        SlideReflector(Directions.EAST);
    }

    public int CalculateLoad(Grid<Rock> grid) => this.directionOrders[Directions.NORTH].Where(pos => grid[pos] is Rock.ROUND)
                                                                                       .Sum(pos   => grid.Height - pos.Y);

    /// <inheritdoc />
    protected override Rock[] LineConverter(string line) => line.Select(c => (Rock)c).ToArray();

    protected override string StringConversion(Rock rock) => ((char)rock).ToString();
    #endregion
}