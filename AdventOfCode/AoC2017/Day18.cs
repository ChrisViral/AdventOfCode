using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Strings;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 18
/// </summary>
public sealed partial class Day18 : RegexSolver<Day18.Instruction>
{
    /// <summary>
    /// Duet opcodes
    /// </summary>
    public enum Opcode
    {
        SND,
        SET,
        ADD,
        MUL,
        MOD,
        RCV,
        JGZ
    }

    /// <summary>
    /// Program state
    /// </summary>
    public enum State
    {
        IDLE,
        RUNNING,
        BLOCKED,
        HALTED
    }

    /// <summary>
    /// Duet program registers
    /// </summary>
    [InlineArray(StringUtils.LETTER_COUNT)]
    public struct Registers
    {
        private long element;
    }

    /// <summary>
    /// Instruction value reference
    /// </summary>
    /// <param name="Value">Integer value</param>
    /// <param name="IsRegister">Wether or not the value points to a register or is an immediate value</param>
    public readonly record struct Ref(int Value, bool IsRegister) : IParsable<Ref>
    {
        /// <summary>
        /// Gets the value for this reference
        /// </summary>
        /// <param name="registers">Program registers</param>
        /// <returns>The correct value this reference points to</returns>
        public long GetValue(in Registers registers) => this.IsRegister
                                                            ? registers[this.Value]
                                                            : this.Value;

        /// <summary>
        /// Gets the register value as a ref for this reference
        /// </summary>
        /// <param name="registers">Program registers</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If this reference is not pointing to registers</exception>
        public ref long GetRegister(ref Registers registers)
        {
            if (!this.IsRegister) throw new InvalidOperationException("Cannot get register value for non-register ref");

            return ref registers[this.Value];
        }

        /// <inheritdoc />
        public static Ref Parse(string s, IFormatProvider? provider) => int.TryParse(s, out int value)
                                                                            ? new Ref(value, false)
                                                                            : new Ref(s[0].AsIndex, true);

        /// <inheritdoc />
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Ref result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            result = Parse(s, provider);
            return false;
        }

        /// <inheritdoc />
        public override string ToString() => this.IsRegister
                                                 ? this.Value.AsAsciiLower.ToString()
                                                 : this.Value.ToString();
    }

    /// <summary>
    /// Duet instruction
    /// </summary>
    /// <param name="Opcode">Instruction opcode</param>
    /// <param name="X">First operand</param>
    /// <param name="Y">Second operant</param>
    public readonly record struct Instruction(Opcode Opcode, Ref X, Ref Y)
    {
        /// <summary>
        /// Creates a new single operand instruction
        /// </summary>
        /// <param name="opcode">Instruction opcode</param>
        /// <param name="x">First operand</param>
        /// ReSharper disable once IntroduceOptionalParameters.Global
        public Instruction(Opcode opcode, Ref x) : this(opcode, x, default) { }
    }

    /// <summary>
    /// Duet program
    /// </summary>
    [DebuggerDisplay("Program {id}")]
    public class Program
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
    [GeneratedRegex(@"([a-z]{3}) ([a-z]|-?\d+)(?: ([a-z]|-?\d+))?")]
    protected override partial Regex Matcher { get; }

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
