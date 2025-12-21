using System.ComponentModel;
using System.Runtime.CompilerServices;
using FastEnumUtility;

namespace AdventOfCode.AoC2018.ElfCode;

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

public readonly record struct Instruction(Opcode Opcode, int A, int B, int C)
{
    public override string ToString() => $"{this.Opcode.FastToString().ToLowerInvariant()} {this.A} {this.B} {this.C}";
}

public sealed record Program(int InstructionPointer, Instruction[] Instructions);

[InlineArray(6)]
public struct Registers : IEquatable<Registers>
{
    private long element;

    public Registers(long a, long b, long c, long d)
    {
        this[0] = a;
        this[1] = b;
        this[2] = c;
        this[3] = d;
    }

    public Registers(long a, long b, long c, long d, long e, long f) : this(a, b, c, d)
    {
        this[4] = e;
        this[5] = f;
    }

    public override string ToString() => $"[{this[0]}, {this[1]}, {this[2]}, {this[3]}, {this[4]}, {this[5]}]";

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(this[0], this[1], this[2], this[3], this[4], this[5]);

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
            && left[3] == right[3]
            && left[4] == right[4]
            && left[5] == right[5];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in Registers left, in Registers right)
    {
        return left[0] != right[0]
            || left[1] != right[1]
            || left[2] != right[2]
            || left[3] != right[3]
            || left[4] != right[4]
            || left[5] != right[5];
    }
}


public static class VirtualMachine
{
    private const long TRUE  = 1L;
    private const long FALSE = 0L;

    // ReSharper disable once CognitiveComplexity
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunInstruction(in Instruction instruction, ref Registers registers)
    {
        // ReSharper disable RedundantCast
        registers[instruction.C] = instruction.Opcode switch
        {
            Opcode.ADDR => registers[instruction.A] + registers[instruction.B],
            Opcode.ADDI => registers[instruction.A] + instruction.B,
            Opcode.MULR => registers[instruction.A] * registers[instruction.B],
            Opcode.MULI => registers[instruction.A] * instruction.B,
            Opcode.BANR => registers[instruction.A] & registers[instruction.B],
            Opcode.BANI => registers[instruction.A] & (long)instruction.B,
            Opcode.BORR => registers[instruction.A] | registers[instruction.B],
            Opcode.BORI => registers[instruction.A] | (long)instruction.B,
            Opcode.SETR => registers[instruction.A],
            Opcode.SETI => instruction.A,
            Opcode.GTIR => instruction.A > registers[instruction.B] ? TRUE : FALSE,
            Opcode.GTRI => registers[instruction.A] > instruction.B ? TRUE : FALSE,
            Opcode.GTRR => registers[instruction.A] > registers[instruction.B] ? TRUE : FALSE,
            Opcode.EQIR => instruction.A == registers[instruction.B] ? TRUE : FALSE,
            Opcode.EQRI => registers[instruction.A] == instruction.B ? TRUE : FALSE,
            Opcode.EQRR => registers[instruction.A] == registers[instruction.B] ? TRUE : FALSE,
            _           => throw new InvalidEnumArgumentException(nameof(instruction.Opcode), (int)instruction.Opcode, typeof(Opcode))
        };
        // ReSharper restore RedundantCast
    }
}
