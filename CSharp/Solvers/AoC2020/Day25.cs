using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using System;
using AdventOfCode.Extensions;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 25
/// </summary>
public class Day25 : Solver<(int cardKey, int doorKey)>
{
    #region Constants
    /// <summary>
    /// Subject number for public keys
    /// </summary>
    private const int PUBLIC_SUBJECT = 7;
    /// <summary>
    /// Key mod factor
    /// </summary>
    private const int MOD = 20_201_227;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1,T2}"/> fails</exception>
    public Day25(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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
        AoCUtils.LogPart1(key);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (int, int) Convert(string[] rawInput) => (int.Parse(rawInput[0]), int.Parse(rawInput[1]));
    #endregion
}