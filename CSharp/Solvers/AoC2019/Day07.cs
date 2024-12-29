using System;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 07
/// </summary>
public class Day07 : IntcodeSolver
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day07(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Create VM copies
        IntcodeVM ampA = this.VM;
        IntcodeVM ampB = new(this.VM);
        IntcodeVM ampC = new(this.VM);
        IntcodeVM ampD = new(this.VM);
        IntcodeVM ampE = new(this.VM);
        IntcodeVM[] amplifiers = [ampA, ampB, ampC, ampD, ampE];

        // Create input/output bridges
        QueueInOut ab = new();
        ampA.OutputProvider = ab;
        ampB.InputProvider  = ab;
        QueueInOut bc = new();
        ampB.OutputProvider = bc;
        ampC.InputProvider  = bc;
        QueueInOut cd = new();
        ampC.OutputProvider = cd;
        ampD.InputProvider  = cd;
        QueueInOut de = new();
        ampD.OutputProvider = de;
        ampE.InputProvider  = de;

        // Go over phase permutations
        long maxOutput = 0L;
        int[] phases = (..5).AsEnumerable().ToArray();
        foreach (int[] ampPerm in phases.PermutationsInPlace())
        {
            foreach (int i in ..amplifiers.Length)
            {
                amplifiers[i].InputProvider.AddInput(ampPerm[i]);
            }

            ampA.InputProvider.AddInput(0L);
            amplifiers.ForEach(amp => amp.Run());
            maxOutput = Math.Max(maxOutput, ampE.OutputProvider.GetOutput());
            amplifiers.ForEach(amp => amp.Reset());
        }
        AoCUtils.LogPart1(maxOutput);

        // Bridge amplifiers E and A
        QueueInOut ea = new();
        ampE.OutputProvider = ea;
        ampA.InputProvider  = ea;


        // Go over phase permutations
        maxOutput = 0L;
        phases    = (5..10).AsEnumerable().ToArray();
        foreach (int[] ampPerm in phases.PermutationsInPlace())
        {
            foreach (int i in ..amplifiers.Length)
            {
                amplifiers[i].InputProvider.AddInput(ampPerm[i]);
            }

            ampA.InputProvider.AddInput(0L);
            do
            {
                amplifiers.ForEach(amp => amp.Run());
            }
            while (!ampE.IsHalted);

            maxOutput = Math.Max(maxOutput, ampE.OutputProvider.GetOutput());
            amplifiers.ForEach(amp => amp.Reset());
        }

        AoCUtils.LogPart2(maxOutput);
    }
    #endregion
}