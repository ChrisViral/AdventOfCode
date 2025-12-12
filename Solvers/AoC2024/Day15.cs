using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 15
/// </summary>
public sealed class Day15 : Solver<(Grid<Day15.Element> warehouse, Direction[] moves)>
{
    /// <summary>
    /// Warehouse element
    /// </summary>
    public enum Element : ushort
    {
        EMPTY     = '.',
        WALL      = '#',
        BOX       = 'O',
        ROBOT     = '@',
        BOX_LEFT  = '[',
        BOX_RIGHT = ']'
    }

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get the robots position
        Vector2<int> startPosition = this.Data.warehouse.PositionOf(Element.ROBOT);
        this.Data.warehouse[startPosition] = Element.EMPTY;

        // Initialize small warehouse
        Vector2<int> position   = startPosition;
        Grid<Element> warehouse = new(this.Data.warehouse);

        // Iterate through moves
        foreach (Direction move in this.Data.moves)
        {
            Vector2<int> newPosition = position + move;
            switch (warehouse[newPosition])
            {
                case Element.EMPTY:
                case Element.BOX when TryMoveBox(newPosition, warehouse, move):
                    position = newPosition;
                    break;

                case Element.WALL:
                case Element.BOX:
                    break;

                case Element.BOX_LEFT:
                case Element.BOX_RIGHT:
                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
        }

        // Calculate final coordinates
        int coordinates = warehouse.Dimensions.AsEnumerable()
                                   .Where(p => warehouse[p] is Element.BOX)
                                   .Sum(p => (100 * p.Y) + p.X);
        AoCUtils.LogPart1(coordinates);

        // Double warehouse size horizontally
        ConsoleView<Element> bigWarehouse = new(this.Data.warehouse.Width * 2, this.Data.warehouse.Height, e => (char)e, Anchor.TOP_LEFT, Element.EMPTY, 60);
        foreach (Vector2<int> pos in this.Data.warehouse.Dimensions.Enumerate())
        {
            Element currentElement    = this.Data.warehouse[pos];
            Vector2<int> doubledPosA  = pos with { X = pos.X * 2 };
            Vector2<int> doubledPosB  = doubledPosA + Vector2<int>.Right;
            if (currentElement is Element.BOX)
            {
                bigWarehouse[doubledPosA] = Element.BOX_LEFT;
                bigWarehouse[doubledPosB] = Element.BOX_RIGHT;
            }
            else
            {
                bigWarehouse[doubledPosA] = currentElement;
                bigWarehouse[doubledPosB] = currentElement;
            }
        }

        // Setup start position and iterate through moves
        position = startPosition with { X = startPosition.X * 2 };
        //int turn = 0;
        //bigWarehouse[position] = Element.ROBOT;
        //bigWarehouse.PrintToConsole($"Move {turn++}");
        //bigWarehouse[position] = Element.EMPTY;
        foreach (Direction move in this.Data.moves)
        {
            Vector2<int> newPosition = position + move;
            switch (bigWarehouse[newPosition])
            {
                case Element.EMPTY:
                case Element.BOX_LEFT  when TryMoveBigBox(newPosition, bigWarehouse, move):
                case Element.BOX_RIGHT when TryMoveBigBox(newPosition, bigWarehouse, move):
                    position = newPosition;
                    break;

                case Element.BOX_LEFT:
                case Element.BOX_RIGHT:
                case Element.WALL:
                    break;

                case Element.BOX:
                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
            //bigWarehouse[position] = Element.ROBOT;
            //bigWarehouse.PrintToConsole($"Move {turn++}");
            //bigWarehouse[position] = Element.EMPTY;
        }
        // Calculate final coordinates
        coordinates = bigWarehouse.Dimensions.AsEnumerable()
                                  .Where(p => bigWarehouse[p] is Element.BOX_LEFT)
                                  .Sum(p => (100 * p.Y) + p.X);
        AoCUtils.LogPart2(coordinates);
    }

    private static bool TryMoveBox(Vector2<int> boxStart, Grid<Element> warehouse, Direction direction)
    {
        // Check along movement until we hit an empty space or a wall
        for (Vector2<int> pushPos = boxStart + direction; ; pushPos += direction)
        {
            switch (warehouse[pushPos])
            {
                case Element.EMPTY:
                    warehouse[pushPos]  = Element.BOX;
                    warehouse[boxStart] = Element.EMPTY;
                    return true;

                case Element.WALL:
                    return false;

                case Element.BOX:
                    break;

                case Element.BOX_LEFT:
                case Element.BOX_RIGHT:
                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
        }
    }

    private static bool TryMoveBigBox(Vector2<int> boxStart, ConsoleView<Element> warehouse, Direction direction)
    {
        if (direction.IsHorizontal())
        {
            // Horizontal movement
            return TryMoveBoxHorizontal(boxStart, warehouse, direction);
        }

        // Vertical movement, check first
        if (!CheckBoxPushableVertical(boxStart, warehouse, direction)) return false;

        // Then push if valid
        PushBoxVertically(boxStart, warehouse, direction);
        return true;
    }

    private static bool TryMoveBoxHorizontal(Vector2<int> boxStart, ConsoleView<Element> warehouse, Direction direction)
    {
        // Check two ahead
        Vector2<int> pushPos = boxStart + direction;
        Vector2<int> checkPos = pushPos + direction;
        switch (warehouse[checkPos])
        {
            case Element.EMPTY:
            case Element.BOX_LEFT  when TryMoveBigBox(checkPos, warehouse, direction):
            case Element.BOX_RIGHT when TryMoveBigBox(checkPos, warehouse, direction):
                // Push box along
                if (direction is Direction.RIGHT)
                {
                    warehouse[pushPos]  = Element.BOX_LEFT;
                    warehouse[checkPos] = Element.BOX_RIGHT;
                }
                else
                {
                    warehouse[pushPos]  = Element.BOX_RIGHT;
                    warehouse[checkPos] = Element.BOX_LEFT;
                }
                warehouse[boxStart] = Element.EMPTY;
                return true;

            case Element.WALL:
            case Element.BOX_LEFT:
            case Element.BOX_RIGHT:
                return false;

            case Element.BOX:
            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }
    }

    private static bool CheckBoxPushableVertical(Vector2<int> boxPos, ConsoleView<Element> warehouse, Direction direction)
    {
        // Check the first push
        Vector2<int> pushPos = boxPos + direction;
        switch (warehouse[pushPos])
        {
            case Element.EMPTY:
            case Element.BOX_LEFT  when CheckBoxPushableVertical(pushPos, warehouse, direction):
            case Element.BOX_RIGHT when CheckBoxPushableVertical(pushPos, warehouse, direction):
                break;

            case Element.BOX_LEFT:
            case Element.BOX_RIGHT:
            case Element.WALL:
                return false;

            case Element.BOX:
            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }

        // Get the second push location
        if (warehouse[boxPos] is Element.BOX_LEFT)
        {
            pushPos += Vector2<int>.Right;
        }
        else
        {
            pushPos += Vector2<int>.Left;
        }

        // Check the second push
        switch (warehouse[pushPos])
        {
            case Element.EMPTY:
            case Element.BOX_LEFT  when CheckBoxPushableVertical(pushPos, warehouse, direction):
            case Element.BOX_RIGHT when CheckBoxPushableVertical(pushPos, warehouse, direction):
                return true;

            case Element.BOX_LEFT:
            case Element.BOX_RIGHT:
            case Element.WALL:
                return false;

            case Element.BOX:
            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }
    }

    private static void PushBoxVertically(Vector2<int> boxPos, ConsoleView<Element> warehouse, Direction direction)
    {
        // Get both box and push locations
        Vector2<int> leftPos = boxPos;
        Vector2<int> leftPush = boxPos + direction;
        Vector2<int> rightPos, rightPush;
        if (warehouse[leftPos] is Element.BOX_LEFT)
        {
            rightPos  = boxPos + Vector2<int>.Right;
            rightPush = leftPush + Vector2<int>.Right;
        }
        else
        {
            rightPos =  leftPos;
            leftPos  += Vector2<int>.Left;

            rightPush  = leftPush;
            leftPush  += Vector2<int>.Left;
        }

        // Push down if needed
        if (warehouse[leftPush] is Element.BOX_LEFT or Element.BOX_RIGHT)
        {
            PushBoxVertically(leftPush, warehouse, direction);
        }
        if (warehouse[rightPush] is Element.BOX_LEFT or Element.BOX_RIGHT)
        {
            PushBoxVertically(rightPush, warehouse, direction);
        }

        // Complete push
        warehouse[leftPush]  = Element.BOX_LEFT;
        warehouse[rightPush] = Element.BOX_RIGHT;
        warehouse[leftPos]   = Element.EMPTY;
        warehouse[rightPos]  = Element.EMPTY;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Grid<Element>, Direction[]) Convert(string[] rawInput)
    {
        int separator = rawInput.IndexOf(string.Empty);
        string[] grid = rawInput[..separator];
        Grid<Element> warehouse = new(grid[0].Length, grid.Length, grid,
                                      l => l.AsSpan().Select(c => (Element)c).ToArray(),
                                      e => ((char)e).ToString());

        Direction[] moves = rawInput.AsSpan(separator + 1)
                                    .SelectMany(l => l.AsSpan().Select(Direction.Parse))
                                    .ToArray();
        return (warehouse, moves);
    }
}
