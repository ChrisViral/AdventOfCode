using System;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 7
    /// </summary>
    public class Day7 : Solver<string[]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day7"/> Solver from the specified file
        /// </summary>
        /// <param name="file">File to load for puzzle input</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        public Day7(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            AoCUtils.LogPart1("");
            AoCUtils.LogPart2("");
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override string[] Convert(string[] rawInput) => rawInput;
        #endregion
    }
}