using AdventOfCode.Intcode;
using AdventOfCode.Intcode.IO.Networking;
using Challenge.Solvers;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 23
/// </summary>
[Solver(2019, 23)]
public sealed class Day23 : Solver<NAT>
{
    private const int COMPUTERS = 50;

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Start network
        this.Data.Start();

        // Wait for first packet to come in
        this.Data.WaitForFirstPacket();
        LogAnswer(this.Data.StoredPacket.Y);

        // Wait until the network completes
        this.Data.WaitForCompletion();
        LogAnswer(this.Data.StoredPacket.Y);
    }

    /// <inheritdoc />
    protected override NAT Convert(string[] rawInput)
    {
        // Make template VM
        IntcodeVM template = new(rawInput[0]);
        return new NAT(template, COMPUTERS);
    }
}
