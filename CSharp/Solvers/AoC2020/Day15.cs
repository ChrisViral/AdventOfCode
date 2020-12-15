using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 15
    /// </summary>
    public class Day15 : Solver<Dictionary<int, int>>
    {
        #region Constants
        /// <summary>
        /// First target to hit
        /// </summary>
        private const int FIRST_TARGET = 2020;
        /// <summary>
        /// Second target to hit
        /// </summary>
        private const int SECOND_TARGET = 30000000;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Dictionary{TKey,TValue}"/> fails</exception>
        public Day15(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            //Setup
            int last = 0;
            bool wasFirst = true;
            int turn = this.Data.Values.Max();
            Dictionary<int, int> previous = new();
            
            //Part 1
            GetToTarget(ref turn, ref wasFirst, ref last, FIRST_TARGET, previous);
            AoCUtils.LogPart1(last);
            
            //Part 2 (takes a couple seconds but who cares)
            GetToTarget(ref turn, ref wasFirst, ref last, SECOND_TARGET, previous);
            AoCUtils.LogPart2(last);
        }

        /// <summary>
        /// Finds the next number spoken up to a target amount of turns
        /// </summary>
        /// <param name="turn">Current turn</param>
        /// <param name="wasFirst">If the last number was a first time hearing</param>
        /// <param name="last">Last number spoken</param>
        /// <param name="target">Target amount of turns</param>
        /// <param name="previous">Dictionary of previous turns a number was spoken</param>
        private void GetToTarget(ref int turn, ref bool wasFirst, ref int last, int target, IDictionary<int, int> previous)
        {
            while (turn++ != target)
            {
                if (wasFirst)
                {
                    last = 0;
                    wasFirst = false;
                    previous[0] = this.Data[0];
                    this.Data[0] = turn;
                }
                else
                {
                    last = this.Data[last] - previous[last];
                    if (!this.Data.ContainsKey(last))
                    {
                        this.Data.Add(last, turn);
                        wasFirst = true;
                    }
                    else
                    {
                        previous[last] = this.Data[last];
                        this.Data[last] = turn;
                    }
                }
            }
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override Dictionary<int, int> Convert(string[] rawInput) => rawInput[0].Split(',')
                                                                                         .Select((n, i) => (int.Parse(n), i + 1))
                                                                                         .ToDictionary(t => t.Item1, t => t.Item2);
        #endregion
    }
}
