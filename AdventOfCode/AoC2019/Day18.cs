using System.Buffers;
using System.Diagnostics;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Pooling;
using AdventOfCode.Collections.Search;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 18
/// </summary>
public sealed class Day18 : GridSolver<char>
{
    /// <summary>Empty position value</summary>
    private const char EMPTY       = '.';
    /// <summary>Wall position value</summary>
    private const char WALL        = '#';
    /// <summary>Entrance position value</summary>
    private const char ENTRANCE    = '@';
    /// <summary>Valid key IDs</summary>
    private const string KEY_IDS   = StringUtils.ALPHABET_UPPER;
    /// <summary>Amount of robots to use</summary>
    private const int ROBOTS_COUNT = 4;

    /// <summary>
    /// Robots container
    /// </summary>
    private static readonly Robot[] Robots = new Robot[ROBOTS_COUNT];
    /// <summary>
    /// Key ID offset
    /// </summary>
    private static readonly int IDOffset = StringUtils.ALPHABET_UPPER[0];

    /// <summary>
    /// Branch path data
    /// </summary>
    /// <param name="Position">Position of robots</param>
    /// <param name="Unlocked">Unlocked keys</param>
    /// <typeparam name="T">Position struct type</typeparam>
    private readonly record struct BranchData<T>(T Position, BitVector32 Unlocked) where T : struct;

    /// <summary>
    /// Multi robot position data
    /// </summary>
    /// <param name="A">Robot A position</param>
    /// <param name="B">Robot B position</param>
    /// <param name="C">Robot C position</param>
    /// <param name="D">Robot D position</param>
    [UsedImplicitly]
    private readonly record struct RobotsData(Vector2<int> A, Vector2<int> B, Vector2<int> C, Vector2<int> D)
    {
        public static RobotsData FromArray(Robot[] robots)
        {
            return new RobotsData(robots[0].Position, robots[1].Position, robots[2].Position, robots[3].Position);
        }
    }

    /// <summary>
    /// Key object
    /// </summary>
    /// <param name="ID">Door ID</param>
    /// <param name="DoorPosition">Door position</param>
    /// <param name="KeyPosition">Key position</param>
    /// <param name="Grid">Grid reference</param>
    [DebuggerDisplay("ID: {ID}, Door: {DoorPosition}, Key: {KeyPosition}, Is Open: {IsOpen}, Robot: {Robot}")]
    private sealed record Key(char ID, Vector2<int> DoorPosition, Vector2<int> KeyPosition, Grid<char> Grid)
    {
        /// <summary>
        /// Move data
        /// </summary>
        /// <param name="Start">Move start</param>
        /// <param name="End">Move end</param>
        /// <param name="Unlocked">Unlocked keys</param>
        public readonly record struct MoveData(Vector2<int> Start, Vector2<int> End, BitVector32 Unlocked);

        /// <summary>
        /// Move data cache
        /// </summary>
        public static readonly Dictionary<MoveData, int?> Moves = new(100);

        /// <summary>
        /// Key ID
        /// </summary>
        private readonly char keyID = char.ToLowerInvariant(ID);

        /// <summary>
        /// Robot associated to this Key
        /// </summary>
        public required Robot Robot { get; set; }

        /// <summary>
        /// Whether this key is opened or not
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Sets the opened status of this key
        /// </summary>
        /// <param name="isOpen">Whether the key is currently unlocked</param>
        /// <param name="unlocked">Unlocked keys data</param>
        public void SetOpen(bool isOpen, ref BitVector32 unlocked)
        {
            this.IsOpen = isOpen;
            unlocked[this.ID - IDOffset] = isOpen;
            if (isOpen)
            {
                this.Grid[this.DoorPosition] = EMPTY;
                this.Grid[this.KeyPosition]  = EMPTY;
            }
            else
            {
                this.Grid[this.DoorPosition] = this.ID;
                this.Grid[this.KeyPosition]  = this.keyID;
            }
        }

        /// <summary>
        /// Gets the path length for the robot to the key given the unlocked keys
        /// </summary>
        /// <param name="unlocked">Unlocked keys</param>
        /// <returns>The path length to the key, or null if it cannot be reached</returns>
        public int? GetCurrentPathLength(BitVector32 unlocked)
        {
            MoveData move = new(this.Robot.Position, this.KeyPosition, unlocked);
            if (!Moves.TryGetValue(move, out int? length))
            {
                length = SearchUtils.GetPathLengthBFS(this.Robot.Position, this.KeyPosition, Neighbours);
                Moves.Add(move, length);
            }

            return length;
        }

        /// <summary>
        /// Checks if the given robot can path to the given key, ignoring doors and other keys in the way
        /// </summary>
        /// <param name="key">Key to get to</param>
        /// <param name="robot">Robot trying to reach key</param>
        /// <returns><see langword="true"/> if the robot can reach the key, otherwise <see langword="false"/></returns>
        public static bool CanPathTo(Key key, Robot robot)
        {
            return SearchUtils.GetPathLengthBFS(robot.Position, key.KeyPosition, key.NeighboursIgnoreDoors) is not null;
        }

        /// <summary>
        /// Enumerates the valid neighbours from a given position
        /// </summary>
        /// <param name="currentPosition">Current position</param>
        /// <returns>Neighbours from this position</returns>
        private IEnumerable<Vector2<int>> Neighbours(Vector2<int> currentPosition)
        {
            bool EmptyOrKey(char value) => value is EMPTY || value == this.keyID;

            return currentPosition.AsAdjacentEnumerable().Where(a => EmptyOrKey(this.Grid[a]));
        }

        /// <summary>
        /// Enumerates the valid neighbours from a given position, ignoring doors
        /// </summary>
        /// <param name="currentPosition">Current position</param>
        /// <returns>Neighbours from this position</returns>
        private IEnumerable<Vector2<int>> NeighboursIgnoreDoors(Vector2<int> currentPosition)
        {
            bool NotWallOrKey(char value) => value is not WALL || value == this.keyID;

            return currentPosition.AsAdjacentEnumerable().Where(a => NotWallOrKey(this.Grid[a]));
        }

        /// <inheritdoc />
        public bool Equals(Key? other) => other is not null && other.ID == this.ID;

        /// <inheritdoc />
        public override int GetHashCode() => this.ID.GetHashCode();
    }

    /// <summary>
    /// Robot data
    /// </summary>
    /// <param name="ID">Robot ID</param>
    [DebuggerDisplay("ID: {ID}, Position: {Position}")]
    public sealed record Robot(int ID)
    {
        /// <summary>
        /// Robot position in Grid
        /// </summary>
        public Vector2<int> Position { get; set; }

        /// <inheritdoc />
        public bool Equals(Robot? other) => other is not null && other.ID == this.ID;

        /// <inheritdoc />
        public override int GetHashCode() => this.ID;
    }

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Get start position
        Vector2<int> start = this.Grid.PositionOf(ENTRANCE);
        this.Grid[start]   = EMPTY;

        // Create starting robot
        Robot robot = new(0) { Position = start };

        // Find keys
        Key[] keys = new Key[KEY_IDS.Length];
        foreach (int i in ..keys.Length)
        {
            char id = KEY_IDS[i];
            Vector2<int> doorPosition = this.Grid.PositionOf(id);
            Vector2<int> keyPosition  = this.Grid.PositionOf(char.ToLowerInvariant(id));
            keys[i] = new Key(id, doorPosition, keyPosition, this.Grid) { Robot = robot };
        }

        // Run part 1
        int pathLength = FindBestKeyPath(keys, () => robot.Position);
        AoCUtils.LogPart1(pathLength);

        // Update grid
        this.Grid[start] = WALL;
        this.Grid[start + Vector2<int>.Up]    = WALL;
        this.Grid[start + Vector2<int>.Down]  = WALL;
        this.Grid[start + Vector2<int>.Left]  = WALL;
        this.Grid[start + Vector2<int>.Right] = WALL;

        // Reset previous data
        Key.Moves.Clear();
        robot.Position = start + new Vector2<int>(-1, -1);

        // Create new robots
        Robots[0] = robot;
        Robots[1] = new Robot(1) { Position = start + new Vector2<int>( 1, -1) };
        Robots[2] = new Robot(2) { Position = start + new Vector2<int>( 1,  1) };
        Robots[3] = new Robot(3) { Position = start + new Vector2<int>(-1,  1) };

        // Update which robot is associated to which key
        keys.ForEach(k => k.Robot = Robots.First(r => Key.CanPathTo(k, r)));

        // Run part 2
        pathLength = FindBestKeyPath(keys, () => RobotsData.FromArray(Robots));
        AoCUtils.LogPart2(pathLength);
    }

    /// <summary>
    /// Find the best path to get all the keys
    /// </summary>
    /// <param name="keys">Keys to get</param>
    /// <param name="getPositions">Function providing a struct containing the position data of all current robots</param>
    /// <typeparam name="T">Struct type used for robot position data aggregating</typeparam>
    /// <returns>The length of the best path to collect all the keys</returns>
    // ReSharper disable once CognitiveComplexity
    private static int FindBestKeyPath<T>(Key[] keys, Func<T> getPositions)
        where T : struct
    {
        // ReSharper disable once CognitiveComplexity
        int FindBestKeyPathInternal(HashSet<Key> remaining, ref BitVector32 unlocked, Dictionary<BranchData<T>, int> branchCache, int distanceSoFar, int bestSoFar)
        {
            int best = int.MaxValue;
            PooledArray<Key> keyArray = ArrayPool<Key>.Shared.RentTracked(remaining.Count);
            remaining.CopyTo(keyArray.Ref);
            foreach (Key key in keyArray.Ref.AsSpan(0, remaining.Count))
            {
                // Get path length to key
                int? keyDistance = key.GetCurrentPathLength(unlocked);
                if (keyDistance is null) continue;

                int newDistanceSoFar = distanceSoFar + keyDistance.Value;
                if (newDistanceSoFar >= bestSoFar) continue;

                if (remaining.Count is 1)
                {
                    best = Math.Min(best, newDistanceSoFar);
                    continue;
                }

                // Open door and set robot to the position of the key
                key.SetOpen(true, ref unlocked);
                remaining.Remove(key);
                Vector2<int> previousPosition = key.Robot.Position;
                key.Robot.Position = key.KeyPosition;

                // Recurse for path length
                int distance;
                BranchData<T> branch = new(getPositions(), unlocked);
                if (branchCache.TryGetValue(branch, out int subDistance))
                {
                    // Cached path found, calculate final distance
                    distance = subDistance is not int.MaxValue ? newDistanceSoFar + subDistance : int.MaxValue;
                }
                else
                {
                    // Cached path not found, recurse to calculate length
                    distance = FindBestKeyPathInternal(remaining, ref unlocked, branchCache, newDistanceSoFar, bestSoFar);
                    subDistance = distance is not int.MaxValue ? distance - newDistanceSoFar : int.MaxValue;
                    branchCache.Add(branch, subDistance);
                }

                // Restore robot position and door
                key.Robot.Position = previousPosition;
                remaining.Add(key);
                key.SetOpen(false, ref unlocked);

                // If a valid path is found, check if it's better than what we have
                best = Math.Min(best, distance);
            }

            return best;
        }

        BitVector32 unlockedRef = new();
        Dictionary<BranchData<T>, int> cache = new(100);
        HashSet<Key> remainingKeys = new(keys);
        return FindBestKeyPathInternal(remainingKeys, ref unlockedRef, cache, 0, int.MaxValue);
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
