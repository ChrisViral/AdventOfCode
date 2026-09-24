using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 9
/// </summary>
[Solver(2020, 9)]
public sealed class Day09 : Solver<long[]>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
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
        LogAnswer(invalid);

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
        LogAnswer(slice.Min() + slice.Max());
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

    /// <inheritdoc />
    protected override long[] Convert(string[] rawInput) => rawInput.ConvertAll(long.Parse);
}
