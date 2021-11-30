using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Intcode;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using Vector2 = AdventOfCode.Grids.Vectors.Vector2;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 15
/// </summary>
public class Day15 : IntcodeSolver
{
    /// <summary>
    /// Droid status codes
    /// </summary>
    public enum Status
    {
        UNKNOWN = -1,
        WALL    = 0,
        EMPTY   = 1,
        OXYGEN  = 2,
        DROID   = 3,
        START   = 4,
        PATH    = 5
    }

    /// <summary>
    /// Repair droid
    /// </summary>
    public class Droid
    {
        #region Fields
        private readonly IntcodeVM program;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Droid with the specified program
        /// </summary>
        /// <param name="program">Program on which the droid runs</param>
        public Droid(IntcodeVM program) => this.program = program;
        #endregion

        #region Methods
        /// <summary>
        /// Moves the robot in the specified direction and gets the new status code
        /// </summary>
        /// <param name="direction">Direction to move in</param>
        /// <returns>The status from the resulting movement</returns>
        public Status Move(Directions direction)
        {
            this.program.AddInput((int)direction);
            this.program.Run();
            return (Status)this.program.GetNextOutput();
        }
        #endregion
    }

    /// <summary>
    /// Maze view
    /// </summary>
    public class Maze : ConsoleView<Status>
    {
        #region Constants
        /// <summary>
        /// Status to char converter
        /// </summary>
        private static readonly Dictionary<Status, char> toChar = new(7)
        {
            [Status.UNKNOWN] = '░',
            [Status.WALL]    = '▓',
            [Status.EMPTY]   = ' ',
            [Status.OXYGEN]  = 'O',
            [Status.DROID]   = '¤',
            [Status.START]   = 'X',
            [Status.PATH]    = '.'
        };
        #endregion
            
        #region Properties
        /// <summary>
        /// Current position of the droid in the grid
        /// </summary>
        public Vector2 DroidPosition { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Maze of the given height and width
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        public Maze(int width, int height) : base(width, height, s => toChar[s], new Vector2((int)Math.Ceiling(width / 2d), (int)Math.Ceiling(height / 2d)), Status.UNKNOWN)
        {
            //Set starting position
            this[Vector2.Zero] = Status.START;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Finds the shortest path from the origin to the target
        /// </summary>
        /// <param name="target">Target to get to</param>
        /// <returns>The length of the shortest path, or -1 if no path is found</returns>
        public int FindShortestPath(Vector2 target)
        {
            //Get path
            Vector2[]? path = SearchUtils.Search(Vector2.Zero, target, v => (v - target).Length, FindNeighbours, MinSearchComparer.Comparer);
            //If valid
            if (path?.Length > 0)
            {
                //Print out the path
                this.DroidPosition = path[0];
                PrintToConsole();
                foreach (Vector2 v in path[1..]!)
                {
                    this[this.DroidPosition] = Status.PATH;
                    this.DroidPosition = v;
                    PrintToConsole();
                }

                //Return the path length;
                return path.Length;
            }

            //no path found, return -1
            return -1;
        }

        /// <summary>
        /// Fills the maze with oxygen from a given starting point
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <returns>The cycles taken to fill out the entire maze</returns>
        public int FillFromPosition(Vector2 start)
        {
            //Positions to fill
            HashSet<Vector2> toFill = new(start.Adjacent().Where(v => this[v] is not Status.WALL and not Status.OXYGEN));
            HashSet<Vector2> fillNext = new();
            int cycles = 0;
            //Keep filling until none left
            while (toFill.Count is not 0)
            {
                //Check all vectors to fill
                foreach (Vector2 filling in toFill)
                {
                    //Set it to oxygen and add it's adjacent to next
                    this[filling] = Status.OXYGEN;
                    foreach (Vector2 next in filling.Adjacent().Where(v => this[v] is not Status.WALL and not Status.OXYGEN))
                    {
                        fillNext.Add(next);
                    }
                }

                //Switch over
                AoCUtils.Swap(ref toFill, ref fillNext);
                fillNext.Clear();
                PrintToConsole();
                cycles++;
            }

            //Return the amount of cycles taken
            return cycles;
        }

        /// <summary>
        /// Function finding neighbours for a given location within the maze
        /// </summary>
        /// <param name="position">Position to look from</param>
        /// <returns>An enumerable of all the neighbours around a given node</returns>
        private IEnumerable<(Vector2, double)> FindNeighbours(Vector2 position)
        {
            //Look through all neighbours
            foreach (Vector2 neighbour in position.Adjacent().Where(n => this[n] is not Status.WALL))
            {
                //Return neighbours with a distance of 1
                yield return (neighbour, 1d);
            }
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString()
        {
            Status temp = this[this.DroidPosition];
            this[this.DroidPosition] = Status.DROID;
            string toString = base.ToString();
            this[this.DroidPosition] = temp;
            return toString;
        }
        #endregion
    }

    #region Fields
    private readonly Droid droid;
    private readonly Maze maze;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day15(string input) : base(input)
    {
        this.droid = new Droid(this.VM);
        this.maze = new Maze(41, 41); //Figured out size by visualization
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        //Hide cursor while running
        Console.CursorVisible = false;
        this.maze.PrintToConsole();
        //Explored set
        Vector2 position = Vector2.Zero;
        HashSet<Vector2> explored = new() { position };
        //Pathing
        Stack<Directions> path = new();
        path.Push(Directions.NONE);
        //Target
        Vector2 oxygenPosition = Vector2.Zero;
        while (true)
        {
            //Get direction and movement flag
            Directions direction = path.Peek();
            bool moved = false;
            //Check all directions
            foreach (Directions dir in DirectionsUtils.AllDirections)
            {
                //Position in given direction
                Vector2 newPosition = position.Move(dir);
                //If not explored
                if (explored.Add(newPosition))
                {
                    //Set status in maze
                    Status report = this.droid.Move(dir);
                    this.maze[newPosition] = report;
                    if (report is not Status.WALL)
                    {
                        path.Push(dir);
                        position += dir;
                        if (report is Status.OXYGEN)
                        {
                            oxygenPosition = position;
                        }
                        moved = true;
                        break;
                    }
                }
            }

            if (!moved)
            {
                //Done
                if (direction is Directions.NONE) break;
                //Backtrack
                direction = direction.Invert();
                path.Pop();
                this.droid.Move(direction);
                position += direction;
            }
            //Move droid
            this.maze.DroidPosition = position;
            this.maze.PrintToConsole();
        }
            
        //First part answer
        AoCUtils.LogPart1(this.maze.FindShortestPath(oxygenPosition));
            
        //Adjust cursor
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        //Get Cycles
        int cycles = this.maze.FillFromPosition(oxygenPosition);
        //Adjust again
        Console.SetCursorPosition(0, Console.CursorTop + 1);
        //Second part answer
        AoCUtils.LogPart2(cycles);
        //Show cursor again
        Console.CursorVisible = true;
    }
    #endregion
}