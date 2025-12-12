using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;
using SpanLinq;

namespace AdventOfCode.AoC2025;

/// <summary>
/// Solver for 2025 Day 06
/// </summary>
public sealed partial class Day06 : Solver<Grid<string>>
{
    [GeneratedRegex(@"([\*|\+] +)(?: |$)")]
    private static partial Regex OperatorPattern { get; }

    /// <summary>
    /// Creates a new <see cref="Day06"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day06(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        string[] column = new string[this.Data.Height];
        int numCount = column.Length - 1;
        long total = 0L;
        foreach (int x in ..this.Data.Width)
        {
            this.Data.GetColumn(x, column);
            ReadOnlySpan<string> numbers = column.AsSpan(0, numCount);
            total += column[^1][0] switch
            {
                '+' => numbers.Sum(long.Parse),
                '*' => numbers.Multiply(long.Parse),
                _   => throw new InvalidOperationException("Unknown operator")
            };
        }
        AoCUtils.LogPart1(total);

        total = 0L;
        long[] numbersBuffer = new long[4];
        foreach (int x in ..this.Data.Width)
        {
            this.Data.GetColumn(x, column);
            Span<long> numbers = numbersBuffer.AsSpan(0, column[0].Length);
            ParseVerticalNumbers(column.AsSpan(0, column.Length - 1), ref numbers);
            total += column[^1][0] switch
            {
                '+' => numbers.Sum(),
                '*' => numbers.Multiply(),
                _   => throw new InvalidOperationException("Unknown operator")
            };
        }
        AoCUtils.LogPart2(total);
    }

    private static void ParseVerticalNumbers(ReadOnlySpan<string> column, ref Span<long> output)
    {
        for (int x = column[0].Length - 1; x >= 0; x--)
        {
            long number = 0L;
            foreach (string verticalNumber in column)
            {
                char value = verticalNumber[x];
                if (value is ' ') continue;

                number *= 10L;
                number += value - '0';
            }
            output[x] = number;
        }
    }

    /// <inheritdoc />
    protected override Grid<string> Convert(string[] rawInput)
    {
        // Match operator pattern
        MatchCollection operators = OperatorPattern.Matches(rawInput[^1]);
        string[] line = operators.Select(m => m.Groups[1].Value).ToArray();

        // Create grid and fill with operators
        int width  = operators.Count;
        int height = rawInput.Length;
        Grid<string> grid = new(width, height, s => s);
        grid.SetRow(^1, line);

        // Parse rest of grid
        foreach (int y in ..(height - 1))
        {
            ReadOnlySpan<char> input = rawInput[y];
            foreach (int x in ..width)
            {
                Group operatorGroup = operators[x].Groups[1];
                line[x] = input.Slice(operatorGroup.Index, operatorGroup.Length).ToString();
            }

            grid.SetRow(y, line);
        }
        return grid;
    }
}
