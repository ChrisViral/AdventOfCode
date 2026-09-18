using Challenge.Utils.Extensions.Ranges;
using Challenge.Solvers;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 25
/// </summary>
public sealed class Day25 : Solver<(int cardKey, int doorKey)>
{
    /// <summary>
    /// Subject number for public keys
    /// </summary>
    private const int PUBLIC_SUBJECT = 7;
    /// <summary>
    /// Key mod factor
    /// </summary>
    private const int MOD = 20_201_227;

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day25(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Get loop number for card public key
        int loops = 0;
        long key = 1L;
        do
        {
            key = (key * PUBLIC_SUBJECT) % MOD;
            loops++;
        }
        while (key != this.Data.cardKey);

        //Get final private key
        key = 1L;
        foreach (int _ in ..loops)
        {
            key = (key * this.Data.doorKey) % MOD;
        }
        LogAnswer(key);
    }

    /// <inheritdoc />
    protected override (int, int) Convert(string[] rawInput) => (int.Parse(rawInput[0]), int.Parse(rawInput[1]));
}
