using System;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Intcode;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 07
/// </summary>
public class Day07 : Solver<IntcodeVM[]>
{
    #region Constants
    /// <summary>
    /// Amount of amplifiers running
    /// </summary>
    private const int AMPS = 5;
    /// <summary>
    /// Part 1 phase settings
    /// </summary>
    private static readonly long[] part1Phase = [0L, 1L, 2L, 3L, 4L];
    /// <summary>
    /// Part 2 phase settings
    /// </summary>
    private static readonly long[] part2Phase = [5L, 6L, 7L, 8L, 9L];
    #endregion

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
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        long max = long.MinValue;
        //Go through all permutations of part 1 settings
        foreach (long[] perm in part1Phase.Permutations())
        {
            //Add phase settings
            foreach (int i in ..AMPS)
            {
                this.Data[i].AddInput(perm[i]);
            }
            //Add input value
            this.Data[0].AddInput(0L);

            //Run all amplifiers
            this.Data.ForEach(amp => amp.Run());

            //Get value from last amplifier
            max = Math.Max(max, this.Data[^1].GetNextOutput());

            //Reset amplifiers
            this.Data.ForEach(amp => amp.Reset());
        }
        AoCUtils.LogPart1(max);

        //Set last output as first input
        this.Data[0].In = this.Data[^1].Out;
        max = long.MinValue;
        //Go through all permutations of part 2 settings
        foreach (long[] perm in part2Phase.Permutations())
        {
            //Add phase settings
            foreach (int i in ..AMPS)
            {
                this.Data[i].AddInput(perm[i]);
            }
            //Add input value
            this.Data[0].AddInput(0L);

            //Run until the last amp has halted
            while (!this.Data[^1].IsHalted)
            {
                //Run all amps
                this.Data.ForEach(amp => amp.Run());
            }

            //Get value from last amplifier
            max = Math.Max(max, this.Data[^1].GetNextOutput());

            //Reset amplifiers
            this.Data.ForEach(amp => amp.Reset());
        }
        AoCUtils.LogPart2(max);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override IntcodeVM[] Convert(string[] rawInput)
    {
        string line = rawInput[0];
        IntcodeVM[] vms = new IntcodeVM[AMPS];
        foreach (int i in ..AMPS)
        {
            IntcodeVM vm = new(line);
            vms[i] = vm;
            if (i > 0)
            {
                vm.In = vms[i - 1].Out;
            }
        }

        return vms;
    }
    #endregion
}
