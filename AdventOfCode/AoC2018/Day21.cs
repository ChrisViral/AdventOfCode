using AdventOfCode.AoC2018.ElfCode;
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
        RunVM();
        AoCUtils.LogPart1("");
        AoCUtils.LogPart2("");
    }

    private void RunVM(int input = 0)
    {
        Registers registers = new();
        ref int ip = ref registers[this.Data.InstructionPointer];
        registers[0] = input;
        while (ip < this.Data.Instructions.Length)
        {
            ref Instruction instruction = ref this.Data.Instructions[ip];
            VirtualMachine.RunInstruction(instruction, ref registers);
            ip++;
        }
    }
}
