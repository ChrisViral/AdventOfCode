using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
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
            foreach (int[] perm in AoCUtils.Permutations(new[] { 0, 1, 2, 3, 4 }))
            {
                foreach (int i in ..5)
                {
                    this.Data[i].WriteToInput(perm[i]);
                }
                this.Data[0].WriteToInput(0);
                
                foreach (IntcodeVM vm in this.Data)
                {
                    vm.ResetInput();
                    vm.Run();
                }

                max = Math.Max(max, this.Data[4].GetOutput()[0]);
                
                foreach (IntcodeVM vm in this.Data)
                {
                    vm.Reset();
                }
            }
            AoCUtils.LogPart1(max);
            
            max = int.MinValue;
            this.Data[0].In = this.Data[4].Out;
            foreach (int[] perm in AoCUtils.Permutations(new[] { 5, 6, 7, 8, 9 }))
            {
                foreach (int i in ..5)
                {
                    this.Data[i].WriteToInput(perm[i]);
                }
                this.Data[0].WriteToInput(0);
                
                while (!this.Data[4].IsHalted)
                {
                    foreach (IntcodeVM vm in this.Data)
                    {
                        vm.ResetInput();
                        vm.Run();
                        vm.ClearInput();
                    }
                }
                
                max = Math.Max(max, this.Data[4].GetOutput()[0]);
                foreach (IntcodeVM vm in this.Data)
                {
                    vm.Reset();
                }
            }
            AoCUtils.LogPart1(max);
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        public override IntcodeVM[] Convert(string[] rawInput)
        {
            string line = rawInput[0];
            IntcodeVM[] vms = new IntcodeVM[5];
            foreach (int i in ..5)
            {
                IntcodeVM vm = new(line);
                vms[i] = vm;
                if (i > 0)
                {
                    vm.In = vms[i - 1].Out;
                }
            }

            //Link up streams
            foreach (int i in 1..5)
            {
                vms[i].In = vms[i - 1].Out;
            }

            return vms;
        }
        #endregion
    }
}
