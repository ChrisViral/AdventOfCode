using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2020 Day 10
    /// </summary>
    public class Day10 : GridSolver<bool>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="GridSolver{T}"/> fails</exception>
        public Day10(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            HashSet<Vector2> asteroids = new();
            foreach (int j in ..this.Grid.Height)
            {
                foreach (int i in ..this.Grid.Width)
                {
                    if (this.Grid[i, j])
                    {
                        asteroids.Add(new Vector2(i, j));
                    }
                }
            }
            
            Vector2 station = Vector2.Zero;
            Dictionary<Vector2, int> visible = new();
            foreach (Vector2 asteroid in asteroids)
            {
                Dictionary<Vector2, int> targets = new();
                foreach (Vector2 target in asteroids)
                {
                    if (target == asteroid) continue;

                    Vector2 direction = (target - asteroid).Reduced();
                    if (targets.ContainsKey(direction))
                    {
                        targets[direction]++;
                    }
                    else
                    {
                        targets.Add(direction, 1);
                    }
                }

                if (targets.Count > visible.Count)
                {
                    station = asteroid;
                    visible = targets;
                }
            }
            AoCUtils.LogPart1(visible.Count);

            Vector2 lastDirection = Vector2.Up;
            Vector2 lastPosition = Vaporize(visible, station, lastDirection);
            int totalVaporized = 1;
            
            while (totalVaporized is not 200)
            {
                Angle bestAngle = Angle.FullCircle;
                Vector2 closestDirection = Vector2.Zero;
                foreach (Vector2 direction in visible.Keys)
                {
                    if (direction == lastDirection) continue;
                    
                    Angle angle = Vector2.Angle(lastDirection, direction).Circular;
                    if (angle < bestAngle)
                    {
                        bestAngle = angle;
                        closestDirection = direction;
                    }
                }
                lastDirection = closestDirection;
                lastPosition = Vaporize(visible, station, closestDirection);
                totalVaporized++;
            }
            AoCUtils.LogPart2((lastPosition.X * 100) + lastPosition.Y);
        }

        /// <summary>
        /// Vaporizes the next asteroid in the specified direction
        /// </summary>
        /// <param name="visible">Dictionary of visible asteroids in the angles at which they are found</param>
        /// <param name="station">Position of the station</param>
        /// <param name="direction">Direction in which to vaporize</param>
        /// <returns>The position of the vaporized asteroid</returns>
        private Vector2 Vaporize(IDictionary<Vector2, int> visible, in Vector2 station, in Vector2 direction)
        {
            if (visible[direction] is 1)
            {
                visible.Remove(direction);
            }
            else
            {
                visible[direction]--;
            }
            Vector2 position = station + direction;
            while (!this.Grid[position])
            {
                position += direction;
            }

            this.Grid[position] = false;
            return position;
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();
        #endregion
    }
}
