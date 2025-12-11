using System;
using AdventOfCode.Intcode;
using AdventOfCode.Intcode.Networking;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 23
/// </summary>
public sealed class Day23 : Solver<NAT>
{
    private const int COMPUTERS = 50;

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Start network
        this.Data.Start();

        // Wait for first packet to come in
        this.Data.WaitForFirstPacket();

        // Wait for completion
        AoCUtils.LogPart1(this.Data.StoredPacket.Y);

        // Wait until the network completes
        this.Data.WaitForCompletion();

        AoCUtils.LogPart2(this.Data.StoredPacket.Y);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override NAT Convert(string[] rawInput)
    {
        // Make template VM
        IntcodeVM template = new(rawInput[0]);
        return new NAT(template, COMPUTERS);
    }
}
