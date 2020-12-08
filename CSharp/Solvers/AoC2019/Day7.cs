using System;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Intcode;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019
{
    /// <summary>
    /// Solver for 2019 Day 7
    /// </summary>
    public class Day7 : Solver<IntcodeVM[]>
    {
        #region Constants
        /// <summary>
        /// Amount of amplifiers running
        /// </summary>
        private const int AMPS = 5;
        /// <summary>
        /// Part 1 phase settings
        /// </summary>
        private static readonly int[] part1Phase = { 0, 1, 2, 3, 4 };
        /// <summary>
        /// Part 2 phase settings
        /// </summary>
        private static readonly int[] part2Phase = { 5, 6, 7, 8, 9 };
        #endregion
        
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day7"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
        public Day7(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            int max = int.MinValue;
            //Go through all permutations of part 1 settings
            foreach (int[] perm in AoCUtils.Permutations(part1Phase))
            {
                //Add phase settings
                foreach (int i in ..5)
                {
                    this.Data[i].AddInput(perm[i]);
                }
                //Add input value
                this.Data[0].AddInput(0);
                
                //Run all amplifiers
                foreach (IntcodeVM amp in this.Data)
                {
                    amp.Run();
                }

                //Get value from last amplifier
                max = Math.Max(max, this.Data[^1].GetNextOutput());
                
                //Reset amplifiers
                foreach (IntcodeVM amp in this.Data)
                {
                    amp.Reset();
                }
            }
            AoCUtils.LogPart1(max);
            
            //Set last output as first input
            this.Data[0].In = this.Data[^1].Out;
            max = int.MinValue;
            //Go through all permutations of part 2 settings
            foreach (int[] perm in AoCUtils.Permutations(part2Phase))
            {
                //Add phase settings
                foreach (int i in ..AMPS)
                {
                    this.Data[i].AddInput(perm[i]);
                }
                //Add input value
                this.Data[0].AddInput(0);
                
                //Run until the last amp has halted
                while (!this.Data[^1].IsHalted)
                {
                    //Run all amps
                    foreach (IntcodeVM amp in this.Data)
                    {
                        amp.Run();
                    }
                }
                
                //Get value from last amplifier
                max = Math.Max(max, this.Data[^1].GetNextOutput());
                
                //Reset amplifiers
                foreach (IntcodeVM vm in this.Data)
                {
                    vm.Reset();
                }
            }
            AoCUtils.LogPart2(max);
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        public override IntcodeVM[] Convert(string[] rawInput)
        {
            string line = rawInput[0];
            IntcodeVM[] vms = new IntcodeVM[AMPS];
            foreach (int i in ..AMPS)
            {
                IntcodeVM vm = new(line);
                vms[i] = vm;
                if (i > 0)
                {
                    vm.In = vms[i - 1].Out;
                }
            }

            return vms;
        }
        #endregion
    }
}
