using AdventOfCode.AoC2019.Solvers;
using AdventOfCode.Intcode;
using AdventOfCode.Intcode.IO;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 07
/// </summary>
[Solver(2019, 7)]
public sealed class Day07 : IntcodeSolver
{
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create VM copies
        IntcodeVM ampA = this.VM;
        using IntcodeVM ampB = new(this.VM);
        using IntcodeVM ampC = new(this.VM);
        using IntcodeVM ampD = new(this.VM);
        using IntcodeVM ampE = new(this.VM);
        IntcodeVM[] amplifiers = [ampA, ampB, ampC, ampD, ampE];

        // Create input/output bridges
        QueueInOut ab = new();
        ampA.Output = ab;
        ampB.Input  = ab;
        QueueInOut bc = new();
        ampB.Output = bc;
        ampC.Input  = bc;
        QueueInOut cd = new();
        ampC.Output = cd;
        ampD.Input  = cd;
        QueueInOut de = new();
        ampD.Output = de;
        ampE.Input  = de;

        // Go over phase permutations
        long maxOutput = 0L;
        int[] phases = (..5).ToArray();
        foreach (int[] ampPerm in phases.PermutationsInPlace())
        {
            foreach (int i in ..amplifiers.Length)
            {
                amplifiers[i].Input.AddValue(ampPerm[i]);
            }

            ampA.Input.AddValue(0L);
            amplifiers.ForEach(amp => amp.Run());
            maxOutput = Math.Max(maxOutput, ampE.Output.GetValue());
            amplifiers.ForEach(amp => amp.Reset());
        }
        LogAnswer(maxOutput);

        // Bridge amplifiers E and A
        QueueInOut ea = new();
        ampE.Output = ea;
        ampA.Input  = ea;


        // Go over phase permutations
        maxOutput = 0L;
        phases    = (5..10).ToArray();
        foreach (int[] ampPerm in phases.PermutationsInPlace())
        {
            foreach (int i in ..amplifiers.Length)
            {
                amplifiers[i].Input.AddValue(ampPerm[i]);
            }

            ampA.Input.AddValue(0L);
            do
            {
                amplifiers.ForEach(amp => amp.Run());
            }
            while (!ampE.IsHalted);

            maxOutput = Math.Max(maxOutput, ampE.Output.GetValue());
            amplifiers.ForEach(amp => amp.Reset());
        }

        LogAnswer(maxOutput);
    }
}
