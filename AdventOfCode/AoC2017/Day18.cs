using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.AoC2017.Common;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Strings;
using FastEnumUtility;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 18
/// </summary>
public sealed class Day18 : RegexSolver<Instruction>
{
    /// <summary>
    /// Program state
    /// </summary>
    private enum State
    {
        IDLE,
        RUNNING,
        BLOCKED,
        HALTED
    }

    /// <summary>
    /// Duet program
    /// </summary>
    [DebuggerDisplay("Program {id}")]
    public sealed class Program
    {
        public readonly int id;
        private readonly ImmutableArray<Instruction> instructions;
        private readonly Queue<long> receiveQueue = new();
        private int address;
        private long lastSound;
        private Registers registers;
        private State state;

        /// <summary>
        /// Program send recipient
        /// </summary>
        public Program Recipient { get; set; } = null!;

        /// <summary>
        /// Total send calls
        /// </summary>
        public int Sends { get; private set; }

        /// <summary>
        /// Creates a new program with the specified instructions and ID
        /// </summary>
        /// <param name="instructions">Program instructions</param>
        /// <param name="id">Program ID</param>
        public Program(Instruction[] instructions, int id)
        {
            this.id = id;
            this.instructions = [..instructions];
            this.registers['p'.AsIndex] = id;
        }

        /// <summary>
        /// Run the program in normal mode
        /// </summary>
        /// <returns>The first recovered sound value</returns>
        public long RunProgram()
        {
            while (this.address >= 0 && this.address < this.instructions.Length)
            {
                long? recovered = RunInstruction(this.instructions[this.address]);
                if (recovered.HasValue) return recovered.Value;
            }

            return long.MinValue;
        }

        /// <summary>
        /// Runs an normal mode instruction
        /// </summary>
        /// <param name="instruction">Instruction to run</param>
        /// <returns>The recovered sound value, if any</returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">For unknown opcodes</exception>
        private long? RunInstruction(in Instruction instruction)
        {
            switch (instruction.Opcode)
            {
                case Opcode.SND:
                    this.lastSound = instruction.X.GetValue(this.registers);
                    this.address++;
                    return null;

                case Opcode.SET:
                    instruction.X.GetRegister(ref this.registers) = instruction.Y.GetValue(this.registers);
                    this.address++;
                    return null;

                case Opcode.ADD:
                    instruction.X.GetRegister(ref this.registers) += instruction.Y.GetValue(this.registers);
                    this.address++;
                    return null;

                case Opcode.MUL:
                    instruction.X.GetRegister(ref this.registers) *= instruction.Y.GetValue(this.registers);
                    this.address++;
                    return null;

                case Opcode.MOD:
                    instruction.X.GetRegister(ref this.registers) %= instruction.Y.GetValue(this.registers);
                    this.address++;
                    return null;

                case Opcode.RCV:
                    this.address++;
                    if (instruction.X.GetValue(this.registers) is not 0L)
                    {
                        return this.lastSound;
                    }

                    return null;

                case Opcode.JGZ:
                    this.address += instruction.X.GetValue(this.registers) > 0L ? (int)instruction.Y.GetValue(this.registers) : 1;
                    return null;

                case Opcode.SUB:
                case Opcode.JNZ:
                    throw new InvalidOperationException($"{instruction.Opcode.FastToString()} opcode not currently defined");

                default:
                    throw instruction.Opcode.Invalid();
            }
        }

        /// <summary>
        /// Run the program in parallel mode
        /// </summary>
        public void RunProgramParallel()
        {
            if (this.state == State.HALTED) return;

            this.state = State.RUNNING;
            while (this.address >= 0 && this.address < this.instructions.Length)
            {
                RunInstructionParallel(this.instructions[this.address]);
                if (this.state is State.BLOCKED) return;
            }

            this.state = State.HALTED;
        }

        /// <summary>
        /// Runs a parallel mode instruction
        /// </summary>
        /// <param name="instruction">Instruction to run</param>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">For unknown opcodes</exception>
        private void RunInstructionParallel(in Instruction instruction)
        {
            switch (instruction.Opcode)
            {
                case Opcode.SND:
                    this.Recipient.receiveQueue.Enqueue(instruction.X.GetValue(this.registers));
                    this.Sends++;
                    this.address++;
                    return;

                case Opcode.SET:
                    instruction.X.GetRegister(ref this.registers) = instruction.Y.GetValue(this.registers);
                    this.address++;
                    return;

                case Opcode.ADD:
                    instruction.X.GetRegister(ref this.registers) += instruction.Y.GetValue(this.registers);
                    this.address++;
                    return;

                case Opcode.MUL:
                    instruction.X.GetRegister(ref this.registers) *= instruction.Y.GetValue(this.registers);
                    this.address++;
                    return;

                case Opcode.MOD:
                    instruction.X.GetRegister(ref this.registers) %= instruction.Y.GetValue(this.registers);
                    this.address++;
                    return;

                case Opcode.RCV:
                    if (this.receiveQueue.TryDequeue(out long received))
                    {
                        instruction.X.GetRegister(ref this.registers) = received;
                        this.address++;
                        return;
                    }

                    this.state = State.BLOCKED;
                    return;

                case Opcode.JGZ:
                    this.address += instruction.X.GetValue(this.registers) > 0L ? (int)instruction.Y.GetValue(this.registers) : 1;
                    return;

                case Opcode.JNZ:
                    throw new InvalidOperationException("JNZ opcode not currently defined");

                default:
                    throw instruction.Opcode.Invalid();
            }
        }

        /// <summary>
        /// Resets the program to it's base state
        /// </summary>
        public void Reset()
        {
            this.address = 0;
            this.lastSound = 0;
            this.registers = new Registers();
            this.registers['p'.AsIndex] = this.id;
            this.state = State.IDLE;
            this.Sends = 0;
        }
    }

    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Program a = new(this.Data, 0);
        long recovered = a.RunProgram();
        AoCUtils.LogPart1(recovered);

        a.Reset();
        Program b = new(this.Data, 1);
        a.Recipient = b;
        b.Recipient = a;

        int aLastSend;
        int bLastSend;
        do
        {
            aLastSend = a.Sends;
            bLastSend = b.Sends;
            a.RunProgramParallel();
            b.RunProgramParallel();
        }
        while (a.Sends != aLastSend || b.Sends != bLastSend);

        AoCUtils.LogPart2(b.Sends);

    }
}
