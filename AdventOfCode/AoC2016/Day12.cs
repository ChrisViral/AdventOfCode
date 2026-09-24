using System.Text.RegularExpressions;
using AdventOfCode.AoC2016.Assembunny;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 12
/// </summary>
[Solver(2016, 12)]
public sealed class Day12 : RegexSolver<Instruction>
{
    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int address = 0;
        Registers registers = new();
        while (address >= 0 && address < this.Data.Length)
        {
            this.Data[address].Execute(ref address, ref registers);
        }
        LogAnswer(registers[0]);

        address = 0;
        registers = new Registers();
        registers[2] = 1;
        while (address >= 0 && address < this.Data.Length)
        {
            this.Data[address].Execute(ref address, ref registers);
        }
        LogAnswer(registers[0]);
    }
}
