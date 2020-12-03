#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    public class Day1 : Solver<int>
    {
        #region Constants
        private const int TARGET = 2020;
        #endregion

        #region Fields
        private readonly HashSet<int> values;
        #endregion

        #region Constructors
        public Day1(FileInfo file) : base(file) => this.values = new HashSet<int>(this.Input);
        #endregion

        #region Methods
        public override int Convert(string s) => int.Parse(s);

        public override void Run()
        {
            FindTwoMatching();

            FindThreeMatching();
        }

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

        private void FindThreeMatching()
        {
            Array.Sort(this.Input);
            for (int i = 0; i < this.Input.Length - 2; /*i++*/)
            {
                int first = this.Input[i++];
                foreach (int second in this.Input[i..^1])
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
