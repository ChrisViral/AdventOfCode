using System;
using System.Collections.Generic;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 8
    /// </summary>
    public class Day8 : Solver<string[]>
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day8"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="T"/> fails</exception>
        public Day8(FileInfo file) : base(file) { }
        #endregion

        private int accumulator;
        private int pointer;

        #region Methods
        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            TestTerminates();
            AoCUtils.LogPart1(this.accumulator);

            for (int i = 0; i < this.Data.Length; i++)
            {
                this.accumulator = 0;
                this.pointer = 0;
                string instruction = this.Data[i];
                if (instruction[..3] is "nop" or "jmp" && TestTerminates(i))
                {
                    break;
                }
            }
            AoCUtils.LogPart2(this.accumulator);
        }

        public bool TestTerminates(int change = -1)
        {
            HashSet<int> visited = new();
            while (this.pointer < this.Data.Length)
            {
                if (!visited.Add(this.pointer))
                {
                    return false;
                }
                
                string[] splits = this.Data[this.pointer].Split(' ');
                string inst = splits[0];
                int value = int.Parse(splits[1]);

                if (this.pointer == change)
                {
                    inst = inst is "nop" ? "jmp" : "nop";
                }

                switch (inst)
                {
                    case "nop":
                        this.pointer++;
                        break;
                    case "acc":
                        this.accumulator += value;
                        this.pointer++;
                        break;
                    case "jmp":
                        this.pointer += value;
                        break;
                }
            }

            return true;
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override string[] Convert(string[] rawInput) => rawInput;
        #endregion
    }
}
