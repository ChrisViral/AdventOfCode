using System.Security.Cryptography;
using System.Text;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Numbers;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 04
/// </summary>
public sealed class Day04 : Solver<byte[]>
{
    private const int PART1_SIZE = 5;
    private const int PART2_SIZE = 6;
    private static readonly byte[] Hash = new byte[MD5.HashSizeInBytes];
    private static readonly byte[] Hex  = new byte[MD5.HashSizeInBytes * 2];

    private readonly MD5 md5 = MD5.Create();

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        ReadOnlySpan<byte> expectedHeader = "000000"u8;

        int value = 0;
        Span<byte> header = stackalloc byte[PART1_SIZE];
        do
        {
            GetHeader(++value, header);
        }
        while (!expectedHeader.StartsWith(header));
        AoCUtils.LogPart1(value);

        header = stackalloc byte[PART2_SIZE];
        do
        {
            GetHeader(++value, header);
        }
        while (!expectedHeader.SequenceEqual(header));
        AoCUtils.LogPart2(value);
    }

    private void GetHeader(int value, Span<byte> header)
    {
        Span<byte> hashKey = stackalloc byte[this.Data.Length + value.DigitCount];
        this.Data.CopyTo(hashKey);
        value.TryFormat(hashKey[this.Data.Length..], out _);

        this.md5.TryComputeHash(hashKey, Hash, out _);
        System.Convert.TryToHexStringLower(Hash, Hex, out _);
        Hex.AsSpan(0, header.Length).CopyTo(header);
    }

    /// <inheritdoc />
    protected override byte[] Convert(string[] rawInput) => Encoding.UTF8.GetBytes(rawInput[0]);

    /// <inheritdoc />
    public override void Dispose()
    {
        this.md5.Dispose();
        base.Dispose();
    }
}
