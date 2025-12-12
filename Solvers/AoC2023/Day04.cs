using System;
using System.Buffers;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 04
/// </summary>
public sealed partial class Day04 : Solver<Day04.Card[]>
{
    public readonly struct Card(string winning, string numbers)
    {
        public readonly byte[] winning = winning.Split(' ', DEFAULT_OPTIONS).ConvertAll(byte.Parse);
        public readonly SearchValues<byte> numbers = SearchValues.Create(numbers.Split(' ', DEFAULT_OPTIONS).ConvertAll(byte.Parse));
    }

    [GeneratedRegex(@"Card\s+\d+: ([\d\s]+) \| ([\d\s]+)")]
    private static partial Regex CardMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        Span<int> cardMatches = stackalloc int[this.Data.Length];
        for (int i = 0; i < this.Data.Length; i++)
        {
            int matches = 0;
            Card card = this.Data[i];
            foreach (byte w in card.winning)
            {
                matches += card.numbers.Contains(w) ? 1 : 0;
            }

            if (matches is 0) continue;

            cardMatches[i] = matches;
            total += 1 << (matches - 1);
        }

        AoCUtils.LogPart1(total);

        total = 0;
        int currentCards = 1;
        Span<int> scratchcards = stackalloc int[this.Data.Length];
        for (int i = 0; i < this.Data.Length; i++)
        {
            total += currentCards;
            int match = cardMatches[i];
            if (match is not 0)
            {
                scratchcards[i] += currentCards;
                scratchcards[i + match] -= currentCards;
            }

            currentCards += scratchcards[i];
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Card[] Convert(string[] rawInput)
    {
        return RegexFactory<Card>.ConstructObjects(CardMatcher, rawInput);
    }
}
