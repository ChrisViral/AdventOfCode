using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 01
    /// </summary>
    public class Day01 : Solver<(int[] leftList, int[] rightList)>
    {
        private const string PATTERN = @"(\d+)   (\d+)";

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day01(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            this.Data.leftList.Sort();
            this.Data.rightList.Sort();
            int distance = this.Data.leftList.Zip(this.Data.rightList).Sum(d => Math.Abs(d.First - d.Second));
            AoCUtils.LogPart1(distance);

            Counter<int> left = new(this.Data.leftList);
            Counter<int> right = new(this.Data.rightList);
            long similarity = left.Sum<int>(v => v * left[v] * right.GetValueOrDefault(v));
            AoCUtils.LogPart2(similarity);
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override (int[] leftList, int[] rightList) Convert(string[] rawInput)
        {
            (int left, int right)[] listData = RegexFactory<(int, int)>.ConstructObjects(PATTERN, rawInput, RegexOptions.Compiled);
            (int[] leftList, int[] rightList) = (new int[listData.Length], new int[listData.Length]);
            foreach (int i in ..listData.Length)
            {
                leftList[i]  = listData[i].left;
                rightList[i] = listData[i].right;
            }
            return (leftList, rightList);
        }
        #endregion
    }
}

