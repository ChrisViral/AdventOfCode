using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 1
    /// </summary>
    public class Day1 : Solver<int[]>
    {
        #region Constants
        /// <summary>
        /// Target total
        /// </summary>
        private const int TARGET = 2020;
        #endregion

        #region Fields
        private readonly HashSet<int> values;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day1"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/> fails</exception>
        public Day1(FileInfo file) : base(file) => this.values = new HashSet<int>(this.Data);
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            FindTwoMatching();
            FindThreeMatching();
        }
        
        ///<inheritdoc cref="Solver{T}.Convert"/>
        protected override int[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, int.Parse);

        /// <summary>
        /// First part solving
        /// </summary>
        private void FindTwoMatching()
        {
            foreach (int expense in this.Data)
            {
                int match = TARGET - expense;
                if (this.values.Contains(match))
                {
                    AoCUtils.LogPart1(expense * match);
                    return;
                }
            }
        }

        /// <summary>
        /// Second part solving
        /// </summary>
        private void FindThreeMatching()
        {
            Array.Sort(this.Data);
            for (int i = 0; i < this.Data.Length - 2; /*i++*/)
            {
                int first = this.Data[i];
                foreach (int second in this.Data[++i..^1])
                {
                    int total = first + second;

                    if (total >= TARGET)
                    {
                        break;
                    }

                    int third = TARGET - total;
                    if (this.values.Contains(third))
                    {
                        AoCUtils.LogPart2(first * second * third);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}