using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Rectangular area definer
    /// </summary>
    public readonly struct Rect
    {
        #region Properties
        /// <summary>
        /// X position (top left corner)
        /// </summary>
        public int X { get; }
        /// <summary>
        /// Y position (top left corner)
        /// </summary>
        public int Y { get; }
        /// <summary>
        /// Rect width
        /// </summary>
        public int W { get; }
        /// <summary>
        /// Rect height
        /// </summary>
        public int H { get; }
        /// <summary>
        /// Max X value (X + W)
        /// </summary>
        public int MaxX { get; }
        /// <summary>
        /// Max Y value (Y + H)
        /// </summary>
        public int MaxY { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new rect with the specified parameters
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Rect(int x, int y, int w, int h)
        {
            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
            this.MaxX = x + w;
            this.MaxY = y + h;
        }
        #endregion

    }

    public class Day3 : Challenge
    {
        #region Properties
        /// <summary>
        /// Day ID
        /// </summary>
        public override int ID { get; } = 3;
        #endregion

        #region Methods
        /// <summary>
        /// Challenge solver
        /// </summary>
        public override void Solve()
        {
            Regex pattern = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)", RegexOptions.Compiled);
            Dictionary<int, Rect> claims = new Dictionary<int, Rect>();
            
            int[,] fabric = new int[1000, 1000];
            int overlaps = 0;

            foreach (string line in GetLines())
            {
                int[] data = pattern.Match(line).Groups.Cast<Group>().Skip(1).Select(c => int.Parse(c.Value)).ToArray();
                Rect rect = new Rect(data[1], data[2], data[3], data[4]);
                claims.Add(data[0], rect);
                for (int i = rect.X; i < rect.MaxX; i++)
                {
                    for (int j = rect.Y; j < rect.MaxY; j++)
                    {
                        if (++fabric[i, j] == 2)
                        {
                            overlaps++;
                        }
                    }
                }
            }
            
            Print("Part one count: " + overlaps);

            foreach (KeyValuePair<int, Rect> claim in claims)
            {
                Rect rect = claim.Value;
                bool correct = true;
                for (int i = rect.X; i < rect.MaxX; i++)
                {
                    for (int j = rect.Y; j < rect.MaxY; j++)
                    {
                        if (fabric[i, j] > 1)
                        {
                            correct = false;
                            break;
                        }
                    }
                }

                if (correct)
                {
                    Print("Part two ID: " + claim.Key);
                    return;
                }
            }
        }
        #endregion
    }
}
