using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 02
    /// </summary>
    public class Day02 : ArraySolver<int[]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
        public Day02(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int[][] unsafeReports = this.Data.WhereNot(IsSafe).ToArray();
            int safe = this.Data.Length - unsafeReports.Length;
            AoCUtils.LogPart1(safe);

            int safeDampened = unsafeReports.Count(IsSafeDampened);
            AoCUtils.LogPart2(safe + safeDampened);
        }

        private static bool IsSafe(ICollection<int> report)
        {
            Span<int> diffs = stackalloc int[report.Count - 1];

            int i = 0;
            int previous = report.First();
            foreach (int current in report.Skip(1))
            {
                diffs[i++] = current - previous;
                previous   = current;
            }

            return diffs.All(d => d is >= 1 and <= 3) || diffs.All(d => d is <= -1 and >= -3);
        }

        private static bool IsSafeDampened(int[] report)
        {
            LinkedList<int> linkedReport = new(report);

            linkedReport.RemoveFirst();
            if (IsSafe(linkedReport)) return true;

            linkedReport.AddFirst(report[0]);
            for (LinkedListNode<int> current = linkedReport.First!; current.Next is not null; current = current.Next)
            {
                LinkedListNode<int> removed = current.Next;
                linkedReport.Remove(removed);
                if (IsSafe(linkedReport)) return true;

                linkedReport.AddAfter(current, removed);
            }

            return false;
        }

        /// <inheritdoc cref="ArraySolver{T}.ConvertLine"/>
        protected override int[] ConvertLine(string line) => line.Split(' ').ConvertAll(int.Parse);
        #endregion
    }
}

