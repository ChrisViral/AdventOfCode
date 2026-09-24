using System.Text.RegularExpressions;
using AdventOfCode.AoC2016.Assembunny;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 25
/// </summary>
[Solver(2016, 25)]
public sealed class Day25 : RegexSolver<Instruction>
{
    // Low, but seemingly good enough
    private const int THRESHOLD = 10;

    /// <inheritdoc />
    protected override Regex Matcher => Instruction.Matcher;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Test program until we find a value that works
        int value = 0;
        while (!TestOutput(++value));
        LogAnswer(value);
    }

    private bool TestOutput(int value)
    {
        // Setup data
        int address = 0;
        Registers registers = new();
        registers[0] = value;

        // Setup sequence tracking
        int length = 0;
        bool nextExpected = false;

        // Run instructions
        while (address >= 0 && address < this.Data.Length && length < THRESHOLD)
        {
            int? output = this.Data[address].Execute(ref address, ref registers);

            // If no output, ignore
            if (output is null) continue;

            // Make sure the output is as expected, if not, exit now
            bool isOn = output is 1;
            if (isOn != nextExpected) return false;

            // If it is, increase current sequence and flip next expected output
            length++;
            nextExpected = !nextExpected;
        }

        return length is THRESHOLD;
    }
}
