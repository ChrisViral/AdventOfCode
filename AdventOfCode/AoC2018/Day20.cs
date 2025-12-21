using System.Diagnostics;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 20
/// </summary>
public sealed class Day20 : Solver<string>
{
    [DebuggerDisplay("{Position}")]
    public sealed class Room(Vector2<int> position) : IEquatable<Room>
    {
        public Vector2<int> Position { get; } = position;

        public int Depth { get; set; }

        public Dictionary<Direction, Room> Connections { get; } = new(4);

        public void ConnectToRoom(Direction direction, Room room)
        {
            // Add both ways if not already done
            if (this.Connections.TryAdd(direction, room))
            {
                room.Connections[direction.Invert()] = this;
            }
        }

        /// <inheritdoc />
        public bool Equals(Room? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Position.Equals(other.Position);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Room other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Position.GetHashCode();
    }

    private const char GROUP_START = '(';
    private const char GROUP_END = ')';
    private const char BRANCH_END = '|';

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Explore rooms
        Room start = new(Vector2<int>.Zero);
        ExploreAllRooms(start, out Dictionary<Vector2<int>, Room> map);

        // Calculate depths
        SetRoomDepths(start);

        // Answers
        int furthestRoom = map.Values.Max(r => r.Depth);
        AoCUtils.LogPart1(furthestRoom);
        int farRooms = map.Values.Count(r => r.Depth >= 1000);
        AoCUtils.LogPart2(farRooms);
    }

    private void ExploreAllRooms(Room start, out Dictionary<Vector2<int>, Room> map)
    {
        // Setup and explore from start
        map = new Dictionary<Vector2<int>, Room>(1000) { [start.Position] = start };
        ReadOnlySpan<char> regex = this.Data.AsSpan(1..^1);
        ExploreRooms(ref regex, start, map, new HashSet<Room>(1000));
    }

    // ReSharper disable once CognitiveComplexity
    private static void ExploreRooms(ref ReadOnlySpan<char> regex, Room currentRoom, Dictionary<Vector2<int>, Room> map, HashSet<Room> ends)
    {
        for (int i = 0; i < regex.Length; i++)
        {
            char instruction = regex[i];
            switch (instruction)
            {
                case GROUP_START:
                {
                    // Consume previous characters
                    regex = regex[i..];

                    // Explore all branches of the group
                    using Pooled<HashSet<Room>> groupEnds = HashSetObjectPool<Room>.Shared.Get();
                    ExploreGroup(ref regex, currentRoom, map, groupEnds.Ref);
                    if (groupEnds.Ref.Count is 1)
                    {
                        // If only one end room, set it as current room and reset to start of regex
                        currentRoom = groupEnds.Ref.First();
                        i = -1;
                        break;
                    }

                    if (regex.IsEmpty || regex[0] is BRANCH_END or GROUP_END)
                    {
                        // If group is ended, return all ends
                        ends.UnionWith(groupEnds.Ref);
                        return;
                    }

                    // Otherwise, explore all branches from their starting rooms
                    foreach (Room branchStart in groupEnds.Ref)
                    {
                        ReadOnlySpan<char> branchRegex = regex;
                        ExploreRooms(ref branchRegex, branchStart, map, ends);
                    }
                    return;
                }

                case BRANCH_END:
                case GROUP_END:
                    // Branch end, consume used characters and return final room
                    regex = regex[i..];
                    ends.Add(currentRoom);
                    return;

                default:
                    // Move to room in given direction
                    Direction direction = Direction.ParseDirection(instruction);
                    Vector2<int> newPosition = currentRoom.Position + direction;
                    if (!map.TryGetValue(newPosition, out Room? newRoom))
                    {
                        // Create the room if it does not exist
                        newRoom = new Room(newPosition);
                        map[newPosition] = newRoom;
                    }

                    // Connect to current room and then continue
                    currentRoom.ConnectToRoom(direction, newRoom);
                    currentRoom = newRoom;
                    break;
            }
        }

        // Regex end, return final room
        ends.Add(currentRoom);
    }

    private static void ExploreGroup(ref ReadOnlySpan<char> regex, Room start, Dictionary<Vector2<int>, Room> map, HashSet<Room> ends)
    {
        do
        {
            // Consume group/branch start character
            regex = regex[1..];

            // Keep all possible end points for this branch
            ExploreRooms(ref regex, start, map, ends);
        }
        while (regex[0] is not GROUP_END);

        // Consume group end character
        regex = regex[1..];
    }

    private static void SetRoomDepths(Room start)
    {
        // Explore rooms with BFS
        using Pooled<HashSet<Room>> explored = HashSetObjectPool<Room>.Shared.Get();
        Queue<Room> toExplore = new(10);
        toExplore.Enqueue(start);
        explored.Ref.Add(start);
        while (toExplore.TryDequeue(out Room? current))
        {
            foreach (Room connected in current.Connections.Values.Where(explored.Ref.Add))
            {
                // Room depth becomes one more than it's parent
                connected.Depth = current.Depth + 1;
                toExplore.Enqueue(connected);
            }
        }
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
