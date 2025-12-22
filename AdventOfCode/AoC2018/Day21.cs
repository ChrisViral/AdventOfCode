using AdventOfCode.AoC2018.ElfCode;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 21
/// </summary>
public sealed class Day21 : ElfCodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int targetIp = this.Data.Instructions.FindIndex(i => i.Opcode is Opcode.EQRR && (i.A is 0 || i.B is 0));
        Instruction targetInstruction = this.Data.Instructions[targetIp];
        int targetRegister = targetInstruction.A is not 0 ? targetInstruction.A : targetInstruction.B;

        List<long> values = new(11000);
        RunVM(values, targetIp, targetRegister);
        AoCUtils.LogPart1(values[0]);
        AoCUtils.LogPart2(values[^1]);
        AoCUtils.Log(values.Count);
    }

    private void RunVM(List<long> values, long targetIp, int targetRegister)
    {
        Registers registers = new();
        ref long ip = ref registers[this.Data.InstructionPointer];
        while (ip < this.Data.Instructions.Length)
        {
            if (ip == targetIp)
            {
                long value = registers[targetRegister];
                if (values.Contains(value)) return;

                values.Add(value);
            }

            VirtualMachine.RunInstruction(this.Data.Instructions[(int)ip], ref registers);
            ip++;
        }
    }
}
