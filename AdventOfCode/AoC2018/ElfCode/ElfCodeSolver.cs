using System.Text.RegularExpressions;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Solvers;
using FastEnumUtility;

namespace AdventOfCode.AoC2018.ElfCode;

/// <summary>
/// Program solver
/// </summary>
public abstract partial class ElfCodeSolver : Solver<Program>
{
    [GeneratedRegex(@"([a-z]{4}) (\d+) (\d+) (\d)")]
    private static partial Regex InstructionRegex { get; }

    /// <summary>
    /// Creates a new <see cref="ElfCodeSolver"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    protected ElfCodeSolver(string input) : base(input) { }

    /// <inheritdoc />
    protected sealed override Program Convert(string[] rawInput)
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
        return new Program(ip, instructions);
    }
}
