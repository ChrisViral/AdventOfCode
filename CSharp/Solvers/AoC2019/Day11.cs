using System;
using System.Collections.Generic;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 11
/// </summary>
public class Day11 : IntcodeSolver
{
    /// <summary>
    /// Hull colours
    /// </summary>
    private enum Colour
    {
        BLACK = 0,
        WHITE = 1
    }

    /// <summary>
    /// Painter robot
    /// </summary>
    private class PainterRobot
    {
        #region Fields
        private Vector2<int> position;
        private Vector2<int> direction = Vector2<int>.Up;
        private Grid<Colour> hull;
        private readonly IntcodeVM brain;
        private readonly HashSet<Vector2<int>> painted = new();
        #endregion

        #region Properties
        /// <summary>
        /// Amount of painted hull sections
        /// </summary>
        public int PaintedCount => this.painted.Count;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new painter robot on the specified hull
        /// </summary>
        /// <param name="hullWidth">Hull width</param>
        /// <param name="hullHeight">Hull height</param>
        /// <param name="brain">Robot brain</param>
        public PainterRobot(int hullWidth, int hullHeight, IntcodeVM brain)
        {
            this.brain = brain;
            this.hull = new(hullWidth, hullHeight, i => i is Colour.WHITE ? "#" : ".");
            this.position = new(hullWidth / 2, hullHeight / 2);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Paints the hull
        /// </summary>
        /// <param name="startingColour">The starting panel colour of the painter robot</param>
        /// <returns>The painted result</returns>
        public string Paint(Colour startingColour = Colour.BLACK)
        {
            this.hull[this.position] = startingColour;
            while (!this.brain.IsHalted)
            {
                //Give input and run
                this.brain.AddInput((int)this.hull[this.position]);
                this.brain.Run();
                //Paint output
                this.hull[this.position] = (Colour)this.brain.GetNextOutput();
                this.painted.Add(this.position);
                //Rotate and move
                this.direction = Vector2<int>.Rotate(this.direction, this.brain.GetNextOutput() is 0L ? -90 : 90);
                this.position += this.direction;
            }

            //Return painted result
            return this.hull.ToString();
        }

        /// <summary>
        /// Resets the robot
        /// </summary>
        public void Reset()
        {
            this.brain.Reset();
            this.position = new(this.hull.Width / 2, this.hull.Height / 2);
            this.direction = Vector2<int>.Up;
            this.hull = new(this.hull.Width, this.hull.Height, i => i is Colour.WHITE ? "#" : ".");
            this.painted.Clear();
        }
        #endregion
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day11(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        PainterRobot robot = new(150, 150, this.VM);
        robot.Paint();
        AoCUtils.LogPart1(robot.PaintedCount);

        robot.Reset();
        string registration = "\n" + robot.Paint(Colour.WHITE);
        AoCUtils.LogPart2(registration);
    }
    #endregion
}