using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 12
/// </summary>
public sealed partial class Day12 : RegexSolver<Day12.Instruction>
{
    public enum Opcode
    {
        CPY,
        INC,
        DEC,
        JNZ
    }

    [InlineArray(4)]
    private struct Registers
    {
        private int element;
    }

    public readonly record struct Instruction(Opcode Opcode, RegisterRef<int> X, RegisterRef<int> Y)
    {
        // ReSharper disable once IntroduceOptionalParameters.Global
        public Instruction(Opcode opcode, RegisterRef<int> x) : this(opcode, x, default) { }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"([a-z]{3}) (-?\d+|[a-z])(?: (-?\d+|[a-z]))?")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int address = 0;
        Registers registers = new();
        while (address >= 0 && address < this.Data.Length)
        {
            ExecuteInstruction(this.Data[address], ref address, ref registers);
        }
        AoCUtils.LogPart1(registers[0]);

        address = 0;
        registers = new Registers();
        registers[2] = 1;
        while (address >= 0 && address < this.Data.Length)
        {
            ExecuteInstruction(this.Data[address], ref address, ref registers);
        }
        AoCUtils.LogPart2(registers[0]);
    }

    private static void ExecuteInstruction(in Instruction instruction, ref int address, ref Registers registers)
    {
        switch (instruction.Opcode)
        {
            case Opcode.CPY:
                instruction.Y.GetRegister(registers) = instruction.X.GetValue(registers);
                break;

            case Opcode.INC:
                instruction.X.GetRegister(registers)++;
                break;

            case Opcode.DEC:
                instruction.X.GetRegister(registers)--;
                break;

            case Opcode.JNZ:
                if (instruction.X.GetValue(registers) is not 0)
                {
                    address += instruction.Y.GetValue(registers);
                    return;
                }
                break;

            default:
                throw instruction.Opcode.Invalid();
        }

        address++;
    }
}
