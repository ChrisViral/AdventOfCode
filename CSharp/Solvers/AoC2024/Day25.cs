using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 25
/// </summary>
public class Day25 : Solver<(Vector<byte>[] locks, Vector<byte>[] keys)>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day25(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Make result match vector
        Span<byte> matchSpan = stackalloc byte[Vector<byte>.Count];
        matchSpan[..5].Fill(5);
        Vector<byte> match = new(matchSpan);

        // Test all combinations
        int combinations = 0;
        foreach (Vector<byte> @lock in this.Data.locks)
        {
            foreach (Vector<byte> key in this.Data.keys)
            {
                if (Vector.LessThanOrEqualAll(@lock + key, match))
                {
                    combinations++;
                }
            }
        }

        AoCUtils.LogPart1(combinations);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Vector<byte>[], Vector<byte>[]) Convert(string[] rawInput)
    {
        // Init lock and keys
        List<Vector<byte>> locks = new(rawInput.Length / 14);
        List<Vector<byte>> keys  = new(rawInput.Length / 14);

        Span<byte> currentData = stackalloc byte[Vector<byte>.Count];
        Span<byte> usefulData  = currentData[..5];
        for (int offset = 0; offset < rawInput.Length; offset += 7)
        {
            ReadOnlySpan<string> current = rawInput.AsSpan(offset + 1, 5);
            foreach (int y in ..5)
            {
                ReadOnlySpan<char> line = current[y];
                foreach (int x in ..5)
                {
                    if (line[x] is '#')
                    {
                        usefulData[x]++;
                    }
                }
            }

            // Add to proper list
            if (rawInput[offset][0] is '#')
            {
                locks.Add(new Vector<byte>(currentData));
            }
            else
            {
                keys.Add(new Vector<byte>(currentData));
            }
            usefulData.Clear();
        }

        return (locks.ToArray(), keys.ToArray());
    }
    #endregion
}
