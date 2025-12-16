using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using FastEnumUtility;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 16
/// </summary>
public sealed partial class Day16 : Solver<(Day16.Sample[] samples, Day16.Instruction[] program)>
{
    public enum Opcode
    {
        ADDR,
        ADDI,
        MULR,
        MULI,
        BANR,
        BANI,
        BORR,
        BORI,
        SETR,
        SETI,
        GTIR,
        GTRI,
        GTRR,
        EQIR,
        EQRI,
        EQRR
    }

    [InlineArray(4)]
    public struct Registers : IEquatable<Registers>
    {
        private int element;

        public Registers(int a, int b, int c, int d)
        {
            this[0] = a;
            this[1] = b;
            this[2] = c;
            this[3] = d;
        }

        public override string ToString() => $"[{this[0]}, {this[1]}, {this[2]}, {this[3]}]";

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(this[0], this[1], this[2], this[3]);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj) => obj is Registers other && Equals(other);

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Registers other) => this == other;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Registers left, in Registers right)
        {
            return left[0] == right[0]
                && left[1] == right[1]
                && left[2] == right[2]
                && left[3] == right[3];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Registers left, in Registers right)
        {
            return left[0] != right[0]
                || left[1] != right[1]
                || left[2] != right[2]
                || left[3] != right[3];
        }
    }

    public readonly record struct Instruction(int OpcodeValue, int A, int B, int C)
    {
        public override string ToString() => $"{this.OpcodeValue} {this.A} {this.B} {this.C}";
    }

    public sealed record Sample(in Registers Before, in Registers After, in Instruction Instruction);

    private const int TRUE  = 1;
    private const int FALSE = 0;
    private const int OPCODE_COUNT = 16;

    [GeneratedRegex(@"\[(\d), (\d), (\d), (\d)\]")]
    private static partial Regex RegistersMatcher { get; }

    [GeneratedRegex(@"(1?\d) (\d) (\d) (\d)")]
    private static partial Regex InstructionMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Keep track of possible opcodes per value
        HashSet<Opcode>[] possibleOpcodes = new HashSet<Opcode>[OPCODE_COUNT];
        possibleOpcodes.Fill(() => [..FastEnum.GetValues<Opcode>()]);

        int threefold = 0;
        foreach (Sample sample in this.Data.samples)
        {
            int possibleCount = 0;
            HashSet<Opcode> possible = possibleOpcodes[sample.Instruction.OpcodeValue];
            foreach (Opcode opcode in FastEnum.GetValues<Opcode>())
            {
                // Run instruction
                Registers sampleRegisters = sample.Before;
                RunInstruction(opcode, sample.Instruction, ref sampleRegisters);
                if (sampleRegisters == sample.After)
                {
                    // Valid, increment
                    possibleCount++;
                }
                else
                {
                    // Invalid, remove from possibilities
                    possible.Remove(opcode);
                }
            }

            // More than three possibilities, increment
            if (possibleCount >= 3)
            {
                threefold++;
            }

        }
        AoCUtils.LogPart1(threefold);

        // Create final opcode map
        Opcode[] opcodeMap = new Opcode[OPCODE_COUNT];
        foreach (int _ in ..opcodeMap.Length)
        {
            foreach (int i in ..possibleOpcodes.Length)
            {
                HashSet<Opcode> possible = possibleOpcodes[i];
                if (possible.Count is 1)
                {
                    // If only one is possible, map it
                    Opcode opcode = possible.First();
                    opcodeMap[i] = opcode;
                    // Then remove it from others possibilities
                    possibleOpcodes.ForEach(p => p.Remove(opcode));
                    break;
                }
            }
        }

        // Run program
        Registers registers = new();
        foreach (Instruction instruction in this.Data.program)
        {
            Opcode opcode = opcodeMap[instruction.OpcodeValue];
            RunInstruction(opcode, instruction, ref registers);
        }
        AoCUtils.LogPart2(registers[0]);
    }

    // ReSharper disable once CognitiveComplexity
    private static void RunInstruction(Opcode opcode, in Instruction instruction, ref Registers registers)
    {
        registers[instruction.C] = opcode switch
        {
            Opcode.ADDR => registers[instruction.A] + registers[instruction.B],
            Opcode.ADDI => registers[instruction.A] + instruction.B,
            Opcode.MULR => registers[instruction.A] * registers[instruction.B],
            Opcode.MULI => registers[instruction.A] * instruction.B,
            Opcode.BANR => registers[instruction.A] & registers[instruction.B],
            Opcode.BANI => registers[instruction.A] & instruction.B,
            Opcode.BORR => registers[instruction.A] | registers[instruction.B],
            Opcode.BORI => registers[instruction.A] | instruction.B,
            Opcode.SETR => registers[instruction.A],
            Opcode.SETI => instruction.A,
            Opcode.GTIR => instruction.A > registers[instruction.B] ? TRUE : FALSE,
            Opcode.GTRI => registers[instruction.A] > instruction.B ? TRUE : FALSE,
            Opcode.GTRR => registers[instruction.A] > registers[instruction.B] ? TRUE : FALSE,
            Opcode.EQIR => instruction.A == registers[instruction.B] ? TRUE : FALSE,
            Opcode.EQRI => registers[instruction.A] == instruction.B ? TRUE : FALSE,
            Opcode.EQRR => registers[instruction.A] == registers[instruction.B] ? TRUE : FALSE,
            _            => throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode))
        };
    }

    /// <inheritdoc />
    protected override (Sample[], Instruction[]) Convert(string[] rawInput)
    {
        RegexFactory<Registers> registersFactory     = new(RegistersMatcher);
        RegexFactory<Instruction> instructionFactory = new(InstructionMatcher);
        List<Sample> samples = new(rawInput.Length / 3);
        int i;
        for (i = 0; rawInput[i][0] is 'B'; i += 3)
        {
            Sample sample = new(registersFactory.ConstructObject(rawInput[i]),
                                registersFactory.ConstructObject(rawInput[i + 2]),
                                instructionFactory.ConstructObject(rawInput[i + 1]));
            samples.Add(sample);
        }

        Instruction[] program = instructionFactory.ConstructObjects(rawInput[i..]);
        return (samples.ToArray(), program);
    }
}
