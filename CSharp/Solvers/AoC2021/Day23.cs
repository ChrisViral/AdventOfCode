using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 22
/// </summary>
public class Day23 : Solver<(Stack<Day23.Amphipod> a, Stack<Day23.Amphipod> b, Stack<Day23.Amphipod> c, Stack<Day23.Amphipod> d)>
{
    /// <summary>
    /// Amphipod types
    /// </summary>
    public enum Amphipod
    {
        NONE = 0,
        A = 1,
        B = 10,
        C = 100,
        D = 1000
    }

    /// <summary>
    /// Node move data
    /// </summary>
    /// <param name="Blockers">Hallway nodes that might block the move</param>
    /// <param name="Distance">Move distance</param>
    private readonly record struct MoveData(HallwayNode[] Blockers, int Distance);

    /// <summary>
    /// Amphipod graph nodes data
    /// </summary>
    /// <param name="Rooms">Room nodes</param>
    /// <param name="Hallways">Hallway nodes</param>
    /// <param name="Paths">Moves data map</param>
    private readonly record struct GraphData(RoomNode[] Rooms, HallwayNode[] Hallways, FrozenDictionary<(AmphipodNode, AmphipodNode), MoveData> Paths);

    /// <summary>
    /// Amphipod location node
    /// </summary>
    /// <param name="id">Node identifier</param>
    private abstract class AmphipodNode(string id) : IEquatable<AmphipodNode>
    {
        /// <summary>
        /// Node identifier
        /// </summary>
        protected readonly string id = id;

        /// <summary>
        /// Current amphipod in the node
        /// </summary>
        public virtual Amphipod Current { get; protected set; }

        /// <summary>
        /// Travel distance required within this node
        /// </summary>
        public virtual int NodeTravel => 0;

        /// <summary>
        /// Tries to accomodate the given amphipod in the node, and calculates the required energy
        /// </summary>
        /// <param name="from">Node the amphipod is coming from</param>
        /// <param name="energy">Energy expanded output parameter</param>
        /// <param name="paths">Dictionary of path lengths between nodes</param>
        /// <returns><see langword="true"/> if the amphipod could be accommodated, otherwise <see langword="false"/></returns>
        public abstract bool TryAccomodate(AmphipodNode from, out int energy, FrozenDictionary<(AmphipodNode, AmphipodNode), MoveData> paths);

        /// <summary>
        /// Removes the current Amphipod from the node and returns it
        /// </summary>
        public abstract Amphipod RemoveCurrent();

        /// <summary>
        /// Force the amphipod into the current node
        /// </summary>
        /// <param name="amphipod">Amphipod to force in</param>
        public abstract void ForceSet(Amphipod amphipod);

        /// <inheritdoc />
        public bool Equals(AmphipodNode? other) => this.id == other?.id;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is AmphipodNode other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.id.GetHashCode();
    }

    /// <summary>
    /// Amphipod hallway node
    /// </summary>
    /// <param name="id">Node identifier</param>
    private sealed class HallwayNode(string id) : AmphipodNode(id)
    {
        /// <inheritdoc />
        public override bool TryAccomodate(AmphipodNode from, out int energy, FrozenDictionary<(AmphipodNode, AmphipodNode), MoveData> paths)
        {
            if (this.Current is Amphipod.NONE)
            {
                (HallwayNode[] blockers, int distance) = paths[(from, this)];
                if (IsPassable(blockers))
                {
                    this.Current = from.RemoveCurrent();
                    energy       = (distance + from.NodeTravel) * (int)this.Current;
                    return true;
                }
            }

            energy = 0;
            return false;
        }

        /// <inheritdoc />
        public override Amphipod RemoveCurrent()
        {
            (Amphipod current, this.Current) = (this.Current, Amphipod.NONE);
            return current;
        }

        /// <inheritdoc />
        public override void ForceSet(Amphipod amphipod) => this.Current = amphipod;

        /// <inheritdoc />
        public override string ToString() => $"[Hall {this.id}]: {this.Current}";
    }

    /// <summary>
    /// Amphipod room node
    /// </summary>
    /// <param name="id">Node identifier</param>
    /// <param name="type">Amphipod type</param>
    /// <param name="room">Room stack</param>
    private sealed class RoomNode(string id, Amphipod type, Stack<Amphipod> room) : AmphipodNode(id)
    {
        /// <inheritdoc />
        public override Amphipod Current
        {
            get => room.TryPeek(out Amphipod current) ? current : Amphipod.NONE;
            protected set => throw new NotSupportedException("Cannot set current amphipod in room node");
        }

        /// <inheritdoc />
        public override int NodeTravel => this.RoomCapacity - room.Count - 1;

        /// <summary>
        /// Maximum capacity of this room
        /// </summary>
        public int RoomCapacity { get; set; } = 2;

        /// <summary>
        /// Checks if this room is sorted
        /// </summary>
        public bool IsSorted => room.Count == this.RoomCapacity && room.All(a => a == type);

        /// <summary>
        /// If this room has intruders
        /// </summary>
        public bool HasIntruders => room.Any(a => a != type);

        /// <inheritdoc />
        public override bool TryAccomodate(AmphipodNode from, out int energy, FrozenDictionary<(AmphipodNode, AmphipodNode), MoveData> paths)
        {
            Amphipod arriving = from.Current;
            if (arriving == type && room.Count <= RoomCapacity && room.All(a => a == type))
            {
                (HallwayNode[] blockers, int distance) = paths[(from, this)];
                if (IsPassable(blockers))
                {
                    from.RemoveCurrent();
                    energy = (distance + this.NodeTravel + from.NodeTravel) * (int)arriving;
                    room.Push(arriving);
                    return true;
                }
            }

            energy = 0;
            return false;
        }

        /// <inheritdoc />
        public override Amphipod RemoveCurrent() => room.Pop();

        /// <inheritdoc />
        public override void ForceSet(Amphipod amphipod) => room.Push(amphipod);

        /// <inheritdoc />
        public override string ToString() => $"[Room {this.id}]: {string.Join(',', room)}";
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day23(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Setup graph nodes
        GraphData data = SetupGraph(this.Data.a, this.Data.b, this.Data.c, this.Data.d);

        // Sort into respective rooms
        int minEnergy = SortAmphipods(data.Rooms, data.Hallways, data.Paths);
        AoCUtils.LogPart1(minEnergy);

        // Update room capacities
        data.Rooms.ForEach(r => r.RoomCapacity = 4);

        // Room A
        RoomNode room = data.Rooms[0];
        Amphipod top  = room.RemoveCurrent();
        room.ForceSet(Amphipod.D);
        room.ForceSet(Amphipod.D);
        room.ForceSet(top);

        // Room B
        room = data.Rooms[1];
        top  = room.RemoveCurrent();
        room.ForceSet(Amphipod.B);
        room.ForceSet(Amphipod.C);
        room.ForceSet(top);

        // Room C
        room = data.Rooms[2];
        top  = room.RemoveCurrent();
        room.ForceSet(Amphipod.A);
        room.ForceSet(Amphipod.B);
        room.ForceSet(top);

        // Room D
        room = data.Rooms[3];
        top  = room.RemoveCurrent();
        room.ForceSet(Amphipod.C);
        room.ForceSet(Amphipod.A);
        room.ForceSet(top);

        // Sort into respective rooms
        minEnergy = SortAmphipods(data.Rooms, data.Hallways, data.Paths);
        AoCUtils.LogPart2(minEnergy);
    }

    /// <summary>
    /// Sorts the amphipods into their respective rooms
    /// </summary>
    /// <param name="rooms"></param>
    /// <param name="hallways"></param>
    /// <param name="pathLengths"></param>
    /// <returns></returns>
    private static int SortAmphipods(RoomNode[] rooms, HallwayNode[] hallways, FrozenDictionary<(AmphipodNode, AmphipodNode), MoveData> pathLengths)
    {
        void FindLeastEnergySort(int usedEnergy, ref int minEnergy)
        {
            // If we've used more energy than the best we've found so far, no need to keep looking
            if (usedEnergy > minEnergy) return;

            // If we're done, store the result if it's our best
            if (rooms.All(r => r.IsSorted))
            {
                minEnergy = Math.Min(minEnergy, usedEnergy);
                return;
            }

            // See if we can't move an amphipod directly to it's target room as that's always an optimal move
            foreach (HallwayNode hallway in hallways)
            {
                // Get target index
                int index = GetRoomIndex(hallway.Current);
                if (index is -1) continue;

                // Try and accomodate the amphipod
                RoomNode target = rooms[index];
                if (target.TryAccomodate(hallway, out int moveEnergy, pathLengths))
                {
                    // Recurse down
                    FindLeastEnergySort(usedEnergy + moveEnergy, ref minEnergy);
                    // Revert back to previous state
                    hallway.ForceSet(target.RemoveCurrent());

                    // This is always the best move, so if it's possible at all, we don't need to check other variations
                    return;
                }
            }

            // Similarly, check for room->room moves
            foreach (int i in ..rooms.Length)
            {
                // Get current room
                RoomNode room = rooms[i];

                // Get target index
                int index = GetRoomIndex(room.Current);
                if (index == i || index is -1) continue;

                // Try and accomodate the amphipod
                RoomNode target = rooms[index];
                if (target.TryAccomodate(room, out int moveEnergy, pathLengths))
                {
                    // Recurse down
                    FindLeastEnergySort(usedEnergy + moveEnergy, ref minEnergy);
                    // Revert back to previous state
                    room.ForceSet(target.RemoveCurrent());

                    // This is always the best move, so if it's possible at all, we don't need to check other variations
                    return;
                }
            }

            // Once we clear these, we try and move an amphipod to the hallway and recurse
            foreach (RoomNode room in rooms)
            {
                // We can't move nothing from a room
                if (room.Current is Amphipod.NONE || !room.HasIntruders) continue;

                // Test all hallways
                foreach (HallwayNode hallway in hallways)
                {
                    // Check if the hallway can take the amphipod
                    if (hallway.TryAccomodate(room, out int moveEnergy, pathLengths))
                    {
                        // If it can, recurse down
                        FindLeastEnergySort(usedEnergy + moveEnergy, ref minEnergy);
                        // Then reset state
                        room.ForceSet(hallway.RemoveCurrent());
                    }
                }
            }
        }

        int minUsedEnergy = int.MaxValue;
        FindLeastEnergySort(0, ref minUsedEnergy);
        return minUsedEnergy;
    }

    /// <summary>
    /// Gets the room index of an amphipod
    /// </summary>
    /// <param name="amphipod">Amphipod to get the room index for</param>
    /// <returns>Room index of this amphipod</returns>
    /// <exception cref="InvalidEnumArgumentException">For unknown values of <paramref name="amphipod"/></exception>
    private static int GetRoomIndex(Amphipod amphipod) => amphipod switch
    {
        Amphipod.NONE => -1,
        Amphipod.A    => 0,
        Amphipod.B    => 1,
        Amphipod.C    => 2,
        Amphipod.D    => 3,
        _             => throw new InvalidEnumArgumentException(nameof(amphipod), (int)amphipod, typeof(Amphipod))
    };

    /// <summary>
    /// Sets up the rooms graph data
    /// </summary>
    /// <param name="a">Room A stack</param>
    /// <param name="b">Room B stack</param>
    /// <param name="c">Room C stack</param>
    /// <param name="d">Room D stack</param>
    /// <returns>The initialized graph data for the given starting room stacks</returns>
    private static GraphData SetupGraph(Stack<Amphipod> a, Stack<Amphipod> b, Stack<Amphipod> c, Stack<Amphipod> d)
    {
        // All room nodes
        RoomNode[] rooms =
        [
            new("A", Amphipod.A, a.CreateCopy()),
            new("B", Amphipod.B, b.CreateCopy()),
            new("C", Amphipod.C, c.CreateCopy()),
            new("D", Amphipod.D, d.CreateCopy()),
        ];

        // All hallway nodes
        HallwayNode[] hallways =
        [
            new("A2"),
            new("A1"),
            new("AB"),
            new("BC"),
            new("CD"),
            new("D1"),
            new("D2")
        ];

        // All nodes in the order they appear in horizontally
        AmphipodNode[] nodes =
        [
            hallways[0],
            hallways[1],
            rooms[0],
            hallways[2],
            rooms[1],
            hallways[3],
            rooms[2],
            hallways[4],
            rooms[3],
            hallways[5],
            hallways[6],
        ];

        // Make distance map
        Dictionary<(AmphipodNode, AmphipodNode), MoveData> paths = new(rooms.Length * hallways.Length * 2);
        foreach (int i in ..nodes.Length)
        {
            if (nodes[i] is not RoomNode room) continue;

            foreach (int j in ..nodes.Length)
            {
                if (i == j) continue;

                // Get the distance and blockers
                int distance = Math.Abs(i - j) + 1;
                HallwayNode[] blockers = (i < j ? nodes.AsSpan((i + 1)..j)
                                                : nodes.AsSpan((j + 1)..i))
                                        .Where(n => n is HallwayNode)
                                        .Cast<HallwayNode>()
                                        .ToArray();

                // Add 1 to distance if target is a room
                AmphipodNode node = nodes[j];
                if (node is RoomNode)
                {
                    distance++;
                }

                // Store distance and blockers
                paths[(room, node)] = new MoveData(blockers, distance);
                paths[(node, room)] = new MoveData(blockers, distance);
            }
        }


        return new GraphData(rooms, hallways, paths.ToFrozenDictionary());
    }

    /// <summary>
    /// Checks if the given set of hallways is passable
    /// </summary>
    /// <param name="blockers">Hallway nodes that might be blocking movement</param>
    /// <returns><see langword="true"/> if the hallways are passable, otherwise <see langword="false"/></returns>
    private static bool IsPassable(HallwayNode[] blockers) => blockers.All(n => n.Current is Amphipod.NONE);

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Stack<Amphipod> a, Stack<Amphipod> b, Stack<Amphipod> c, Stack<Amphipod> d) Convert(string[] rawInput)
    {
        Stack<Amphipod> a = new(4);
        Stack<Amphipod> b = new(4);
        Stack<Amphipod> c = new(4);
        Stack<Amphipod> d = new(4);

        for (int i = 3; i >= 2; i--)
        {
            ReadOnlySpan<char> line = rawInput[i];
            a.Push(Enum.Parse<Amphipod>(line.Slice(3, 1)));
            b.Push(Enum.Parse<Amphipod>(line.Slice(5, 1)));
            c.Push(Enum.Parse<Amphipod>(line.Slice(7, 1)));
            d.Push(Enum.Parse<Amphipod>(line.Slice(9, 1)));
        }

        return (a, b, c, d);
    }
    #endregion
}
