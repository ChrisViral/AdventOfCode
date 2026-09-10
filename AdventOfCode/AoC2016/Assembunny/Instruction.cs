using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using JetBrains.Annotations;

namespace AdventOfCode.AoC2016.Assembunny;

public enum Opcode
{
    CPY,
    INC,
    DEC,
    JNZ
}

[InlineArray(4)]
public struct Registers
{
    private int element;
}

public readonly partial record struct Instruction(Opcode Opcode, RegisterRef<int> X, RegisterRef<int> Y)
{
    [GeneratedRegex(@"([a-z]{3}) (-?\d+|[a-z])(?: (-?\d+|[a-z]))?")]
    public static partial Regex Matcher { get; }

    // ReSharper disable once IntroduceOptionalParameters.Global
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public Instruction(Opcode opcode, RegisterRef<int> x) : this(opcode, x, default) { }

    public void Execute(ref int address, ref Registers registers)
    {
        switch (this.Opcode)
        {
            case Opcode.CPY:
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
                    return;
                }
                break;

            default:
                throw this.Opcode.Invalid();
        }

        address++;
    }
}
