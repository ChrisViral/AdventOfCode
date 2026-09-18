using AdventOfCode.AoC2019.Solvers;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 21
/// </summary>
public sealed class Day21 : IntcodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day21(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        ReadOnlySpan<string> walkProgram =
        [
            "NOT A J",
            "NOT B T",
            "AND D T",
            "OR T J",
            "NOT C T",
            "AND D T",
            "OR T J"
        ];

        RunProgram(walkProgram, "WALK", out long result);
        LogAnswer(result);

        this.VM.Reset();

        ReadOnlySpan<string> runProgram =
        [
            "NOT A J",
            "NOT B T",
            "AND D T",
            "OR T J",
            "NOT C T",
            "AND D T",
            "AND H T",
            "OR T J"
        ];

        RunProgram(runProgram, "RUN", out result);
        LogAnswer(result);
    }

    private void RunProgram(ReadOnlySpan<string> program, string command, out long result)
    {
        this.VM.Run();
        this.VM.Output.PrintAllLines();
        foreach (string line in program)
        {
            this.VM.Input.WriteLine(line);
        }
        this.VM.Input.WriteLine(command);

        this.VM.Run();
        while (this.VM.Output.TryPeekValue(out result) && result <= byte.MaxValue)
        {
            this.VM.Output.PrintLine();
        }
    }
}
