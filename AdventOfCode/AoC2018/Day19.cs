using AdventOfCode.AoC2018.ElfCode;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 19
/// </summary>
public sealed class Day19 : ElfCodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int sum = RunVM();
        AoCUtils.LogPart1(sum);

        sum = RunVM(1);
        AoCUtils.LogPart2(sum);
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
