using System.Text.RegularExpressions;
using AdventOfCode.AoC2016.Assembunny;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 23
/// </summary>
[Solver(2016, 23)]
public sealed class Day23 : RegexSolver<Instruction>
{
    private const int PART1_VALUE = 7;
    private const int PART2_VALUE = 12;

    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int password = RunProgram(PART1_VALUE);
        LogAnswer(password);

        password = RunProgram(PART2_VALUE);
        LogAnswer(password);
    }

    private int RunProgram(int initialValue)
    {
        // Copy instructions to avoid modification
        Span<Instruction> instructions = stackalloc Instruction[this.Data.Length];
        this.Data.CopyTo(instructions);

        // Setup initial data
        int address = 0;
        Registers registers = new();
        registers[0] = initialValue;

        // Run instructions
        while (address >= 0 && address < instructions.Length)
        {
            instructions[address].Execute(ref address, ref registers, instructions);
        }

        // Return register a
        return registers[0];
    }
}
