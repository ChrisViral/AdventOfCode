using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;
using AdventOfCode.Utils.Extensions.Spans;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 16
/// </summary>
public sealed class Day16 : Solver<string>
{
    private const int PART1_SIZE = 272;
    private const int PART2_SIZE = 35651584;
    private const int BUFFER_SIZE = ((PART2_SIZE - 1) * 2) + 1;

    private static readonly bool[] Buffer = new bool[BUFFER_SIZE];
    private static readonly bool[] Copy   = new bool[BUFFER_SIZE];

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int bufferLength = this.Data.Length;
        this.Data.AsValueEnumerable()
                 .Select(c => c is '1')
                 .CopyTo(Buffer);

        string checksum = CalculateChecksum(PART1_SIZE, ref bufferLength);
        AoCUtils.LogPart1(checksum);

        checksum = CalculateChecksum(PART2_SIZE, ref bufferLength);
        AoCUtils.LogPart2(checksum);
    }

    private static string CalculateChecksum(int diskSize, ref int bufferLength)
    {
        while (bufferLength < diskSize)
        {
            Span<bool> dataBuffer = Buffer.AsSpan(0, bufferLength);
            Span<bool> copyBuffer = Copy.AsSpan(0, bufferLength);

            dataBuffer.CopyTo(copyBuffer);
            copyBuffer.Reverse();
            copyBuffer.Apply(v => !v);
            copyBuffer.CopyTo(Buffer.AsSpan(bufferLength + 1, bufferLength));
            bufferLength = (bufferLength * 2) + 1;
        }

        Buffer.AsSpan(0, diskSize).CopyTo(Copy);
        Span<bool> checksumBuffer = Copy.AsSpan(0, diskSize);
        int checksumLength = checksumBuffer.Length;
        while (checksumLength.IsEven)
        {
            for (int i = 0; i < checksumLength; i += 2)
            {
                bool isPair = checksumBuffer[i] == checksumBuffer[i + 1];
                checksumBuffer[i / 2] = isPair;
            }

            checksumLength /= 2;
            checksumBuffer = checksumBuffer[..checksumLength];
        }

        Span<char> checksum = stackalloc char[checksumLength];
        checksumBuffer.AsValueEnumerable()
                      .Select(v => v ? '1' : '0')
                      .CopyTo(checksum);
        return checksum.ToString();
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
