﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 05
/// </summary>
public class Day05 : Solver<Day05.Rule[][]>
{
    public class Rule(int value) : IEquatable<Rule>
    {
        public int Value { get; } = value;

        public HashSet<Rule> MustFollow { get; } = [];

        public override string ToString() => $"Rule({this.Value})";

        /// <inheritdoc />
        public bool Equals(Rule? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Value == other.Value;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Rule rule && Equals(rule);

        /// <inheritdoc />
        public override int GetHashCode() => this.Value;
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
                Rule after = update[j];
                if (before.MustFollow.Contains(after)) return false;
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
                if (before.MustFollow.Contains(tail))
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
        Rule?[] rules = new Rule?[100];
        for (ReadOnlySpan<char> line = rawInput[i]; line.Length is 5; line = rawInput[++i])
        {
            int pageBefore = int.Parse(line[..2]);
            int pageAfter = int.Parse(line[3..]);

            Rule? ruleBefore = rules[pageBefore];
            if (ruleBefore is null)
            {
                ruleBefore        = new Rule(pageBefore);
                rules[pageBefore] = ruleBefore;
            }

            Rule? ruleAfter = rules[pageAfter];
            if (ruleAfter is null)
            {
                ruleAfter        = new Rule(pageAfter);
                rules[pageAfter] = ruleAfter;
            }

            ruleAfter.MustFollow.Add(ruleBefore);
        }

        Rule[][] updates = new Rule[rawInput.Length - i][];
        foreach (int j in ..updates.Length)
        {
            ReadOnlySpan<char> line = rawInput[i + j];
            Rule[] update = new Rule[(line.Length + 1) / 3];
            updates[j] = update;
            foreach (int k in ..update.Length)
            {
                int ruleValue = int.Parse(line.Slice(k * 3, 2));
                update[k] = rules[ruleValue]!;
            }
        }

        return updates;
    }
    #endregion
}
