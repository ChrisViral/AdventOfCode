using AdventOfCode.AoC2017.Common;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 10
/// </summary>
public sealed class Day10 : Solver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day10(string input) : base(input) { }

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
        AoCUtils.LogPart1(list[0] * list[1]);

        // Use full hash function
        UInt128 hash = Knot.Hash(this.Data);
        AoCUtils.LogPart2(hash.ToString("x"));
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
