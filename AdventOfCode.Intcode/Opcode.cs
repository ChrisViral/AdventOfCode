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
    /// Input
    /// </summary>
    INP = 3,
    /// <summary>
    /// Output
    /// </summary>
    OUT = 4,
    /// <summary>
    /// Jump not zero
    /// </summary>
    JNZ = 5,
    /// <summary>
    /// Jump equals zero
    /// </summary>
    JEZ = 6,
    /// <summary>
    /// Test lest than
    /// </summary>
    TLT = 7,
    /// <summary>
    /// Test equals
    /// </summary>
    TEQ = 8,
    /// <summary>
    /// Relative base set
    /// </summary>
    REL = 9,
    /// <summary>
    /// Halt
    /// </summary>
    HLT = 99
}
