using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    public class Day5 : Solver<Day5.BoardingPass[]>
    {
        public class BoardingPass
        {
            public const int MAX_ROW = 127;
            public const int MAX_COLUMN = 7;
            
            public int Row { get; }
            public int Column { get; }
            public int Id { get; }

            public BoardingPass(string pattern)
            {
                int l = 0, r = MAX_ROW;
                int middle = MAX_ROW / 2;
                foreach (char c in pattern[..7])
                {
                    if (c is 'F')
                    {
                        r = middle;
                    }
                    else
                    {
                        l = middle + 1;
                    }

                    middle = l + ((r - l) / 2);
                }
                this.Row = middle;

                l = 0;
                r = MAX_COLUMN;
                middle = MAX_COLUMN / 2;
                foreach (char c in pattern[7..])
                {
                    if (c is 'L')
                    {
                        r = middle;
                    }
                    else
                    {
                        l = middle + 1;
                    }

                    middle = l + ((r - l) / 2);
                }
                this.Column = middle;

                this.Id = (this.Row * 8) + this.Column;
            }

        }

        /// <summary>
        /// Creates a new <see cref="Day5"/> with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <typeparamref name="T"/> fails</exception>
        public Day5(FileInfo file) : base(file) { }

        /// <summary>
        /// Runs the solver on the problem input
        /// </summary>
        public override void Run()
        {
            int max = 0;
            HashSet<int> existing = new();
            bool[,] seats = new bool[BoardingPass.MAX_ROW + 1, BoardingPass.MAX_COLUMN + 1];
            foreach (BoardingPass pass in this.Input)
            {
                max = Math.Max(max, pass.Id);
                existing.Add(pass.Id);
                seats[pass.Row, pass.Column] = true;
            }

            //Part 1
            Trace.WriteLine(max);
            
            for (int row = 0; row <= BoardingPass.MAX_ROW; row++)
            {
                for (int col = 0; col <= BoardingPass.MAX_COLUMN; col++)
                {
                    if (!seats[row, col])
                    {
                        int id = (row * 8) + col;
                        if (existing.Contains(id + 1) && existing.Contains(id - 1))
                        {
                            //Part 2
                            Trace.WriteLine(id);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Input conversion function<br/>
        /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
        /// </summary>
        /// <param name="rawInput">Input value</param>
        /// <returns>Target converted value</returns>
        public override BoardingPass[] Convert(string[] rawInput) => rawInput.Select(s => new BoardingPass(s)).ToArray();
    }
}
