using System.Text.RegularExpressions;
using AdventOfCode.AoC2017.Common;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Enums;
using Challenge.Utils.Extensions.Numbers;
using FastEnumUtility;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 23
/// </summary>
[Solver(2017, 23)]
public sealed class Day23 : RegexSolver<Instruction>
{
    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int multiplications = 0;
        int address = 0;
        Registers registers = new();
        do
        {
            ref Instruction instruction = ref this.Data[address];
            RunInstruction(instruction, ref registers, ref address);
            if (instruction.Opcode is Opcode.MUL)
            {
                multiplications++;
            }
        }
        while (address >= 0 && address < this.Data.Length);
        LogAnswer(multiplications);

        // Extract registers index for lower and upper bound
        int lowerBoundIndex = (int)this.Data[0].X.Value;
        int upperBoundIndex = (int)this.Data[1].X.Value;

        // Calculate jump address for end of setup
        long setupEnd = this.Data[3].Y.Value + 3;

        // Run program until we finish setup
        address = 0;
        registers = new Registers();
        registers[0] = 1L;
        do
        {
            RunInstruction(this.Data[address], ref registers, ref address);
        }
        while (address != setupEnd);

        // Calculate result in a non-idiotic way
        int notPrime = 0;
        int lowerBound = (int)registers[lowerBoundIndex];
        int upperBound = (int)registers[upperBoundIndex];
        int step = (int)-this.Data[^2].Y.Value;
        for (int b = lowerBound; b <= upperBound; b += step)
        {
            if (!b.IsPrime())
            {
                notPrime++;
            }
        }
        LogAnswer(notPrime);
    }

    private static void RunInstruction(in Instruction instruction, ref Registers registers, ref int address)
    {
        switch (instruction.Opcode)
        {
            case Opcode.SET:
                instruction.X.GetRegister(registers) = instruction.Y.GetValue(registers);
                address++;
                return;

            case Opcode.SUB:
                instruction.X.GetRegister(registers) -= instruction.Y.GetValue(registers);
                address++;
                return;

            case Opcode.MUL:
                instruction.X.GetRegister(registers) *= instruction.Y.GetValue(registers);
                address++;
                return;

            case Opcode.JNZ:
                address += instruction.X.GetValue(registers) is not 0L ? (int)instruction.Y.GetValue(registers) : 1;
                return;

            case Opcode.SND:
            case Opcode.ADD:
            case Opcode.MOD:
            case Opcode.RCV:
            case Opcode.JGZ:
                throw new InvalidOperationException($"{instruction.Opcode.FastToString()} opcode not currently defined");

            default:
                throw instruction.Opcode.Invalid();
        }
    }
}
