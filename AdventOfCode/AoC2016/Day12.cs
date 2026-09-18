using System.Text.RegularExpressions;
using AdventOfCode.AoC2016.Assembunny;
using Challenge.Solvers.Specialized;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 12
/// </summary>
public sealed class Day12 : RegexSolver<Instruction>
{
    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <summary>
    /// Creates a new <see cref="Day12"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day12(string input, ILogger logger) : base(input, logger) { }

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
