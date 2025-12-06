using System;
using System.Linq;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 07
/// </summary>
public sealed class Day07 : ArraySolver<Day07.Hand>
{
    private const string ORDER   = "23456789TJQKA";
    private const string J_ORDER = "J23456789TQKA";

    public enum HandType
    {
        HIGH_CARD,
        ONE_PAIR,
        TWO_PAIR,
        TRIPLE,
        FULL_HOUSE,
        QUADRUPLE,
        ALL_SAME
    }

    public struct Hand : IComparable<Hand>
    {
        private const int HAND_SIZE = 5;
        private const char JOKER = 'J';

        public readonly string cards;
        public readonly int[] cardCounts;
        public readonly int bid;

        public bool UsesJokers { get; set; }

        public HandType HandType { get; set; }

        public Hand(string data)
        {
            this.cards = data[..HAND_SIZE];
            this.cardCounts = new int[ORDER.Length];
            foreach (char card in this.cards)
            {
                this.cardCounts[ORDER.IndexOf(card)]++;
            }

            this.bid = int.Parse(data[(HAND_SIZE + 1)..]);
            this.HandType = this.cardCounts.Max() switch
            {
                5 => HandType.ALL_SAME,
                4 => HandType.QUADRUPLE,
                3 => this.cardCounts.Contains(2) ? HandType.FULL_HOUSE : HandType.TRIPLE,
                2 => this.cardCounts.Count(c => c is 2) is 2 ? HandType.TWO_PAIR : HandType.ONE_PAIR,
                _ => HandType.HIGH_CARD
            };
        }

        /// <inheritdoc />
        public int CompareTo(Hand other)
        {
            int c = this.HandType - other.HandType;
            if (c is not 0) return c;

            foreach (int i in ..HAND_SIZE)
            {
                int a = GetCardStrength(this.cards[i]);
                int b = GetCardStrength(other.cards[i]);

                if (a == b) continue;

                return a - b;
            }

            return 0;
        }

        public int GetCardStrength(char c) => (this.UsesJokers ? J_ORDER : ORDER).IndexOf(c);

        public override string ToString() => $"{this.cards} {this.bid}";

        public static Hand ConvertJokers(Hand hand)
        {
            hand.UsesJokers = true;
            if (hand.HandType is HandType.ALL_SAME) return hand;

            int jokers = hand.cards.Count(c => c is JOKER);
            if (jokers is 0) return hand;

            hand.HandType = hand.HandType switch
            {
                HandType.QUADRUPLE  => HandType.ALL_SAME,
                HandType.FULL_HOUSE => HandType.ALL_SAME,
                HandType.TRIPLE     => HandType.QUADRUPLE,
                HandType.TWO_PAIR   => jokers is 2 ? HandType.QUADRUPLE : HandType.FULL_HOUSE,
                HandType.ONE_PAIR   => HandType.TRIPLE,
                HandType.HIGH_CARD  => HandType.ONE_PAIR,
                _                   => hand.HandType
            };

            return hand;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.Data.Sort();
        long winnings = CalculateWinnings();
        AoCUtils.LogPart1(winnings);

        this.Data.Apply(Hand.ConvertJokers);
        this.Data.Sort();
        winnings = CalculateWinnings();
        AoCUtils.LogPart2(winnings);
    }

    public long CalculateWinnings()
    {
        long winnings = 0L;
        foreach (int i in ..this.Data.Length)
        {
            winnings += (i + 1L) * this.Data[i].bid;
        }

        return winnings;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Hand ConvertLine(string line) => new(line);
}
