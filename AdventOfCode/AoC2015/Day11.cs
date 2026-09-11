using System.Buffers;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Strings;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 11
/// </summary>
public sealed partial class Day11 : Solver<string>
{
    private static readonly SearchValues<char> Banned = SearchValues.Create("iol");

    [GeneratedRegex(@"([a-z])\1.*(?!\1)([a-z])\2")]
    private static partial Regex DualPairMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<int> data = stackalloc int[this.Data.Length];
        this.Data.AsValueEnumerable().Select(c => c.AsIndex).CopyTo(data);
        while (!IsValid(data))
        {
            Increment(data);
        }

        Span<char> password = stackalloc char[data.Length];
        data.AsValueEnumerable().Select(c => c.AsAsciiLower).CopyTo(password);
        AoCUtils.LogPart1(password.ToString());

        do
        {
            Increment(data);
        }
        while (!IsValid(data));
        data.AsValueEnumerable().Select(c => c.AsAsciiLower).CopyTo(password);
        AoCUtils.LogPart2(password.ToString());
    }

    private static void Increment(Span<int> data)
    {
        int i = 0;
        for (ref int digit = ref i; digit is 0; )
        {
            i %= data.Length;
            i++;
            digit = ref data[^i];
            digit = (digit + 1) % StringUtils.LETTER_COUNT;
        }
    }

    private static bool IsValid(ReadOnlySpan<int> data)
    {
        Span<char> password = stackalloc char[data.Length];
        data.AsValueEnumerable().Select(c => c.AsAsciiLower).CopyTo(password);
        if (password.ContainsAny(Banned) || !DualPairMatcher.IsMatch(password)) return false;

        int a = data[0];
        int b = data[1];
        for (int i = 2; i < data.Length; i++)
        {
            int c = data[i];
            if (a + 1 == b && b + 1 == c) return true;

            a = b;
            b = c;
        }

        return false;
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
