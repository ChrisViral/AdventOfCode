using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 05
/// </summary>
public class Day05 : Solver<Day05.Rule[][]>
{
    public class Rule(byte value, SearchValues<byte> mustFollow)
    {
        public byte Value { get; } = value;

        public SearchValues<byte> MustFollow { get; } = mustFollow;

        public override string ToString() => $"Rule({this.Value})";
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Rule"/>[][] fails</exception>
    public Day05(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        ILookup<bool, Rule[]> updatesLookup = this.Data.ToLookup(u => IsUpdateValid(u));
        int middlePages = updatesLookup[true].Sum(u => u[u.Length / 2].Value);
        AoCUtils.LogPart1(middlePages);

        middlePages = updatesLookup[false].Sum(u => FixUpdate(u));
        AoCUtils.LogPart2(middlePages);
    }

    private static bool IsUpdateValid(in ReadOnlySpan<Rule> update)
    {
        foreach (int i in ..(update.Length - 1))
        {
            Rule before = update[i];
            foreach (int j in ^i..update.Length)
            {
                if (before.MustFollow.Contains(update[j].Value)) return false;
            }
        }
        return true;
    }

    private static int FixUpdate(in Span<Rule> update)
    {
        foreach (int i in 1..update.Length)
        {
            ref Rule tail = ref update[^i];
            foreach (int j in ^i..^update.Length)
            {
                ref Rule before = ref update[^j];
                if (before.MustFollow.Contains(tail.Value))
                {
                    AoCUtils.Swap(ref tail, ref before);
                }
            }
        }
        return update[update.Length / 2].Value;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Rule[][] Convert(string[] rawInput)
    {
        int i = 0;
        List<byte>?[] rulesData = new List<byte>?[100];
        for (ReadOnlySpan<char> line = rawInput[i]; line.Length is 5; line = rawInput[++i])
        {
            byte pageBefore = byte.Parse(line[..2]);
            byte pageAfter  = byte.Parse(line[3..]);

            List<byte>? ruleAfter = rulesData[pageAfter];
            if (ruleAfter is null)
            {
                ruleAfter            = new List<byte>(20);
                rulesData[pageAfter] = ruleAfter;
            }

            ruleAfter.Add(pageBefore);
        }

        Rule?[] rules = new Rule?[100];
        foreach (byte id in ..100)
        {
            List<byte>? ruleData = rulesData[id];
            if (ruleData is not null)
            {
                rules[id] = new Rule(id, SearchValues.Create(CollectionsMarshal.AsSpan(ruleData)));
            }
        }

        Rule[][] updates = new Rule[rawInput.Length - i][];
        foreach (int j in ..updates.Length)
        {
            ReadOnlySpan<char> line = rawInput[i + j];
            Rule[] update = new Rule[(line.Length + 1) / 3];
            updates[j] = update;
            foreach (int k in ..update.Length)
            {
                byte ruleValue = byte.Parse(line.Slice(k * 3, 2));
                update[k] = rules[ruleValue]!;
            }
        }

        return updates;
    }
    #endregion
}
