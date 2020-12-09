using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020
{
    /// <summary>
    /// Solver for 2020 Day 8
    /// </summary>
    public class Day8 : Solver<Day8.Instruction[]>
    {
        /// <summary>
        /// Instruction operations
        /// </summary>
        public enum Operations
        {
            NOP,
            ACC,
            JMP
        }
        
        /// <summary>
        /// Instruction structure
        /// </summary>
        public record Instruction(Operations Operation, int Value)
        {
            #region Constructors
            /// <summary>
            /// Creates a new instruction from a given line
            /// </summary>
            /// <param name="line">Instruction line</param>
            public Instruction(string line) : this(Enum.Parse<Operations>(line[..3], true), int.Parse(line[4..])) { }
            #endregion

            #region Methods
            /// <summary>
            /// Executes the instruction
            /// </summary>
            /// <param name="accumulator">Current accumulator</param>
            /// <param name="pointer">Current pointer</param>
            /// <exception cref="InvalidEnumArgumentException">If the Operation to execute isn't valid</exception>">
            public void Execute(ref int accumulator, ref int pointer)
            {
                switch (this.Operation)
                {
                    case Operations.NOP:
                        pointer++;
                        break;

                    case Operations.ACC:
                        accumulator += this.Value;
                        pointer++;
                        break;

                    case Operations.JMP:
                        pointer += this.Value;
                        break;

                    default:
                        throw new InvalidEnumArgumentException(nameof(this.Operation), (int)this.Operation, typeof(Operations));
                }
            }
            #endregion
        }

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day8"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Instruction"/> fails</exception>
        public Day8(FileInfo file) : base(file) { }
        #endregion

        #region Fields
        private int accumulator;
        private int pointer;
        private readonly HashSet<int> visited = new();
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver.Run"/>
        public override void Run()
        {
            RunProgram();
            AoCUtils.LogPart1(this.accumulator);

            foreach (int i in ..this.Data.Length)
            {
                this.accumulator = 0;
                this.pointer = 0;
                this.visited.Clear();
                
                if (this.Data[i].Operation is not Operations.ACC && RunProgram(i))
                {
                    AoCUtils.LogPart2(this.accumulator);
                    return;
                }
            }
        }

        /// <summary>
        /// Runs the program and sees if it terminates
        /// </summary>
        /// <param name="change">Instruction ID to change from NOP to JMP or vice-versa. Defaults to -1 (no changes)</param>
        /// <returns>True if the program terminate, false if they loop forever</returns>
        public bool RunProgram(int change = -1)
        {
            while (this.pointer < this.Data.Length)
            {
                if (!this.visited.Add(this.pointer)) return false;

                Instruction instruction = this.Data[this.pointer];
                if (this.pointer == change)
                {
                    Operations newOp = instruction.Operation is Operations.NOP ? Operations.JMP : Operations.NOP;
                    instruction = instruction with { Operation = newOp };
                }

                instruction.Execute(ref this.accumulator, ref this.pointer);
            }

            return true;
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override Instruction[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, s => new Instruction(s));
        #endregion
    }
}
