using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using FastEnumUtility;

using static AdventOfCode.AoC2018.Common.VirtualMachine;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 19
/// </summary>
public sealed partial class Day19 : Solver<(int ip, Instruction[] instructions)>
{
    [GeneratedRegex(@"([a-z]{4}) (\d) (1?\d) (\d)")]
    private static partial Regex InstructionRegex { get; }

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
        ref int ip = ref registers[this.Data.ip];
        registers[0] = input;
        while (ip is not 1)
        {
            RunInstruction(this.Data.instructions[ip], ref registers);
            ip++;
        }

        int sum = 0;
        int max = int.Max(registers);
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

    /// <inheritdoc />
    protected override (int, Instruction[]) Convert(string[] rawInput)
    {
        int ip = int.Parse(rawInput[0].AsSpan(4..));
        ReadOnlySpan<string> instructionsInput = rawInput.AsSpan(1);
        Instruction[] instructions = new Instruction[instructionsInput.Length];
        foreach (int i in ..instructions.Length)
        {
            string line = instructionsInput[i];
            GroupCollection groups = InstructionRegex.Match(line).Groups;
            Opcode opcode = FastEnum.Parse<Opcode>(groups[1].ValueSpan, ignoreCase: true);
            int a = int.Parse(groups[2].ValueSpan);
            int b = int.Parse(groups[3].ValueSpan);
            int c = int.Parse(groups[4].ValueSpan);
            instructions[i] = new Instruction(opcode, a, b, c);
        }
        return (ip, instructions);
    }
}
