using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Enums;
using ZLinq;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 15
/// </summary>
public sealed class Day15 : IntcodeSolver
{
    /// <summary>
    /// Element values
    /// </summary>
    private enum Element
    {
        WALL   = 0,
        EMPTY  = 1,
        OXYGEN = 2,
        START,
        ROBOT,
        PATH,
        UNKNOWN
    }

    /// <summary>
    /// Start position
    /// </summary>
    private static readonly Vector2<int> Start = (1, 1);

    /// <summary>
    /// Oxygen system position
    /// </summary>
    private Vector2<int> oxygenPosition;
    /// <summary>
    /// Environment map
    /// </summary>
    private readonly ConsoleView<Element> map = new(41, 41, ShowElement, defaultValue: Element.UNKNOWN, fps: 60);

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Fully explore map
        this.map[Start] = Element.START;
        HashSet<Vector2<int>> explored = new(100);
        Explore(Start, explored);

        // Get oxygen position
        this.oxygenPosition = this.map.PositionOf(Element.OXYGEN);

        // Get path length to oxygen
        Vector2<int>[] path = SearchUtils.Search(Start, this.oxygenPosition, Heuristic, Neighbours, MinSearchComparer<int>.Comparer, out _)!;

        // Move along path to oxygen system
        this.map[Start] = Element.START;
        foreach (Vector2<int> move in path)
        {
            this.map[move] = Element.ROBOT;
            this.map.PrintToConsole();
            this.map[move] = Element.PATH;
        }
        this.map[this.oxygenPosition] = Element.OXYGEN;
        AoCUtils.LogPart1(path.Length);

        //Adjust cursor
        Console.SetCursorPosition(0, Console.CursorTop - 3);

        // Fill map
        HashSet<Vector2<int>> spreadLocations = new(this.map.Size);
        Queue<Vector2<int>> spread     = new(this.map.Size);
        Queue<Vector2<int>> nextSpread = new(this.map.Size);
        spread.Enqueue(this.oxygenPosition);
        int spreadTime = -1;
        do
        {
            // Apply current spread
            while (spread.TryDequeue(out Vector2<int> from))
            {
                this.map[from] = Element.OXYGEN;
                foreach (Vector2<int> to in from.Adjacent())
                {
                    // Enqueue next spreads
                    if (this.map[to] is not Element.WALL && spreadLocations.Add(to))
                    {
                        nextSpread.Enqueue(to);
                    }
                }
            }

            // Swap queues and print
            spreadTime++;
            (spread, nextSpread) = (nextSpread, spread);
            this.map.PrintToConsole();
        }
        while (!spread.IsEmpty);

        //Adjust back down
        Console.SetCursorPosition(0, Console.CursorTop + 3);
        AoCUtils.LogPart2(spreadTime);
    }

    private IEnumerable<MoveData<Vector2<int>, int>> Neighbours(Vector2<int> node) => node.AsAdjacentEnumerable()
                                                                                          .Where(p => this.map[p] is not Element.WALL)
                                                                                          .Select(p => new MoveData<Vector2<int>, int>(p, 1));

    private int Heuristic(Vector2<int> value) => Vector2<int>.ManhattanDistance(value, this.oxygenPosition);

    /// <summary>
    /// Explores the entire map exhaustively from the current position
    /// </summary>
    /// <param name="position">Current robot position</param>
    /// <param name="explored">Set of explored positions</param>
    private void Explore(Vector2<int> position, HashSet<Vector2<int>> explored)
    {
        // Keep track of original element and print
        Element originalElement = this.map[position];
        this.map[position] = Element.ROBOT;
        this.map.PrintToConsole();

        // Go through all possible directions
        foreach (Direction direction in Direction.CardinalDirections)
        {
            // Check if the location has already been explored
            Vector2<int> movePosition = position + direction;
            if (!explored.Add(movePosition)) continue;

            // If not, give command to move into direction
            Element moveElement = GiveCommand(direction);
            this.map[movePosition] = moveElement;

            // If we didnt hit a wall, explore from that location
            if (moveElement is not Element.WALL)
            {
                // Reset current position to original element and explore further down
                this.map[position] = originalElement;
                Explore(movePosition, explored);

                // Return to current position by sending inversed command
                GiveCommand(direction.Invert());
                this.map[position] = Element.ROBOT;
                this.map.PrintToConsole();
            }
        }

        // Make sure to reset map before leaving
        this.map[position] = originalElement;
    }

    /// <summary>
    /// Gives a move command to the robot and returns the robots feedback
    /// </summary>
    /// <param name="direction">Direction to move into</param>
    /// <returns>Map element reported by the robot</returns>
    private Element GiveCommand(Direction direction)
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (direction)
        {
            case Direction.NORTH:
                this.VM.Input.AddValue(1L);
                break;
            case Direction.SOUTH:
                this.VM.Input.AddValue(2L);
                break;
            case Direction.WEST:
                this.VM.Input.AddValue(3L);
                break;
            case Direction.EAST:
                this.VM.Input.AddValue(4L);
                break;
        }

        this.VM.Run();
        return (Element)this.VM.Output.GetValue();
    }

    /// <summary>
    /// Prints a given map element
    /// </summary>
    /// <param name="element">Element to print</param>
    /// <returns>The character representation of the element</returns>
    /// <exception cref="InvalidEnumArgumentException">Form invalid values of <paramref name="element"/></exception>
    private static char ShowElement(Element element) => element switch
    {
        Element.WALL    => '▓',
        Element.EMPTY   => ' ',
        Element.OXYGEN  => 'O',
        Element.START   => 'X',
        Element.ROBOT   => '¤',
        Element.PATH    => '·',
        Element.UNKNOWN => '░',
        _               => throw element.Invalid()
    };
}
