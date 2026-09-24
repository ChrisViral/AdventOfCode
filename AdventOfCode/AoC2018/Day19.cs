using AdventOfCode.AoC2018.ElfCode;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Numbers;
using Challenge.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 19
/// </summary>
[Solver(2018, 19)]
public sealed class Day19 : ElfCodeSolver
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int sum = RunVM();
        LogAnswer(sum);

        sum = RunVM(1);
        LogAnswer(sum);
    }

    private int RunVM(int input = 0)
    {
        Registers registers = new();
        ref long ip = ref registers[this.Data.InstructionPointer];
        registers[0] = input;
        while (ip is not 1)
        {
            VirtualMachine.RunInstruction(this.Data.Instructions[(int)ip], ref registers);
            ip++;
        }

        int sum = 0;
        int max = (int)long.Max(registers);
        foreach (int i in 1..(max / 2))
        {
            (int q, int r) = Math.DivRem(max, i);
            if (r is 0)
            {
                sum += i + q;
            }
        }
        return sum;
    }
}
