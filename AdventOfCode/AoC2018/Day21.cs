using AdventOfCode.AoC2018.ElfCode;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 21
/// </summary>
[Solver(2018, 21)]
public sealed partial class Day21 : ElfCodeSolver
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int targetIp = this.Data.Instructions.FindIndex(i => i.Opcode is Opcode.EQRR && (i.A is 0 || i.B is 0));
        Instruction targetInstruction = this.Data.Instructions[targetIp];
        int targetRegister = targetInstruction.A is not 0 ? targetInstruction.A : targetInstruction.B;

        List<long> values = new(11000);
        RunVM(values, targetIp, targetRegister);
        LogAnswer(values[0]);
        LogAnswer(values[^1]);
        Log(values.Count);
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
