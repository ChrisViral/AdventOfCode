using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 10
    /// </summary>
    public class Day10 : Solver<int[]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="T"/> fails</exception>
        public Day10(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            Array.Sort(this.Data);

            //To make this simpler lets just store the results in an array, easier to code than vars, less waste than a dict
            int[] counts = new int[4];
            counts[3]++;
            counts[this.Data[0]]++;
            
            for (int i = 0; i < this.Data.Length - 1; /*i++*/)
            {
                counts[-this.Data[i++] + this.Data[i]]++;
            }
            
            AoCUtils.LogPart1(counts[1] * counts[3]);

            //Store how many ways the goal can be reached from each node
            long[] ways = new long[this.Data.Length];
            ways[^1] = 1L; //Seed the array, there's only one way to the goal from the last
            ways[^2] = 1L; //And one way from the second to last, the last node since the goal is more than three away
            for (int i = this.Data.Length - 3; i >= 0; i--)
            {
                int number = this.Data[i];
                //Ways to reach the goal from a node is the sum of the ways from the reachable nodes
                for (int j = i + 1; j < this.Data.Length && this.Data[j] - number <= 3; j++)
                {
                    ways[i] += ways[j];
                }
            }
            
            //Count the ways from the reachable nodes from zero
            long total = 0L;
            for (int i = 0; this.Data[i] <= 3; i++)
            {
                total += ways[i];
            }
            
            AoCUtils.LogPart2(total);
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override int[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, int.Parse);
        #endregion
    }
}