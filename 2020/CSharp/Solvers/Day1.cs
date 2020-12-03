#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    public class Day1 : Solver<int>
    {
        #region Constructors
        public Day1(FileInfo file) : base(file) => Array.Sort(this.Input);
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
            int length = this.Input.Length;
            for (int i = 0; i < length - 1; i++)
            {
                int a = this.Input[i];
                for (int j = i + 1; j < length; j--)
                {
                    int b = this.Input[j];
                    int total = a + b;
                    if (total is 2020)
                    {
                        Trace.WriteLine(a * b);
                        return;
                    }

                    if (total < 2020)
                    {
                        break;
                    }
                }
            }
        }

        private void FindThreeMatching()
        {
            int length = this.Input.Length;
            for (int i = 0; i < length - 2; i++)
            {
                int a = this.Input[i];
                for (int j = i + 1; j < length - 1; j++)
                {
                    int b = this.Input[j];
                    int ab = + a + b;

                    if (ab > 2020)
                    {
                        break;
                    }

                    for (int k = j + 1; k < length; k++)
                    {
                        int c = this.Input[k];
                        int total = ab + c;
                        if (total is 2020)
                        {
                            Trace.WriteLine(a * b * c);
                            return;
                        }

                        if (total > 2020)
                        {
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
