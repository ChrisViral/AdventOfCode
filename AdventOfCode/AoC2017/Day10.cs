using AdventOfCode.AoC2017.Common;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 10
/// </summary>
[Solver(2017, 10)]
public sealed class Day10 : Solver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create hash list
        int position = 0;
        int skip     = 0;
        Span<byte> list = stackalloc byte[Knot.SIZE];
        foreach (int i in ..Knot.SIZE)
        {
            list[i] = (byte)i;
        }

        // Get length numbers
        byte[] numbers = this.Data.Split(',').ConvertAll(byte.Parse);

        // Hash and output
        Knot.HashIteration(ref list, ref position, ref skip, numbers);
        LogAnswer(list[0] * list[1]);

        // Use full hash function
        UInt128 hash = Knot.Hash(this.Data);
        LogAnswer(hash.ToString("x"));
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
