using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Strings;

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
public readonly partial record struct Instruction(Opcode Opcode, Ref X, Ref Y)
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
    public Instruction(Opcode opcode, Ref x) : this(opcode, x, default) { }
}
