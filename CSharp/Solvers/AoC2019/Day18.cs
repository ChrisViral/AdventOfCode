using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using Microsoft.VisualBasic.CompilerServices;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 18
    /// </summary>
    public class Day18 : Solver<Day18.Maze>
    {
        /// <summary>
        /// Lock record
        /// </summary>
        public record Lock(char ID, Vector2 Key, Vector2 Door, int Bitwise, int Required)
        {
            #region Constructors
            /// <summary>
            /// Creates a new default lock from the given ID, key, and door
            /// </summary>
            /// <param name="id">Character ID of the door</param>
            public Lock(char id) : this(id, Vector2.Zero, Vector2.Zero, 1 << (id - 'a'), 0) { }
            #endregion

            #region Methods
            /// <summary>
            /// Check if the Lock's key is reachable with a given set of keys
            /// </summary>
            /// <param name="keys">Currently available keys</param>
            /// <returns>True if the key for this lock can be reached, false otherwise</returns>
            public bool IsReachable(int keys) => (keys & this.Bitwise) is 0 && (keys & this.Required) == this.Required;

            /// <inheritdoc cref="object.GetHashCode"/>
            public override int GetHashCode() => this.ID.GetHashCode();
            #endregion
        }

        /// <summary>
        /// Maze grid class
        /// </summary>
        public class Maze : ConsoleView<char>
        {
            public class Node
            {
                #region Properties
                /// <summary>
                /// Distance to reach this node
                /// </summary>
                public int Distance { get; }

                /// <summary>
                /// Position of the node
                /// </summary>
                public Vector2 Position { get; }

                /// <summary>
                /// Children of the node
                /// </summary>
                public List<Node> Children { get; } = new();

                /// <summary>
                /// Total distance from this node to the end
                /// </summary>
                public int DistanceToEnd { get; private set; }
                #endregion

                #region Constructors
                /// <summary>
                /// Creates a new Node from the given position
                /// </summary>
                /// <param name="position">Position of the node</param>
                /// <param name="distance">Distance so far to reach this node,defaults to zero</param>
                public Node(Vector2 position, int distance = 0)
                {
                    this.Position = position;
                    this.Distance = distance;
                }

                /// <summary>
                /// Creates a new Node from the given position, then explores it's children
                /// </summary>
                /// <param name="position">Position of the node</param>
                /// <param name="distance">Distance so far to reach this node</param>
                /// <param name="maze">Maze the node is in</param>
                /// <param name="keys">Currently available keys</param>
                private Node(Vector2 position, int distance, Maze maze, int keys) : this(position, distance) => Explore(maze, keys);
                #endregion

                #region Methods
                /// <summary>
                /// Explores the children of this node
                /// </summary>
                /// <param name="maze">Maze to explore into</param>
                /// <param name="keys">Currently available keys</param>
                public void Explore(Maze maze, int keys = 0)
                {
                    if (this.Distance >= maze.shortestPath)
                    {
                        this.DistanceToEnd = maze.shortestPath;
                        return;
                    }

                    if (keys == maze.allKeys)
                    {
                        //Got all the keys
                        maze.shortestPath = Math.Min(maze.shortestPath, this.Distance);
                    }
                    else if (maze.states.TryGetValue((this.Position, keys), out int toEnd))
                    {
                        this.DistanceToEnd = toEnd;
                        maze.shortestPath = Math.Min(maze.shortestPath, this.Distance + toEnd);
                    }
                    else
                    {
                        //Get distance to key
                        this.DistanceToEnd = int.MaxValue;
                        //Look at every key that can be traveled to
                        foreach (Lock locked in maze.locks.Values.Where(l => l.IsReachable(keys)))
                        {
                            (Vector2, Vector2) travel = (this.Position, locked.Key);
                            if (!maze.distances.TryGetValue(travel, out int distance))
                            {
                                int? path = SearchUtils.GetPathLength(this.Position, locked.Key, v => (v - locked.Key).Length, v => FindNeighbours(v, maze, locked.ID, keys), MinSearchComparer.Comparer, maze.distances);
                                if (!path.HasValue)
                                {
                                    continue;
                                }

                                distance = path.Value;
                            }

                            //Create node further down
                            Node child = new(locked.Key, this.Distance + distance, maze, keys | locked.Bitwise);
                            this.DistanceToEnd = Math.Min(this.DistanceToEnd, child.DistanceToEnd + distance);
                            this.Children.Add(child);
                        }

                        if (!this.Children.IsEmpty())
                        {
                            maze.states.Add((this.Position, keys), this.DistanceToEnd);
                        }
                    }
                }
                #endregion

                #region Static methods
                /// <summary>
                /// Function finding neighbours for a given key within the maze, not walking through other keys
                /// </summary>
                /// <param name="position">Position to look from</param>
                /// <param name="maze">Maze to find the neighbours in</param>
                /// <param name="key">Searched key</param>
                /// <param name="keys">Hashset of currently unlocked keys</param>
                /// <returns>An enumerable of all the neighbours around a given node</returns>
                private static IEnumerable<(Vector2, double)> FindNeighbours(Vector2 position, Maze maze, char key, int keys)
                {
                    //Look through all neighbours
                    foreach (Vector2 neighbour in position.Adjacent().Where(n => maze[n] is not WALL))
                    {
                        char value = maze[neighbour];
                        if (!char.IsLetter(value) || key == value || (keys & (1 << (char.ToLower(value) - 'a'))) is not 0)
                        {
                            //Return neighbours with a distance of 1
                            yield return (neighbour, 1d);
                        }
                    }
                }
                #endregion
            }

            #region Constants
            /// <summary>
            /// Character code for a wall
            /// </summary>
            private const char WALL = '#';
            #endregion
            
            #region Fields
            /// <summary>Locks dictionary</summary>
            private readonly Dictionary<char, Lock> locks = new(26);
            /// <summary>Travel distance dictionary</summary>
            private readonly Dictionary<(Vector2, Vector2), int> distances = new();
            /// <summary>Maze search states</summary>
            private readonly Dictionary<(Vector2, int), int> states = new();
            /// <summary>Shortest path found to reach all the keys</summary>
            private int shortestPath = int.MaxValue;
            /// <summary>Bit mask for all keys</summary>
            private readonly int allKeys = 0;
            #endregion

            #region Properties
            /// <summary>
            /// Starting/root node
            /// </summary>
            public Node Start { get; }
            #endregion

            #region Constructors
            /// <summary>
            /// Creates a new maze with the specified parameters
            /// </summary>
            /// <param name="width">Width of the maze</param>
            /// <param name="height">Height of the maze</param>
            /// <param name="input">Maze data</param>
            /// <param name="start">Starting position</param>
            public Maze(int width, int height, string[] input, Vector2 start) : base(width, height, ToChar, start)
            {
                //Setup the maze
                Populate(input, s => s.ToCharArray());

                //Get all the keys and locks
                foreach (Vector2 pos in Vector2.Enumerate(width, height))
                {
                    char value = this.grid[pos.Y, pos.X];
                    char key = char.ToLower(value);
                    if (key is >= 'a' and <= 'z')
                    {
                        if (!this.locks.TryGetValue(key, out Lock? currentLock))
                        {
                            currentLock = new Lock(key);
                            this.allKeys |= 1 << (key - 'a');
                        }
                        this.locks[key] = char.IsLower(value) ? currentLock with { Key = pos - start } : currentLock with { Door = pos - start };
                    }
                }

                //Get all the key requirements
                Lock[] allLocks = new Lock[this.locks.Count];
                this.locks.Values.CopyTo(allLocks, 0);
                foreach (Lock target in allLocks)
                {
                    int required = 0;
                    Vector2[]? path = SearchUtils.Search(Vector2.Zero, target.Key, v => (v - target.Key).Length, FindNeighbours, MinSearchComparer.Comparer);
                    if (path is not null)
                    {
                        foreach (char door in path.Select(v => this[v]).Where(c => c is >= 'A' and <= 'Z'))
                        {
                            required |= 1 << (door - 'A');
                        }
                    }

                    this.locks[target.ID] = target with { Required = required };
                }

                //Create search graph
                this.Start = new Node(Vector2.Zero);
            }
            #endregion
            
            #region Methods
            /// <summary>
            /// Finds the shortest path through the maze to collect all keys
            /// </summary>
            /// <returns>The length of the shortest path</returns>
            public int FindShortestPath()
            {
                this.Start.Explore(this);
                return this.shortestPath;
            }

            /// <summary>
            /// Function finding neighbours for a given location within the maze
            /// </summary>
            /// <param name="position">Position to look from</param>
            /// <returns>An enumerable of all the neighbours around a given node</returns>
            private IEnumerable<(Vector2, double)> FindNeighbours(Vector2 position)
            {
                //Look through all neighbours
                foreach (Vector2 neighbour in position.Adjacent().Where(n => this[n] is not WALL))
                {
                    //Return neighbours with a distance of 1
                    yield return (neighbour, 1d);
                }
            }
            #endregion

            #region Static methods
            /// <summary>
            /// Converts the maze data to a prettier values
            /// </summary>
            /// <param name="c">Character to convert</param>
            /// <returns>The pretty version of the character</returns>
            private static char ToChar(char c)
            {
                return c switch
                {
                    '#' => '▓',
                    '.' => ' ',
                    '@' => '¤',
                    _   => c
                };
            }
            #endregion
        }
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ConsoleView{T}"/> fails</exception>
        public Day18(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            AoCUtils.LogPart1(this.Data.FindShortestPath());
            AoCUtils.LogPart2("");
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override Maze Convert(string[] rawInput)
        {
            int height = rawInput.Length;
            int width = rawInput[0].Length;
            Vector2 start = Vector2.Enumerate(width, height).First(v => rawInput[v.Y][v.X] is '@');
            return new Maze(rawInput[0].Length, rawInput.Length, rawInput, start);
        }
        #endregion
    }
}
