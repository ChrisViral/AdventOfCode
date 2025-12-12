using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 16
/// </summary>
public sealed class Day16 : Solver<int[]>
{
    /// <summary>
    /// FFT Phases
    /// </summary>
    private const int PHASES = 100;
    /// <summary>
    /// Input copies
    /// </summary>
    private const int COPIES = 10000;

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create spans to store data
        int dataLength = this.Data.Length;
        Span<int> current = stackalloc int[dataLength];
        Span<int> updated = stackalloc int[dataLength];
        this.Data.CopyTo(current);

        // Execute phases
        foreach (int _ in ..PHASES)
        {
            // Update all digits and swap spans
            foreach (int i in ..dataLength)
            {
                updated[i] = GetUpdatedDigit(i, current);
            }
            AoCUtils.SwapSpans(ref current, ref updated);
        }

        // Get first eight digits
        Span<char> result = stackalloc char[8];
        current[..8].Select(c => (char)(c + '0')).CopyTo(result);
        AoCUtils.LogPart1(result.ToString());

        // Get starting offset
        int start = this.Data.AsSpan(0, 7).Aggregate((acc, d) => (acc * 10) + d);
        current = new int[dataLength * COPIES];

        // Copy repeated
        foreach (int i in ..COPIES)
        {
            this.Data.CopyTo(current.Slice(i * dataLength, dataLength));
        }

        // Get actual input from offset
        current    = current[start..];
        dataLength = current.Length;
        updated    = new int[dataLength];

        // Execute phases
        foreach (int _ in ..PHASES)
        {
            // Filter should all be 1 this far into the sequence
            int last = current[^1];
            for (int i = 2; i <= dataLength; i++)
            {
                last = updated[^i] = (last + current[^i]) % 10;
            }
            AoCUtils.SwapSpans(ref current, ref updated);
        }

        // Get first eight digits
        current[..8].Select(c => (char)(c + '0')).CopyTo(result);
        AoCUtils.LogPart2(result.ToString());
    }

    /// <summary>
    /// Updates an FFT digit
    /// </summary>
    /// <param name="index">Iteration index</param>
    /// <param name="values">Data value span</param>
    /// <returns></returns>
    private static int GetUpdatedDigit(int index, ReadOnlySpan<int> values)
    {
        int result = 0;
        int size = index + 1;
        int length = values.Length;
        do
        {
            // Check for end
            int end = index + size;
            if (end >= length)
            {
                result += values[index..].Sum();
                break;
            }

            // Sum first chunk
            result += values[index..end].Sum();
            index   = end + size;
            if (index >= length) break;

            // Check for end
            end = index + size;
            if (end >= length)
            {
                result -= values[index..].Sum();
                break;
            }

            // Subtract second chunk
            result -= values[index..end].Sum();
            index   = end + size;
        }
        while (index < length);

        // Truncate
        return Math.Abs(result) % 10;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput[0].AsSpan()
                                                                      .Select(c => c - '0')
                                                                      .ToArray();
}
