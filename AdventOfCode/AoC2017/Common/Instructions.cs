using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2017.Common;

/// <summary>
/// Duet opcodes
/// </summary>
public enum Opcode
{
    SND,
    SET,
    ADD,
    SUB,
    MUL,
    MOD,
    RCV,
    JGZ,
    JNZ
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
/// Duet instruction
/// </summary>
/// <param name="Opcode">Instruction opcode</param>
/// <param name="X">First operand</param>
/// <param name="Y">Second operant</param>
public readonly partial record struct Instruction(Opcode Opcode, RegisterRef<long> X, RegisterRef<long> Y)
{
    /// <summary>
    /// Instruction matching regex
    /// </summary>
    [GeneratedRegex(@"([a-z]{3}) ([a-z]|-?\d+)(?: ([a-z]|-?\d+))?")]
    public static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new single operand instruction
    /// </summary>
    /// <param name="opcode">Instruction opcode</param>
    /// <param name="x">First operand</param>
    /// ReSharper disable once IntroduceOptionalParameters.Global
    public Instruction(Opcode opcode, RegisterRef<long> x) : this(opcode, x, default) { }
}
