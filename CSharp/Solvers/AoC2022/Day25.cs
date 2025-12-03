using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 25
/// </summary>
public class Day25 : ArraySolver<long>
{
    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver for 2022 - 25 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        // Get the data sum and then convert to SNAFU
        long sum = this.Data.Sum();
        AoCUtils.LogPart1(ToSNAFU(sum));
    }

    /// <summary>
    /// Parses a SNAFU number to decimal base
    /// </summary>
    /// <param name="snafu">Snafu number</param>
    /// <returns>The decimal value of this SNAFU number</returns>
    private static long ParseSNAFU(string snafu)
    {
        // Get the largest exponent
        long value = 0L;
        long exp = (long)Math.Pow(5, snafu.Length - 1);
        foreach (char c in snafu)
        {
            int digit = c switch
            {
                '=' => -2,
                '-' => -1,
                '0' => 0,
                '1' => 1,
                '2' => 2,
                _   => throw new UnreachableException("Unknown digit")
            };

            // Add the digit and reduce the exponent
            value += digit * exp;
            exp /= 5L;
        }

        return value;
    }

    /// <summary>
    /// Converts a decimal number to SNAFU base
    /// </summary>
    /// <param name="value">Number to convert</param>
    /// <returns>The SNAFU value of the number</returns>
    private static string ToSNAFU(long value)
    {
        static bool ReduceSNAFU(List<int> snafu)
        {
            // Go through the whole number
            foreach (int i in ..snafu.Count)
            {
                int value = snafu[i];
                // If value is greater than two, we are two high
                if (value > 2)
                {
                    snafu[i] -= 5;
                    if (i is 0)
                    {
                        // Insert a 1 if nothing before
                        snafu.Insert(0, 1);
                    }
                    else
                    {
                        // Increment value if there was something
                        snafu[i - 1]++;
                    }

                    // Loop needs to restart
                    return false;
                }
            }

            // No changes made, SNAFU fully reduced
            return true;
        }

        // Stack the individual base 5 digits
        Stack<int> base5 = new();
        while (value > 0L)
        {
            // Remainder will be the base 5 digits, reversed
            value = Math.DivRem(value, 5L, out long remainder);
            base5.Push((int)remainder);
        }

        // Convert stack to list will reverse the order
        List<int> snafu = base5.ToList();
        // Fully reduce it
        while (!ReduceSNAFU(snafu)) { }

        // Convert to SNAFU base
        char[] buffer = new char[snafu.Count];
        foreach (int i in ..snafu.Count)
        {
            buffer[i] = snafu[i] switch
            {
                -2 => '=',
                -1 => '-',
                0  => '0',
                1  => '1',
                2  => '2',
                _  => throw new UnreachableException("Unknown digit")
            };
        }

        return new string(buffer);
    }

    /// <inheritdoc cref="ArraySolver{T}.ConvertLine"/>
    protected override long ConvertLine(string line) => ParseSNAFU(line);
}
