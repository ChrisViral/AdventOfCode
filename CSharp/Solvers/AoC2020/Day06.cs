using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 6
    /// </summary>
    public class Day06 : Solver<HashSet<char>[][]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="HashSet{T}"/>[] fails</exception>
        public Day06(FileInfo file) : base(file, options: StringSplitOptions.TrimEntries) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int anyTotal = 0;
            int allTotal = 0;
            foreach (HashSet<char>[] group in this.Data)
            {
                HashSet<char> anyAnswered = group[0];
                HashSet<char> allAnswered = new(anyAnswered);
                foreach (HashSet<char> answers in group[1..])
                {
                    //Part 1
                    anyAnswered.UnionWith(answers);
                    //Part 2
                    allAnswered.IntersectWith(answers);
                }

                anyTotal += anyAnswered.Count;
                allTotal += allAnswered.Count;
            }
            
            AoCUtils.LogPart1(anyTotal);
            AoCUtils.LogPart2(allTotal);
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override HashSet<char>[][] Convert(string[] rawInput) => AoCUtils.CombineLines(rawInput)
                                                                                   .Select(l => l.Select(s => new HashSet<char>(s)).ToArray())
                                                                                   .ToArray();
        #endregion
    }
}