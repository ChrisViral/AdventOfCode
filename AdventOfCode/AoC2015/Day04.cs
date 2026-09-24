using System.Security.Cryptography;
using System.Text;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Numbers;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 4
/// </summary>
[Solver(2015, 4)]
public sealed class Day04 : Solver<byte[]>
{
    private const int PART1_SIZE = 5;
    private const int PART2_SIZE = 6;
    private static readonly byte[] Hash = new byte[MD5.HashSizeInBytes];
    private static readonly byte[] Hex  = new byte[MD5.HashSizeInBytes * 2];

    private readonly MD5 md5 = MD5.Create();

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
        LogAnswer(value);

        header = stackalloc byte[PART2_SIZE];
        do
        {
            GetHeader(++value, header);
        }
        while (!expectedHeader.SequenceEqual(header));
        LogAnswer(value);
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
