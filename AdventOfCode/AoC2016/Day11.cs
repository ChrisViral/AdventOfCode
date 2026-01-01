using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Collections.Search;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Ranges;
using FastEnumUtility;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 11
/// </summary>
public sealed partial class Day11 : Solver<(Day11.RTG[] objects, Day11.Placements placement)>
{
    /// <summary>
    /// RTG item type
    /// </summary>
    public enum RTGType
    {
        MICROCHIP,
        GENERATOR
    }

    /// <summary>
    /// RTG object
    /// </summary>
    /// <param name="Element">RTG Element</param>
    /// <param name="Type">Item type</param>
    public readonly record struct RTG(string Element, RTGType Type)
    {
        /// <summary>
        /// Object ID
        /// </summary>
        public int ID { get; init; }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RTG other) => this.Element == other.Element && this.Type == other.Type;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(this.Element, this.Type);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"({this.ID}) {this.Element} {this.Type.FastToString().ToLowerInvariant()}";
    }

    /// <summary>
    /// Facility state
    /// </summary>
    /// <param name="Elevator">Elevator position</param>
    /// <param name="Placement">Item placement</param>
    public readonly record struct Facility(int Elevator, Placements Placement);

    /// <summary>
    /// RTG Item placements
    /// </summary>
    [InlineArray(ITEM_COUNT)]
    public struct Placements : IEquatable<Placements>
    {
        private int element;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Placements other) => ((ReadOnlySpan<int>)this).SequenceEqual(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is Placements other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => ((ReadOnlySpan<int>)this).Aggregate(0, HashCode.Combine);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Placements left, Placements right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Placements left, Placements right) => !left.Equals(right);
    }

    private const int FLOOR_COUNT = 4;
    private const int ITEM_COUNT  = 10;

    [GeneratedRegex("([a-z]+)(?: (generator)|-compatible (microchip))")]
    private static partial Regex RTGMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create start state
        Facility start = new(1, this.Data.placement);

        // Create end state
        Placements finalPlacement = new();
        ((Span<int>)finalPlacement).Fill(FLOOR_COUNT);
        Facility end = new(FLOOR_COUNT, finalPlacement);

        // Find best path
        SearchUtils.Search(start, end, MinTravel, GetPotentialMoves, MinSearchComparer<int>.Comparer, out int pathLength);
        AoCUtils.LogPart1(pathLength);

        // Moving a pair of objects from the first floor to the top floor takes 12 steps, so two new pairs adds 24 steps
        AoCUtils.LogPart2(pathLength + 24);
    }

    /// <summary>
    /// Gets the minimum travel distance to get all items to the top floor, used as a heuristic in the search
    /// </summary>
    /// <param name="state">Current facility state</param>
    /// <returns>The minimum expected distance to get all the items to the top floor</returns>
    private int MinTravel(Facility state)
    {
        // This god tier heuristic takes the calculation time from ~1min to ~100ms
        int minTravel = 0;
        foreach (int floor in 1..FLOOR_COUNT)
        {
            // Get item count on floor
            int itemsOnFloor = GetItemsOnFloor(floor, state.Placement).AsValueEnumerable().Count();

            // If no items, nothing to add
            if (itemsOnFloor is 0) continue;

            int distanceFromTop = FLOOR_COUNT - floor;
            if (floor == state.Elevator)
            {
                // On our floor, the first trip can take two items
                minTravel += distanceFromTop;
                itemsOnFloor -= 2;
                if (itemsOnFloor <= 0) continue;
            }
            else
            {
                // When not on our floor, add the travel distance to get to it
                minTravel += Math.Abs(state.Elevator - floor);
            }

            // Add time to take all items to top, one at a time
            minTravel += itemsOnFloor * distanceFromTop * 2;
        }
        return minTravel;
    }

    /// <summary>
    /// Gets all potential moves between floors
    /// </summary>
    /// <param name="state">Current facility state</param>
    /// <returns>An enumerable of all the potential moves to do</returns>
    /// ReSharper disable once CognitiveComplexity
    private IEnumerable<MoveData<Facility, int>> GetPotentialMoves(Facility state)
    {
        // If we're on the top floor, check for pairs we can move in known steps
        if (state.Elevator is FLOOR_COUNT && TryGetFastMoves(state, out int fastSteps, out Placements updatedPlacements))
        {
            // If we can, apply this move and do nothing else
            yield return new MoveData<Facility, int>(state with { Placement = updatedPlacements }, fastSteps);
            yield break;
        }

        // Get list of items on floor
        List<RTG> onFloor = ListObjectPool<RTG>.Shared.Get().Ref;
        onFloor.AddRange(GetItemsOnFloor(state.Elevator, state.Placement));

        // Try only taking one object
        foreach (RTG item in onFloor)
        {
            // Try moving to floor above
            int newFloor = state.Elevator + 1;
            if (CanMoveToFloor(item, newFloor, state.Placement, onFloor))
            {
                Placements newPlacements = state.Placement;
                newPlacements[item.ID] = newFloor;
                yield return new MoveData<Facility, int>(new Facility(newFloor, newPlacements));
            }

            // Try moving to floor below
            newFloor -= 2;
            if (CanMoveToFloor(item, newFloor, state.Placement, onFloor))
            {
                Placements newPlacements = state.Placement;
                newPlacements[item.ID] = newFloor;
                yield return new MoveData<Facility, int>(new Facility(newFloor, newPlacements));
            }
        }

        List<RTG> pairsList = ListObjectPool<RTG>.Shared.Get().Ref;
        pairsList.AddRange(onFloor);
        foreach ((RTG first, RTG second) in pairsList.EnumeratePairs())
        {
            // Try moving to floor above
            int newFloor = state.Elevator + 1;
            if (CanMoveToFloor(first, second, state.Elevator, newFloor, state.Placement, onFloor))
            {
                Placements newPlacements = state.Placement;
                newPlacements[first.ID]  = newFloor;
                newPlacements[second.ID] = newFloor;
                yield return new MoveData<Facility, int>(new Facility(newFloor, newPlacements));
            }

            // Try moving to floor below
            newFloor -= 2;
            if (CanMoveToFloor(first, second, state.Elevator, newFloor, state.Placement, onFloor))
            {
                Placements newPlacements = state.Placement;
                newPlacements[first.ID]  = newFloor;
                newPlacements[second.ID] = newFloor;
                yield return new MoveData<Facility, int>(new Facility(newFloor, newPlacements));
            }
        }

        // Return lists to pool
        ListObjectPool<RTG>.Shared.Return(onFloor);
        ListObjectPool<RTG>.Shared.Return(pairsList);
    }

    private bool TryGetFastMoves(Facility state, out int steps, out Placements placements)
    {
        steps = 0;
        placements = state.Placement;
        for (int floor = 1; floor < FLOOR_COUNT; floor++)
        {
            // Get items that are grouped
            int stepsToTop = (FLOOR_COUNT - floor) * 4;
            foreach (IGrouping<string, RTG> group in GetItemsOnFloor(floor, state.Placement).AsValueEnumerable()
                                                                                            .GroupBy(i => i.Element)
                                                                                            .Where(g => g.AsValueEnumerable().Count() is 2))
            {
                // Move them to top and add steps
                steps += stepsToTop;
                foreach (RTG item in group)
                {
                    placements[item.ID] = FLOOR_COUNT;
                }
            }
        }

        return steps is not 0;
    }

    /// <summary>
    /// Gets all the items on a given floor
    /// </summary>
    /// <param name="floor">Floor to get the items for</param>
    /// <param name="placements">Current item placements</param>
    /// <returns>An enumerable of all the items on the given floor</returns>
    private IEnumerable<RTG> GetItemsOnFloor(int floor, Placements placements)
    {
        // Get items on current floor
        for (int i = 0; i < ITEM_COUNT; i++)
        {
            if (placements[i] == floor)
            {
                yield return this.Data.objects[i];
            }
        }

    }

    /// <summary>
    /// Checks if we can move to a floor with the two given items
    /// </summary>
    /// <param name="first">First item to move with</param>
    /// <param name="second">Second item to move with</param>
    /// <param name="fromFloor">Floor we're moving from</param>
    /// <param name="toFloor">Floor we're moving to</param>
    /// <param name="placements">Current items placement</param>
    /// <param name="fromFloorItems">Items on the current floor</param>
    /// <returns><see langword="true"/> if the move is valid, otherwise <see langword="false"/></returns>
    private bool CanMoveToFloor(RTG first, RTG second, int fromFloor, int toFloor, Placements placements, List<RTG> fromFloorItems)
    {
        // Make sure the floor is valid
        // Never move to the first floor with two items
        // Never move from the fourth floor with two items
        if (toFloor is < 2 or > FLOOR_COUNT
         || fromFloor is FLOOR_COUNT) return false;

        // If we move paired items, the move is always valid
        if (first.Element == second.Element) return true;

        // If both items are of differing types (but not paired), the move is invalid
        if (first.Type != second.Type) return false;

        // Move the second item to the new floor
        placements[second.ID] = toFloor;
        fromFloorItems.RemoveSwap(second);

        // Try moving with the first item
        bool canMove = CanMoveToFloor(first, toFloor, placements, fromFloorItems);

        // If the move fails, we abort
        if (!canMove)
        {
            // Restore second item to original floor
            placements[second.ID] = fromFloor;
            fromFloorItems.Add(second);
            return false;
        }

        // Move first item to new floor
        placements[first.ID] = toFloor;
        fromFloorItems.RemoveSwap(first);

        // Restore second item to original floor
        placements[second.ID] = fromFloor;
        fromFloorItems.Add(second);

        // Try moving again with the second item this time
        canMove = CanMoveToFloor(second, toFloor, placements, fromFloorItems);

        // Restore first item to original floor
        placements[first.ID] = fromFloor;
        fromFloorItems.Add(first);

        // If the second move succeeded, we can definitely move
        return canMove;
    }

    /// <summary>
    /// Checks if we can move to a floor with the given item
    /// </summary>
    /// <param name="item">Item to move with</param>
    /// <param name="toFloor">Floor we're moving to</param>
    /// <param name="placements">Current items placement</param>
    /// <param name="fromFloorItems">Items on the current floor</param>
    /// <returns><see langword="true"/> if the move is valid, otherwise <see langword="false"/></returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">If the item's type is invalid</exception>
    private bool CanMoveToFloor(RTG item, int toFloor, Placements placements, List<RTG> fromFloorItems)
    {
        // Make sure the floor is valid
        if (toFloor is < 1 or > FLOOR_COUNT) return false;

        return item.Type switch
        {
            RTGType.MICROCHIP => CanMoveToFloorMicrochip(item, toFloor, placements),
            RTGType.GENERATOR => CanMoveToFloorGenerator(item, toFloor, placements, fromFloorItems),
            _                 => throw item.Type.Invalid()
        };
    }

    /// <summary>
    /// Checks if we can move to a floor with the given microchip
    /// </summary>
    /// <param name="chip">Chip to move with</param>
    /// <param name="toFloor">Floor we're moving to</param>
    /// <param name="placements">Current items placement</param>
    /// <returns><see langword="true"/> if the move is valid, otherwise <see langword="false"/></returns>
    private bool CanMoveToFloorMicrochip(RTG chip, int toFloor, Placements placements)
    {
        // Get items on the floor we're going to
        using PooledArray<RTG> toFloorItemsPooled = GetItemsOnFloor(toFloor, placements).AsValueEnumerable().ToArrayPool();
        ReadOnlySpan<RTG> toFloorItems = toFloorItemsPooled.Span;

        // If there are no items on the target floor, we can move there
        if (toFloorItems.Length is 0) return true;

        // If there is only one other item on the floor
        if (toFloorItems.Length is 1)
        {
            // We can move there if and only if:
            // - The item is another chip OR
            // - It is our matching generator
            RTG otherItem = toFloorItems[0];
            return otherItem.Type is RTGType.MICROCHIP
                || otherItem.Element == chip.Element;
        }

        // If there are multiple items on the target floor, we can move to the there if and only if:
        // - Our matching generator is on the floor OR
        // - All items on the floor are other chips
        return toFloorItems.Any(i => i.Element == chip.Element)
            || toFloorItems.All(i => i.Type is RTGType.MICROCHIP);
    }

    /// <summary>
    /// Checks if we can move to a floor with the given generator
    /// </summary>
    /// <param name="generator">Generator to move with</param>
    /// <param name="toFloor">Floor we're moving to</param>
    /// <param name="placements">Current items placement</param>
    /// <param name="fromFloorItems">Items on the current floor</param>
    /// <returns><see langword="true"/> if the move is valid, otherwise <see langword="false"/></returns>
    private bool CanMoveToFloorGenerator(RTG generator, int toFloor, Placements placements, List<RTG> fromFloorItems)
    {
        // Cannot leave the floor with our generator if:
        // - There is more than one other item on the floor AND
        // - Our matching item is on the floor we're leaving AND
        // - There exists any other generator on the floor
        if (fromFloorItems.Count > 2
         && fromFloorItems.Exists(i => i != generator && i.Element == generator.Element)
         && fromFloorItems.Exists(i => i.Type is RTGType.GENERATOR))
        {
            return false;
        }

        // Get items on the floor we're going to
        using PooledArray<RTG> toFloorItemsPooled = GetItemsOnFloor(toFloor, placements).AsValueEnumerable().ToArrayPool();
        ReadOnlySpan<RTG> toFloorItems = toFloorItemsPooled.Span;

        // If there are no items on the target floor, we can move there
        if (toFloorItems.Length is 0) return true;

        // If there is only one other item on the floor
        if (toFloorItems.Length is 1)
        {
            // We can move there if and only if:
            // - The item is a generator OR
            // - The item is our matching chip
            RTG otherItem = toFloorItems[0];
            return otherItem.Type is RTGType.GENERATOR
                || otherItem.Element == generator.Element;
        }

        // If there are multiple items on the target floor, we can move to the there if and only if:
        // - All other items are generators OR
        // - There are no unshielded chips on the floor that aren't our matching one
        return toFloorItems.All(i => i.Type is RTGType.GENERATOR)
            || toFloorItems.GroupBy(i => i.Element)
                           .Where(g => g.AsValueEnumerable().Count() is not 2)
                           .Select(g => g.AsValueEnumerable().First())
                           .All(i => i.Element == generator.Element || i.Type is not RTGType.MICROCHIP);
    }

    /// <inheritdoc />
    protected override (RTG[], Placements) Convert(string[] rawInput)
    {
        int id = 0;
        Dictionary<RTG, int> objectsMap = new(ITEM_COUNT);
        RegexFactory<RTG> rtgFactory = new(RTGMatcher);
        foreach (int floor in ..FLOOR_COUNT)
        {
            foreach (RTG rtg in rtgFactory.ConstructObjects(rawInput[floor]))
            {
                objectsMap[rtg with { ID = id++ }] = floor + 1;
            }
        }

        RTG[] objects = objectsMap.Keys.ToArray();
        Placements placement = new();
        foreach (RTG rtg in objects)
        {
            placement[rtg.ID] = objectsMap[rtg];
        }

        return (objects, placement);
    }
}
