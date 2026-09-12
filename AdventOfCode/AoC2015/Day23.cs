using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;
using AdventOfCode.Utils.Extensions.Numbers;
using FastEnumUtility;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 23
/// </summary>
public sealed partial class Day23 : RegexSolver<Day23.Instruction>
{
    public enum Opcode
    {
        HLF,
        TPL,
        INC,
        JMP,
        JIE,
        JIO
    }

    [InlineArray(2)]
    public struct Registers
    {
        private int element;
    }

    public readonly record struct Instruction(Opcode Opcode, RegisterRef<int> Value, int Offset)
    {
        // ReSharper disable once IntroduceOptionalParameters.Global
        public Instruction(Opcode opcode, RegisterRef<int> value) : this(opcode, value, 0) { }

        public void ExecuteInstruction(ref int address, ref Registers registers)
        {
            switch (this.Opcode)
            {
                case Opcode.HLF:
                    ref int register = ref this.Value.GetRegister(registers);
                    register /= 2;
                    break;

                case Opcode.TPL:
                    register = ref this.Value.GetRegister(registers);
                    register *= 3;
                    break;

                case Opcode.INC:
                    register = ref this.Value.GetRegister(registers);
                    register++;
                    break;

                case Opcode.JMP:
                    address += this.Value.GetValue(registers);
                    return;

                case Opcode.JIE when this.Value.GetValue(registers).IsEven:
                case Opcode.JIO when this.Value.GetValue(registers) is 1:
                    address += this.Offset;
                    return;

                case Opcode.JIE:
                case Opcode.JIO:
                    break;

                default:
                    throw this.Opcode.Invalid();
            }

            address++;
        }

        /// <inheritdoc />
        public override string ToString() => this.Opcode is not Opcode.JIO and not Opcode.JIE
                                                 ? $"{this.Opcode.FastToString().ToLowerInvariant()} {this.Value}"
                                                 : $"{this.Opcode.FastToString().ToLowerInvariant()} {this.Value}, {this.Offset}";
    }

    /// <inheritdoc />
    [GeneratedRegex(@"([a-z]{3}) (a|b|[\-+]\d+)(?:, ([\-+]\d+))?")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int address = 0;
        Registers registers = new();
        while (address >= 0 && address < this.Data.Length)
        {
            Instruction instruction = this.Data[address];
            instruction.ExecuteInstruction(ref address, ref registers);
        }
        AoCUtils.LogPart1(registers[1]);

        registers = new Registers();
        registers[0] = 1;
        address = 0;
        while (address >= 0 && address < this.Data.Length)
        {
            Instruction instruction = this.Data[address];
            instruction.ExecuteInstruction(ref address, ref registers);
        }
        AoCUtils.LogPart2(registers[1]);
    }
}
