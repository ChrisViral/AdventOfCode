using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 14
/// </summary>
public sealed class Day14 : GridSolver<Day14.Rock>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum Rock
    {
        EMPTY = '.',
        ROUND = 'O',
        CUBE  = '#'
    }

    private const int CYCLES = 1_000_000_000;

    private readonly Dictionary<Direction, Vector2<int>[]> directionOrders = new(4);
    private readonly Dictionary<string, int> states = new();

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day14(string input) : base(input)
    {
        this.directionOrders[Direction.NORTH] = Vector2<int>.MakeEnumerable(this.Data.Width, this.Data.Height).ToArray();
        this.directionOrders[Direction.SOUTH] = this.directionOrders[Direction.UP].AsEnumerable().Reverse().ToArray();
        this.directionOrders[Direction.WEST]  = Vector2<int>.MakeEnumerable(this.Data.Height, this.Data.Width)
                                                            .Select(p => new Vector2<int>(p.Y, p.X)).ToArray();
        this.directionOrders[Direction.EAST]  = this.directionOrders[Direction.WEST].AsEnumerable().Reverse().ToArray();
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        SlideReflector(Direction.NORTH);
        int load = CalculateLoad(this.Data);
        AoCUtils.LogPart1(load);

        SlideReflector(Direction.WEST);
        SlideReflector(Direction.SOUTH);
        SlideReflector(Direction.EAST);

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

    // ReSharper disable once CognitiveComplexity
    public void SlideReflector(Direction direction)
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
        SlideReflector(Direction.NORTH);
        SlideReflector(Direction.WEST);
        SlideReflector(Direction.SOUTH);
        SlideReflector(Direction.EAST);
    }

    public int CalculateLoad(Grid<Rock> grid) => this.directionOrders[Direction.NORTH].Where(pos => grid[pos] is Rock.ROUND)
                                                     .Sum(pos => grid.Height - pos.Y);

    /// <inheritdoc />
    protected override Rock[] LineConverter(string line) => line.Select(c => (Rock)c).ToArray();

    protected override string StringConversion(Rock rock) => ((char)rock).ToString();
}
