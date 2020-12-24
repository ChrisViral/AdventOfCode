using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 18
    /// </summary>
    public class Day18 : Solver<ConsoleView<char>>
    {
        /// <summary>
        /// Lock record
        /// </summary>
        public record Lock(char ID, Vector2 Key, Vector2 Door, HashSet<Lock> Required)
        {
            public override int GetHashCode() => this.ID.GetHashCode();
        }
        
        /// <summary>
        /// Maze grid class
        /// </summary>
        public class Maze : ConsoleView<char>
        {
            #region Constants
            /// <summary>
            /// Character code for a wall
            /// </summary>
            private const char WALL = '#';
            #endregion
            
            #region Fields
            /// <summary>
            /// Locks dictionary
            /// </summary>
            private readonly Dictionary<char, Lock> locks = new(26);
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
                            currentLock = new Lock(key, Vector2.Zero, Vector2.Zero, new HashSet<Lock>());
                        }
                        this.locks[key] = char.IsLower(value) ? currentLock with { Key = pos - start } : currentLock with { Door = pos - start };
                    }
                }

                //Get all the key requirements
                foreach ((char _, Vector2 key, Vector2 _, HashSet<Lock> required) in this.locks.Values)
                {
                    Vector2[]? path = SearchUtils.Search(Vector2.Zero, key, v => (v - key).Length, FindNeighbours, MinSearchComparer.Comparer);
                    if (path is not null)
                    {
                        foreach (char door in path.Select(v => this[v]).Where(c => c is >= 'A' and <= 'Z'))
                        {
                            required.Add(this.locks[char.ToLower(door)]);
                        }
                    }
                }
            }
            #endregion
            
            #region Methods
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
            AoCUtils.LogPart1("");
            AoCUtils.LogPart2("");
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override ConsoleView<char> Convert(string[] rawInput)
        {
            int height = rawInput.Length;
            int width = rawInput[0].Length;
            Vector2 start = Vector2.Enumerate(width, height).First(v => rawInput[v.Y][v.X] is '@');
            return  new Maze(rawInput[0].Length, rawInput.Length, rawInput, start);
        }
        #endregion
    }
}
