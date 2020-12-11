using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 5
    /// </summary>
    public class Day05 : Solver<Day05.BoardingPass[]>
    {
        /// <summary>
        /// BoardingPass info
        /// </summary>
        public class BoardingPass
        {
            #region Constants
            public const int MAX_ROW = 127;
            public const int MAX_COLUMN = 7;
            #endregion
            
            #region Properties
            public int Row { get; }
            
            public int Column { get; }
            
            public int Id { get; }
            #endregion

            #region Constructors
            public BoardingPass(string pattern)
            {
                this.Row = BinarySearch(pattern[..7], 'F', MAX_ROW);
                this.Column = BinarySearch(pattern[7..], 'L', MAX_COLUMN);
                this.Id = (this.Row * 8) + this.Column;
            }
            #endregion

            #region Static methods
            private static int BinarySearch(string pattern, char trueChar, int max)
            {
                int l = 0, r = max;
                int middle = max / 2;

                foreach (char c in pattern)
                {
                    if (c == trueChar)
                    {
                        r = middle;
                    }
                    else
                    {
                        l = middle + 1;
                    }

                    middle = l + ((r - l) / 2);
                }

                return middle;
            }
            #endregion
        }

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="BoardingPass"/> fails</exception>
        public Day05(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            //Part 1
            int max = 0;
            HashSet<int> existing = new();
            bool[,] seats = new bool[BoardingPass.MAX_ROW + 1, BoardingPass.MAX_COLUMN + 1];
            foreach (BoardingPass pass in this.Data)
            {
                max = Math.Max(max, pass.Id);
                existing.Add(pass.Id);
                seats[pass.Row, pass.Column] = true;
            }
            AoCUtils.LogPart1(max);
            
            //Part 2
            for (int row = 0; row <= BoardingPass.MAX_ROW; row++)
            {
                int rowId = row * 8;
                for (int col = 0; col <= BoardingPass.MAX_COLUMN; col++)
                {
                    if (!seats[row, col])
                    {
                        int id = rowId + col;
                        if (existing.Contains(id + 1) && existing.Contains(id - 1))
                        {
                            AoCUtils.LogPart2(id);
                            return;
                        }
                    }
                }
            }
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override BoardingPass[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, s => new BoardingPass(s));
        #endregion
    }
}