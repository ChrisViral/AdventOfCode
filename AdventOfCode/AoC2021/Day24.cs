using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 24
/// </summary>
public sealed class Day24 : Solver<(int a, int b, int c)[]>
{
    /// <summary>
    /// Digit count
    /// </summary>
    private const int DIGITS = 14;

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<char> largestResult  = stackalloc char[DIGITS];
        Span<char> smallestResult = stackalloc char[DIGITS];
        Stack<(int, int)> pushed  = new(DIGITS);
        foreach (int i in ..DIGITS)
        {
            (int a, int b, int c) = this.Data[i];
            // If a is 1, just push the current C value
            if (a is 1)
            {
                pushed.Push((i, c));
                continue;
            }

            // Pop the value and calculate the constant offset
            (int pushIndex, c) = pushed.Pop();
            int offset = b + c;

            // Get the highest valid digit
            int pushDigit    = 9;
            int currentDigit = pushDigit + offset;
            while (currentDigit > 9)
            {
                currentDigit = --pushDigit + offset;
            }

            // Store both digits
            largestResult[pushIndex] = (char)('0' + pushDigit);
            largestResult[i]         = (char)('0' + currentDigit);

            // Get the lowest valid digit
            pushDigit    = 1;
            currentDigit = pushDigit + offset;
            while (currentDigit <= 0)
            {
                currentDigit = ++pushDigit + offset;
            }

            // Store both digits
            smallestResult[pushIndex] = (char)('0' + pushDigit);
            smallestResult[i]         = (char)('0' + currentDigit);
        }

        // Print both results
        AoCUtils.LogPart1(largestResult.ToString());
        AoCUtils.LogPart2(smallestResult.ToString());
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (int a, int b, int c)[] Convert(string[] rawInput)
    {
        (int, int, int)[] result = new (int, int, int)[DIGITS];
        ReadOnlySpan<string> inputSpan = rawInput;
        foreach (int i in ..DIGITS)
        {
            ReadOnlySpan<string> block = inputSpan.Slice(i * 18, 18);
            int a = int.Parse(block[4].AsSpan(6));
            int b = int.Parse(block[5].AsSpan(6));
            int c = int.Parse(block[15].AsSpan(6));
            result[i] = (a, b, c);
        }

        return result;
    }
}
