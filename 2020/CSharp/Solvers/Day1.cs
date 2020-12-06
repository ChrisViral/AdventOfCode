using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    /// <summary>
    /// Day 1 solver
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
        /// Creates a new Day2 <see cref="Solver"/> with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/> fails</exception>
        public Day1(FileInfo file) : base(file) => this.values = new HashSet<int>(this.Input);
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            FindTwoMatching();
            FindThreeMatching();
        }
        
        ///<inheritdoc cref="Solver{T}"/>
        public override int[] Convert(string[] rawInput) => rawInput.Select(int.Parse).ToArray();

        /// <summary>
        /// First part solving
        /// </summary>
        private void FindTwoMatching()
        {
            foreach (int expense in this.Input)
            {
                int match = TARGET - expense;
                if (this.values.Contains(match))
                {
                    Trace.WriteLine(expense * match);
                    return;
                }
            }
        }

        /// <summary>
        /// Second part solving
        /// </summary>
        private void FindThreeMatching()
        {
            Array.Sort(this.Input);
            for (int i = 0; i < this.Input.Length - 2; /*i++*/)
            {
                int first = this.Input[i];
                foreach (int second in this.Input[++i..^1])
                {
                    int total = first + second;

                    if (total >= TARGET)
                    {
                        break;
                    }

                    int third = TARGET - total;
                    if (this.values.Contains(third))
                    {
                        Trace.WriteLine(first * second * third);
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
