using FastEnumUtility;

namespace AdventOfCode.Intcode;

/// <summary>
/// Intcode Opcodes
/// </summary>
public enum Opcode
{
    /// <summary>
    /// No operation
    /// </summary>
    NOP = 0,
    /// <summary>
    /// Add
    /// </summary>
    ADD = 1,
    /// <summary>
    /// Multiply
    /// </summary>
    MUL = 2,
    /// <summary>
    /// Halt
    /// </summary>
    HLT = 99
}

/// <summary>
/// Intcode enum booster
/// </summary>
[FastEnum<Opcode>]
public sealed partial class OpcodeBooster;