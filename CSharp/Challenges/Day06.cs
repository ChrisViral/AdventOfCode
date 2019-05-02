using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Tools;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Point coordinate structure
    /// </summary>
    public readonly struct Point
    {
        #region Properties
        /// <summary>
        /// X coordinate
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y Coordinate
        /// </summary>
        public int Y { get; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new point with the given coordinates
        /// </summary>
        /// <param name="x">X coordinate of the point</param>
        /// <param name="y">Y coordinate of the point</param>
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates the Manhattan distance between this point and another point
        /// </summary>
        /// <param name="other">Other point to calculate the distance to</param>
        /// <returns>The Manhattan distance between both points</returns>
        public int Distance(Point other) => Math.Abs(this.X - other.X) + Math.Abs(this.Y - other.Y);
        #endregion

        #region Static methods
        /// <summary>
        /// Parses string data into a new Point
        /// </summary>
        /// <param name="data">Data to parse</param>
        /// <returns>The parsed point</returns>
        public static Point Parse(string[] data) => new Point(int.Parse(data[0]), int.Parse(data[1]));
        #endregion
    }

    /// <summary>
    /// Day 6 challenge
    /// </summary>
    public class Day06 : Challenge
    {
        #region Properties
        /// <summary>
        /// Day ID
        /// </summary>
        public override int ID { get; } = 6;
        #endregion

        #region Methods
        /// <summary>
        /// Challenge solver
        /// </summary>
        public override void Solve()
        {
            Regex pattern = new Regex(@"(\d+), (\d+)", RegexOptions.Compiled);
            Point[] points = GetLines().Select(line => pattern.ParseData(line, Dista)).Select(data => new Point(data[0], data[1]));

            int width = points.Max(p => p.X);
            int height = points.Max(p => p.Y);
            Dictionary<Point, int> counts = points.ToDictionary(p => p, p => 0);
            HashSet<Point> edges = new HashSet<Point>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point pos = new Point(x, y);
                    Point? best = points[0];
                    int shortest = pos.Distance(points[0]);
                    foreach (Point point in points)
                    {
                        int distance = pos.Distance(point);
                        if (distance < shortest)
                        {
                            best = point;
                            shortest = distance;
                        }
                        else if (distance == shortest)
                        {
                            best = null;
                        }
                    }

                    if (best != null)
                    {
                        Point closest = best.Value;
                        if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        {
                            edges.Add(closest);
                            counts[closest] = 0;

                        }
                        else if (!edges.Contains(closest)) { counts[closest]++; }
                    }
                }
            }

            Print("Part one max area: " + counts.Values.Max());

            int safe = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Point pos = new Point(x, y);
                    int total = points.Sum(pos.Distance);
                    if (total < 10000) { safe++; }
                }
            }
            
            Print("Part one safe area: " + safe);
        }
        #endregion
    }
}