using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
public class Day15 : Solver<(Grid<Day15.Element> warehouse, Direction[] moves)>
{
    /// <summary>
    /// Warehouse element
    /// </summary>
    public enum Element : ushort
    {
        EMPTY = '.',
        WALL  = '#',
        BOX   = 'O',
        ROBOT = '@'
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day15(string input) : base(input, options: StringSplitOptions.TrimEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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

                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
        }

        // Calculate final coordinates
        int boxCount = 0;
        int coordinates = 0;
        foreach (Vector2<int> pos in warehouse.Dimensions.EnumerateOver().Where(p => warehouse[p] is Element.BOX))
        {
            coordinates += (100 * pos.Y) + pos.X;
            boxCount++;
        }
        AoCUtils.LogPart1(coordinates);
        //AoCUtils.Log(warehouse);

        // Double warehouse size horizontally
        HashSet<Vector2<int>> boxes = new(boxCount);
        Grid<Element> bigWarehouse = new(this.Data.warehouse.Width * 2, this.Data.warehouse.Height, e => ((char)e).ToString());
        foreach (Vector2<int> pos in this.Data.warehouse.Dimensions.EnumerateOver())
        {
            Element currentElement    = this.Data.warehouse[pos];
            Vector2<int> doubledPosA  = pos with { X = pos.X * 2 };
            Vector2<int> doubledPosB  = doubledPosA + Vector2<int>.Right;
            bigWarehouse[doubledPosA] = currentElement;
            bigWarehouse[doubledPosB] = currentElement;
            if (currentElement is Element.BOX)
            {
                boxes.Add(doubledPosA);
            }
        }

        // Setup start position and iterate through moves
        position = startPosition with { X = startPosition.X * 2 };
        foreach (Direction move in this.Data.moves)
        {
            Vector2<int> newPosition = position + move;
            switch (bigWarehouse[newPosition])
            {
                case Element.EMPTY:
                case Element.BOX when TryMoveBigBox(newPosition, bigWarehouse, boxes, move):
                    position = newPosition;
                    break;

                case Element.BOX:
                case Element.WALL:
                    break;

                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
        }

        // Calculate final coordinates
        coordinates = boxes.Where(pos => bigWarehouse[pos] is Element.BOX).Sum(pos => (100 * pos.Y) + pos.X);

        AoCUtils.LogPart2(coordinates);
        //AoCUtils.Log(bigWarehouse);
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

                case Element.ROBOT:
                default:
                    throw new UnreachableException("This shouldn't happen.");
            }
        }
    }

    private static bool TryMoveBigBox(Vector2<int> boxStart, Grid<Element> warehouse, HashSet<Vector2<int>> boxes, Direction direction)
    {
        if (direction.IsHorizontal())
        {
            // Horizontal movement
            return TryMoveBoxHorizontal(boxStart, warehouse, boxes, direction);
        }

        // Vertical movement, check first
        if (!CheckBoxPushableVertical(boxStart, warehouse, boxes, direction)) return false;

        // Then push if valid
        PushBoxVertically(boxStart, warehouse, boxes, direction);
        return true;

    }

    private static bool TryMoveBoxHorizontal(Vector2<int> boxStart, Grid<Element> warehouse, HashSet<Vector2<int>> boxes, Direction direction)
    {
        // Check two ahead
        Vector2<int> pushPos = boxStart + direction;
        Vector2<int> checkPos = pushPos + direction;
        switch (warehouse[checkPos])
        {
            case Element.EMPTY:
            case Element.BOX when TryMoveBigBox(checkPos, warehouse, boxes, direction):
                // Push box along
                warehouse[checkPos] = Element.BOX;
                warehouse[boxStart] = Element.EMPTY;
                if (direction is Direction.RIGHT)
                {
                    boxes.Remove(boxStart);
                    boxes.Add(pushPos);
                }
                else
                {
                    boxes.Remove(pushPos);
                    boxes.Add(checkPos);
                }
                return true;

            case Element.WALL:
            case Element.BOX:
                return false;

            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }
    }

    private static bool CheckBoxPushableVertical(Vector2<int> boxPos, Grid<Element> warehouse, HashSet<Vector2<int>> boxes, Direction direction)
    {
        // Check the first push
        Vector2<int> pushPos = boxPos + direction;
        switch (warehouse[pushPos])
        {
            case Element.EMPTY:
            case Element.BOX when CheckBoxPushableVertical(pushPos, warehouse, boxes, direction):
                break;

            case Element.BOX:
            case Element.WALL:
                return false;

            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }

        // Get the second push location
        if (boxes.Contains(boxPos))
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
            case Element.BOX when CheckBoxPushableVertical(pushPos, warehouse, boxes, direction):
                return true;

            case Element.BOX:
            case Element.WALL:
                return false;

            case Element.ROBOT:
            default:
                throw new UnreachableException("This shouldn't happen.");
        }
    }

    private static void PushBoxVertically(Vector2<int> boxPos, Grid<Element> warehouse, HashSet<Vector2<int>> boxes, Direction direction)
    {
        // Get both box and push locations
        Vector2<int> leftPos = boxPos;
        Vector2<int> leftPush = boxPos + direction;
        Vector2<int> rightPos, rightPush;
        if (boxes.Contains(leftPos))
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
        if (warehouse[leftPush] is Element.BOX)
        {
            PushBoxVertically(leftPush, warehouse, boxes, direction);
        }
        if (warehouse[rightPush] is Element.BOX)
        {
            PushBoxVertically(rightPush, warehouse, boxes, direction);
        }

        // Complete push
        boxes.Remove(leftPos);
        boxes.Add(leftPush);
        warehouse[leftPush]  = Element.BOX;
        warehouse[rightPush] = Element.BOX;
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
                                    .SelectMany(l => l.AsSpan().Select(DirectionsUtils.Parse))
                                    .ToArray();
        return (warehouse, moves);
    }
    #endregion
}