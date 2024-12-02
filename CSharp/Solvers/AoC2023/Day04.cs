using System;
using System.Buffers;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 04
/// </summary>
public class Day04 : Solver<Day04.Card[]>
{
    public readonly struct Card
    {
        public readonly byte[] winning;
        public readonly SearchValues<byte> numbers;

        public Card(string winning, string numbers)
        {
            this.winning = winning.Split(' ', DEFAULT_OPTIONS).ConvertAll(byte.Parse);
            this.numbers = SearchValues.Create(numbers.Split(' ', DEFAULT_OPTIONS).ConvertAll(byte.Parse));
        }
    }

    private const string CARD_PATTERN = @"Card\s+\d+: ([\d\s]+) \| ([\d\s]+)";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
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
        return RegexFactory<Card>.ConstructObjects(CARD_PATTERN, rawInput, RegexOptions.Compiled);
    }
    #endregion
}