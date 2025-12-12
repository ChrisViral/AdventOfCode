using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 07
/// </summary>
public sealed class Day07 : ArraySolver<(long test, long[] operands)>
{
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        ILookup<bool, (long test, long[] operands)> calibrations = this.Data.ToLookup(IsValidOperation);
        long calibrationResult = calibrations[true].Sum(e => e.test);
        AoCUtils.LogPart1(calibrationResult);

        calibrationResult += calibrations[false].Where(IsValidWithConcatenation).Sum(e => e.test);
        AoCUtils.LogPart2(calibrationResult);
    }

    private static bool IsValidOperation((long test, long[] operands) equation)
    {
        static bool IsValidOperationRecursive(in long test, in ReadOnlySpan<long> operands, in long runningTotal, in int currentIndex)
        {
            long current = operands[currentIndex];
            int nextIndex = currentIndex + 1;
            // Check if we have a valid solution
            if (nextIndex == operands.Length)
            {
                return runningTotal + current == test || runningTotal * current == test;
            }

            // Check if there is a valid branch down by adding here
            long newTotal = runningTotal + current;
            if (newTotal <= test && IsValidOperationRecursive(test, operands, newTotal, nextIndex)) return true;

            // Check if there is a valid branch down by multiplying here
            newTotal = runningTotal * current;
            return newTotal <= test && IsValidOperationRecursive(test, operands, newTotal, nextIndex);
        }

        return IsValidOperationRecursive(equation.test, equation.operands, equation.operands[0], 1);
    }

    private static bool IsValidWithConcatenation((long test, long[] operands) equation)
    {
        static bool IsValidWithConcatenationRecursive(in long test, in ReadOnlySpan<long> operands, in long runningTotal, in int currentIndex)
        {
            long current = operands[currentIndex];
            int nextIndex = currentIndex + 1;
            // Check if we have a valid solution
            if (nextIndex == operands.Length)
            {
                return runningTotal + current == test || runningTotal * current == test || runningTotal.ConcatNum(current) == test;
            }

            // Check if there is a valid branch down by adding here
            long newTotal = runningTotal + current;
            if (newTotal <= test && IsValidWithConcatenationRecursive(test, operands, newTotal, nextIndex)) return true;

            // Check if there is a valid branch down by multiplying here
            newTotal = runningTotal * current;
            if (newTotal <= test && IsValidWithConcatenationRecursive(test, operands, newTotal, nextIndex)) return true;

            // Check if there is a valid branch down by concatenating here
            newTotal = runningTotal.ConcatNum(current);
            return newTotal <= test && IsValidWithConcatenationRecursive(test, operands, newTotal, nextIndex);
        }

        return IsValidWithConcatenationRecursive(equation.test, equation.operands, equation.operands[0], 1);
    }

    /// <inheritdoc />
    protected override (long test, long[] operands) ConvertLine(string line)
    {
        // Get result side
        ReadOnlySpan<char> lineSpan = line;
        Span<Range> splits = stackalloc Range[15];
        lineSpan.Split(splits, ':', StringSplitOptions.TrimEntries);
        long test = long.Parse(lineSpan[splits[0]]);

        // Get operands side
        ReadOnlySpan<char> operandsSpan = lineSpan[splits[1]];
        int length = operandsSpan.Split(splits, ' ');

        // Compile to an array
        long[] operands = new long[length];
        foreach (int i in ..length)
        {
            operands[i] = long.Parse(operandsSpan[splits[i]]);
        }

        return (test, operands);
    }
}
