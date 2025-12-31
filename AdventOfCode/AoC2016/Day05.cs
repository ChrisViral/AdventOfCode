using System.Security.Cryptography;
using System.Text;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 05
/// </summary>
public sealed class Day05 : Solver<string>
{
    private const int PASSWORD_LENGTH = 8;

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Put the data to hash in a span
        Span<byte> data = stackalloc byte[this.Data.Length + 10];
        Encoding.UTF8.TryGetBytes(this.Data, data, out int dataWritten);
        Span<byte> idSpan = data[dataWritten..];

        // Setup password storage
        Span<char> firstPassword  = stackalloc char[PASSWORD_LENGTH];
        Span<char> secondPassword = stackalloc char[PASSWORD_LENGTH];

        // Setup for hashing
        using MD5 md5 = MD5.Create();
        Span<byte> hash = stackalloc byte[MD5.HashSizeInBytes];
        for (int i = 0, j = 0, id = 0; j < PASSWORD_LENGTH; id++)
        {
            // Encode the ID to the data
            id.TryFormat(idSpan, out int idWritten);
            int totalLength = dataWritten + idWritten;
            Span<byte> hashData = data[..totalLength];

            // Compute hash
            md5.Initialize();
            md5.TryComputeHash(hashData, hash, out _);

            // Check that the first five hex digits are 0
            if (hash[0] is 0 && hash[1] is 0 && hash[2] <= 0b00001111)
            {
                // Get sixth hex digit out
                byte sixthDigit = hash[2];
                if (i < PASSWORD_LENGTH)
                {
                    // Apply it to the first password
                    firstPassword[i++] = StringUtils.HEX_LOWER[hash[2]];
                }

                // If the sixth digit could be a password position index
                if (sixthDigit < PASSWORD_LENGTH && secondPassword[sixthDigit] is '\0')
                {
                    // Apply the seventh digit to the second password
                    secondPassword[sixthDigit] = StringUtils.HEX_LOWER[hash[3] >> 4];
                    j++;
                }
            }
        }
        // Print passwords
        AoCUtils.LogPart1(firstPassword.ToString());
        AoCUtils.LogPart2(secondPassword.ToString());
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
