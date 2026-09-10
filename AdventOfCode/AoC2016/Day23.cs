using System.Text.RegularExpressions;
using AdventOfCode.AoC2016.Assembunny;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 23
/// </summary>
public sealed class Day23 : RegexSolver<Instruction>
{
    private const int PART1_VALUE = 7;
    private const int PART2_VALUE = 12;

    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int password = RunProgram(PART1_VALUE);
        AoCUtils.LogPart1(password);

        password = RunProgram(PART2_VALUE);
        AoCUtils.LogPart2(password);
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
