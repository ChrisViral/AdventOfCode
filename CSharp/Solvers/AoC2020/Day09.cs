using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 09
/// </summary>
public class Day09 : Solver<long[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="long"/>[] fails</exception>
    public Day09(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        long invalid = 0L;
        for (int i = 0, j = 25; j < this.Data.Length; i++, j++)
        {
            long number = this.Data[j];
            if (!IsSumOfTwo(this.Data[i..j], number))
            {
                invalid = number;
                break;
            }
        }
        AoCUtils.LogPart1(invalid);

        int start = 0, end = 1;
        long sum = this.Data[start] + this.Data[end];
        while (sum != invalid && end < this.Data.Length)
        {
            if (sum > invalid && start + 1 != end)
            {
                sum -= this.Data[start++];
            }
            else
            {
                sum += this.Data[++end];
            }
        }

        long[] slice = this.Data[start..++end];
        AoCUtils.LogPart2(slice.Min() + slice.Max());
    }

    /// <summary>
    /// Checks if the target number is the sum of two numbers from the array
    /// </summary>
    /// <param name="array">Array to check in</param>
    /// <param name="target">Target sum to find</param>
    /// <returns>True if the target is the sum of any two numbers in the array, otherwise false</returns>
    private static bool IsSumOfTwo(long[] array, long target)
    {
        for (int i = 0; i < array.Length; /*i++*/)
        {
            long a = target - array[i];
            if (array[++i..].Any(b => a == b))
            {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override long[] Convert(string[] rawInput) => Array.ConvertAll(rawInput, long.Parse);
    #endregion
}