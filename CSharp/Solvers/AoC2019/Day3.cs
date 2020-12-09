using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 3
    /// </summary>
    public class Day3 : Solver<(Vector2[] first, Vector2[] second)>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day3"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1, T2}"/> fails</exception>
        public Day3(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            Dictionary<Vector2, int> firstVisited = GetVisited(this.Data.first);
            Dictionary<Vector2, int> secondVisited = GetVisited(this.Data.second);

            HashSet<Vector2> intersections = new(firstVisited.Keys);
            intersections.IntersectWith(secondVisited.Keys);

            int min = intersections.Min(i => Math.Abs(i.X) + Math.Abs(i.Y));
            AoCUtils.LogPart1(min);

            min = intersections.Min(i => firstVisited[i] + secondVisited[i]);
            AoCUtils.LogPart2(min);
        }

        /// <summary>
        /// Gets the visited positions for a given wire
        /// </summary>
        /// <param name="movements">Sequence of movements made by the wire</param>
        /// <returns>A dictionary containing the visited location as key and steps amount as values</returns>
        private static Dictionary<Vector2, int> GetVisited(IEnumerable<Vector2> movements)
        {
            int steps = 0;
            Vector2 position = Vector2.Zero;
            Dictionary<Vector2, int> visited = new();
            foreach (Vector2 movement in movements)
            {
                Vector2 step = movement / Math.Max(Math.Abs(movement.X), Math.Abs(movement.Y));
                Vector2 target = position + movement;
                do
                {
                    steps++;
                    position += step;
                    visited.TryAdd(position, steps);
                }
                while (position != target);
            }

            return visited;
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override (Vector2[], Vector2[]) Convert(string[] rawInput) => (Array.ConvertAll(rawInput[0].Split(','), Vector2.ParseFromDirection), Array.ConvertAll(rawInput[1].Split(','), Vector2.ParseFromDirection));
        #endregion
    }
}
