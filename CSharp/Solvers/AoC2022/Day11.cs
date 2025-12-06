using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 11
/// </summary>
public sealed class Day11 : Solver<Day11.Monkey[]>
{
    /// <summary>
    /// Worry value update function
    /// </summary>
    /// <param name="oldValue">old worry value</param>
    /// <returns>The updated worry value</returns>
    public delegate long WorryUpdate(long oldValue);

    /// <summary>
    /// Target monkey determination function
    /// </summary>
    /// <param name="value">Worry value</param>
    /// <returns>The index of the monkey to which the item should be sent</returns>
    public delegate Index WorryTest(long value);

    /// <summary>
    /// Monkey object
    /// </summary>
    public sealed class Monkey
    {
        /// <summary>
        /// Items queue
        /// </summary>
        public Queue<long> Items { get; }

        /// <summary>
        /// Worry update function
        /// </summary>
        public WorryUpdate Update { get; }

        /// <summary>
        /// Worry test function
        /// </summary>
        public WorryTest Test { get; }

        /// <summary>
        /// Divisibility value for this monkey
        /// </summary>
        public int Divisibility { get; }

        /// <summary>
        /// Total inspections count
        /// </summary>
        public long Inspections { get; private set; }

        /// <summary>
        /// Parses a new monkey from data
        /// </summary>
        /// <param name="data">Data to parse from</param>
        public Monkey(IReadOnlyList<string> data)
        {
            this.Items = new Queue<long>(data[1][16..].Split(", ").Select(long.Parse));
            string[] operation = data[2][17..].Split(' ');
            if (operation[1] is "+")
            {
                int value = int.Parse(operation[2]);
                this.Update = old => old + value;
            }
            else
            {
                if (operation[2] is "old")
                {
                    this.Update = old => old * old;
                }
                else
                {
                    int value = int.Parse(operation[2]);
                    this.Update = old => old * value;
                }
            }

            this.Divisibility = int.Parse(data[3][19..]);
            int ifTrue    = int.Parse(data[4][25..]);
            int ifFalse   = int.Parse(data[5][26..]);
            this.Test     = value => value.IsMultiple(this.Divisibility) ? ifTrue : ifFalse;
        }

        /// <summary>
        /// Monkey copy constructor
        /// </summary>
        /// <param name="original">Original monkey</param>
        public Monkey(Monkey original)
        {
            this.Items        = new Queue<long>(original.Items);
            this.Update       = original.Update;
            this.Test         = original.Test;
            this.Divisibility = original.Divisibility;
            this.Inspections  = original.Inspections;
        }

        /// <summary>
        /// Inspects all the items in this monkey's queue
        /// </summary>
        /// <param name="mod">Mod value, pass zero to instead divide by three</param>
        /// <returns>An enumerable of the update worry values, and the index of which monkey the item should go to</returns>
        public IEnumerable<(long item, Index target)> InspectItems(int mod)
        {
            while (Items.TryDequeue(out long worry))
            {
                yield return InspectItem(worry, mod);
            }
        }

        /// <summary>
        /// Inspects an item
        /// </summary>
        /// <param name="worry">Item to inspect worry value</param>
        /// <param name="mod">Mod value, pass zero to instead divide by three</param>
        /// <returns>A tuple containing the updated worry value, and target index of which monkey the item should go to</returns>
        private (long worry, Index target) InspectItem(long worry, int mod)
        {
            Inspections++;
            worry = Update(worry);
            if (mod is 0)
            {
                worry /= 3L;
            }
            else
            {
                worry %= mod;
            }
            Index target = Test(worry);
            return (worry, target);
        }
    }

    /// <summary>Rounds count for the first part</summary>
    private const int ROUNDS = 20;
    /// <summary>Rounds count for the second part</summary>
    private const int LONG_ROUNDS = 10000;

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver for 2022 - 11 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(PlayRound(ROUNDS, false));

        AoCUtils.LogPart2(PlayRound(LONG_ROUNDS, true));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Monkey[] Convert(string[] lines)
    {
        Monkey[] monkeys = new Monkey[lines.Length / 6];
        foreach (int i in ..monkeys.Length)
        {
            int start = i * 6;
            monkeys[i] = new Monkey(lines[start..(start + 6)]);
        }
        return monkeys;
    }

    private long PlayRound(int rounds, bool useMod)
    {
        int mod = useMod ? this.Data.Multiply(m => m.Divisibility) : 0;
        Monkey[] monkeys = this.Data.Select(m => new Monkey(m)).ToArray();
        foreach (int _ in ..rounds)
        {
            foreach ((long worry, Index target) in monkeys.SelectMany(m => m.InspectItems(mod)))
            {
                monkeys[target].Items.Enqueue(worry);
            }
        }

        return monkeys.OrderByDescending(m => m.Inspections)
                      .Take(2)
                      .Multiply(m => m.Inspections);
    }
}
