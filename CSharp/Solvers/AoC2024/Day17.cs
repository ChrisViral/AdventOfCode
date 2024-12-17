using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024
{
    /// <summary>
    /// Solver for 2024 Day 17
    /// </summary>
    public class Day17 : Solver<(long a, long b, long c, int[] program)>
    {
        private enum Opcode
        {
            ADV = 0,    // A <- A / 2^Op
            BXL = 1,    // B <- B Xor Lit
            BST = 2,    // B <- Op Mod 8
            JNZ = 3,    // A not 0 => Jump Lit
            BXC = 4,    // B <- B Xor C
            OUT = 5,    // B Mod 8 -> Out
            BDV = 6,    // B <- A / 2^Op
            CDV = 7     // C <- A / 2^Op
        }

        private long registerA;
        private long registerB;
        private long registerC;
        private int ip;
        private int[] code = null!;
        private readonly List<int> output = new(16);

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="input">Puzzle input</param>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day17(string input) : base(input) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Base.Solver.Run"/>
        public override void Run()
        {
            // Run with initial params
            this.code = this.Data.program;
            RunProgram(this.Data.a, this.Data.b, this.Data.c);
            AoCUtils.LogPart1(string.Join(',', this.output));

            // Search from 0
            long minInitialA = long.MaxValue;
            Queue<(int chunkSize, long initialA)> search = new();
            search.Enqueue((1, 0L));
            while (search.TryDequeue(out (int chunkSize, long initialA) test))
            {
                // Try matching only last few values from output
                Index startIndex = ^test.chunkSize;
                ReadOnlySpan<int> programChunk = this.code.AsSpan(startIndex);
                // Test eight options from current initial A value
                foreach (int offset in ..8)
                {
                    // Run program with specified parameters
                    long initialA = test.initialA + offset;
                    RunProgram(initialA, this.Data.b, this.Data.c);
                    if (this.output.Count < test.chunkSize) continue;

                    // Check if the last output chunk matches
                    ReadOnlySpan<int> outputChunk = CollectionsMarshal.AsSpan(this.output)[startIndex..];
                    if (!programChunk.SequenceEqual(outputChunk)) continue;

                    if (test.chunkSize == this.code.Length)
                    {
                        // If we have the same total output size, we have a candidate
                        minInitialA = Math.Min(minInitialA, initialA);
                    }
                    else
                    {
                        // Else increase chunk size and add to search queue
                        search.Enqueue((test.chunkSize + 1, initialA * 8L));
                    }

                }
            }
            AoCUtils.LogPart2(minInitialA);
        }

        private void RunProgram(long a, long b, long c)
        {
            this.registerA = a;
            this.registerB = b;
            this.registerC = c;
            this.ip        = 0;
            this.output.Clear();
            int eod = this.code.Length;
            while (this.ip < eod)
            {
                Opcode opcode = (Opcode)this.code[this.ip++];
                switch (opcode)
                {
                    case Opcode.ADV:
                        Div(out this.registerA);
                        break;

                    case Opcode.BXL:
                        Xor(this.code[this.ip++]);
                        break;

                    case Opcode.BST:
                        this.registerB = GetComboOperand() % 8;
                        break;

                    case Opcode.JNZ when this.registerA is not 0:
                        this.ip = this.code[this.ip];
                        break;

                    case Opcode.JNZ:
                        this.ip++;
                        break;

                    case Opcode.BXC:
                        this.ip++;
                        Xor(this.registerC);
                        break;

                    case Opcode.OUT:
                        this.output.Add((int)(GetComboOperand() % 8));
                        break;

                    case Opcode.BDV:
                        Div(out this.registerB);
                        break;

                    case Opcode.CDV:
                        Div(out this.registerC);
                        break;

                    default:
                        throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode));
                }
            }
        }

        private void Div(out long register) => register = this.registerA / (1L << (int)GetComboOperand());

        private void Xor(long value) => this.registerB ^= value;

        private long GetComboOperand()
        {
            long operand = this.code[this.ip++];
            return operand switch
            {
                0 or 1 or 2 or 3 => operand,
                4                => this.registerA,
                5                => this.registerB,
                6                => this.registerC,
                7                => throw new InvalidOperationException("Combo operator 7 is not supported"),
                _                => throw new InvalidOperationException("Invalid combo operator")
            };
        }

        /// <inheritdoc cref="Solver{T}.Convert"/>
        protected override (long, long, long, int[]) Convert(string[] rawInput)
        {
            // Registers
            long a = long.Parse(rawInput[0].AsSpan(12));
            long b = long.Parse(rawInput[1].AsSpan(12));
            long c = long.Parse(rawInput[2].AsSpan(12));

            // Bytecode
            ReadOnlySpan<char> programSpan = rawInput[3].AsSpan(9);
            int[] program = new int[(programSpan.Length + 1) / 2];
            foreach (int i in ..program.Length)
            {
                program[i] = programSpan[i * 2] - '0';
            }
            return (a, b, c, program);
        }
        #endregion
    }
}

