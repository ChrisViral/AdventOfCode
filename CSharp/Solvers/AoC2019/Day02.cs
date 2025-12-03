using System;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 02
/// </summary>
public class Day02 : IntcodeSolver
{
    /// <summary>
    /// Maximum input argument value
    /// </summary>
    private const int MAX_ARG = 100;
    /// <summary>
    /// Target output value
    /// </summary>
    private const int TARGET  = 19690720;

    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day02(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.VM[1] = 12L;
        this.VM[2] = 2L;
        this.VM.Run();
        AoCUtils.LogPart1(this.VM[0]);

        foreach (int noun in ..MAX_ARG)
        {
            foreach (int verb in ..MAX_ARG)
            {
                // State already tested previously
                if (noun is 12 && verb is 2) continue;

                this.VM.Reset();
                this.VM[1] = noun;
                this.VM[2] = verb;
                this.VM.Run();
                if (this.VM[0] is TARGET)
                {
                    AoCUtils.LogPart2((noun * 100) + verb);
                    return;
                }
            }
        }
    }
}
