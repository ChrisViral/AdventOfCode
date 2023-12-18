using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Collections;
using AdventOfCode.Intcode;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 15
/// </summary>
public class Day15 : IntcodeSolver
{
    /// <summary>
    /// Droid status codes
    /// </summary>
    private enum Status
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
    private class Droid
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
        public Status Move(Direction direction)
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
    private class Maze : ConsoleView<Status>
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
        public Vector2<int> DroidPosition { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Maze of the given height and width
        /// </summary>
        /// <param name="width">Width of the maze</param>
        /// <param name="height">Height of the maze</param>
        public Maze(int width, int height) : base(width, height, s => toChar[s], new Vector2<int>((int)Math.Ceiling(width / 2d), (int)Math.Ceiling(height / 2d)), Status.UNKNOWN)
        {
            //Set starting position
            this[Vector2<int>.Zero] = Status.START;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Finds the shortest path from the origin to the target
        /// </summary>
        /// <param name="target">Target to get to</param>
        /// <returns>The length of the shortest path, or -1 if no path is found</returns>
        public int FindShortestPath(Vector2<int> target)
        {
            //Get path
            Vector2<int>[]? path = SearchUtils.Search(Vector2<int>.Zero, target, v => (v - target).Length, FindNeighbours, MinSearchComparer.Comparer);
            //No path found, return -1
            if (path is null || path.Length <= 0) return -1;

            //Print out the path
            this.DroidPosition = path[0];
            PrintToConsole();
            foreach (Vector2<int> v in path[1..])
            {
                this[this.DroidPosition] = Status.PATH;
                this.DroidPosition = v;
                PrintToConsole();
            }

            //Return the path length;
            return path.Length;
        }

        /// <summary>
        /// Fills the maze with oxygen from a given starting point
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <returns>The cycles taken to fill out the entire maze</returns>
        public int FillFromPosition(Vector2<int> start)
        {
            //Positions to fill
            HashSet<Vector2<int>> toFill = new(start.Adjacent().Where(v => this[v] is not Status.WALL and not Status.OXYGEN));
            HashSet<Vector2<int>> fillNext = new();
            int cycles = 0;
            //Keep filling until none left
            while (toFill.Count is not 0)
            {
                //Check all vectors to fill
                foreach (Vector2<int> filling in toFill)
                {
                    //Set it to oxygen and add it's adjacent to next
                    this[filling] = Status.OXYGEN;
                    foreach (Vector2<int> next in filling.Adjacent().Where(v => this[v] is not Status.WALL and not Status.OXYGEN))
                    {
                        fillNext.Add(next);
                    }
                }

                //Switch over
                (toFill, fillNext) = (fillNext, toFill);
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
        private IEnumerable<(Vector2<int>, double)> FindNeighbours(Vector2<int> position)
        {
            //Look through all neighbours
            return position.Adjacent().Where(n => this[n] is not Status.WALL).Select(neighbour => (neighbour, 1d));
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
        this.droid = new(this.VM);
        this.maze = new(41, 41); //Figured out size by visualization
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
        Vector2<int> position = Vector2<int>.Zero;
        HashSet<Vector2<int>> explored = new() { position };
        //Pathing
        Stack<Direction> path = new();
        path.Push(Direction.NONE);
        //Target
        Vector2<int> oxygenPosition = Vector2<int>.Zero;
        while (true)
        {
            //Get direction and movement flag
            Direction direction = path.Peek();
            bool moved = false;
            //Check all directions
            foreach (Direction dir in DirectionsUtils.AllDirections)
            {
                //Position in given direction
                Vector2<int> newPosition = position.Move(dir);
                //If not explored
                if (!explored.Add(newPosition)) continue;

                //Set status in maze
                Status report = this.droid.Move(dir);
                this.maze[newPosition] = report;
                if (report is Status.WALL) continue;

                path.Push(dir);
                position += dir;
                if (report is Status.OXYGEN)
                {
                    oxygenPosition = position;
                }
                moved = true;
                break;
            }

            if (!moved)
            {
                //Done
                if (direction is Direction.NONE) break;
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