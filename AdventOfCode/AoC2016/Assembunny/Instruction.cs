using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;

namespace AdventOfCode.AoC2016.Assembunny;

/// <summary>
/// Assembunny opcode
/// </summary>
public enum Opcode
{
    CPY,
    INC,
    DEC,
    JNZ,
    TGL,
    OUT
}

/// <summary>
/// Assembunny registers
/// </summary>
[InlineArray(4)]
public struct Registers
{
    private int element;
}

/// <summary>
/// Assembunny instruction
/// </summary>
/// <param name="Opcode">Instruction opcode</param>
/// <param name="X">First instruction argument</param>
/// <param name="Y">Second instruction argument</param>
public readonly partial record struct Instruction(Opcode Opcode, RegisterRef<int> X, RegisterRef<int> Y)
{
    /// <summary>
    /// Assembunny matching regex
    /// </summary>
    [GeneratedRegex(@"([a-z]{3}) (-?\d+|[a-z])(?: (-?\d+|[a-z]))?")]
    public static partial Regex Matcher { get; }

    /// <summary>
    /// Assembunny instruction
    /// </summary>
    /// <param name="opcode">Instruction opcode</param>
    /// <param name="x">First instruction argument</param>
    /// ReSharper disable once IntroduceOptionalParameters.Global
    public Instruction(Opcode opcode, RegisterRef<int> x) : this(opcode, x, default) { }

    /// <summary>
    /// Executes this Assembunny instruction
    /// </summary>
    /// <param name="address">Current instruction address</param>
    /// <param name="registers">Current registers</param>
    /// <param name="instructions">Instruction list, required to run <see cref="Opcode.TGL"/> instructions</param>
    /// <exception cref="InvalidEnumArgumentException">If <see cref="Opcode"/> is invalid/></exception>
    /// <returns>The instructio output, if any</returns>
    /// ReSharper disable once CognitiveComplexity
    public int? Execute(ref int address, ref Registers registers, Span<Instruction> instructions = default)
    {
        int? output = null;
        switch (this.Opcode)
        {
            case Opcode.CPY when this.Y.IsRegister:
                this.Y.GetRegister(registers) = this.X.GetValue(registers);
                break;

            case Opcode.INC:
                this.X.GetRegister(registers)++;
                break;

            case Opcode.DEC:
                this.X.GetRegister(registers)--;
                break;

            case Opcode.JNZ:
                if (this.X.GetValue(registers) is not 0)
                {
                    address += this.Y.GetValue(registers);
                    return output;
                }
                break;

            case Opcode.TGL when !instructions.IsEmpty:
                int targetIndex = address + this.X.GetRegister(registers);
                if (targetIndex < 0 || targetIndex >= instructions.Length) break;

                ref Instruction target = ref instructions[targetIndex];
                Opcode newOpcode = target.Y.IsSet
                                       ? target.Opcode is Opcode.JNZ ? Opcode.CPY : Opcode.JNZ
                                       : target.Opcode is Opcode.INC ? Opcode.DEC : Opcode.INC;
                target = target with { Opcode = newOpcode };
                break;

            case Opcode.OUT:
                output = this.X.GetValue(registers);
                break;

            default:
                throw this.Opcode.Invalid();
        }

        address++;
        return output;
    }
}
