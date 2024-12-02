﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 21
/// </summary>
public class Day21 : Solver<Dictionary<string, Day21.Monkey>>
{
    /// <summary>
    /// Monkey object
    /// </summary>
    public class Monkey
    {
        /// <summary>Root name</summary>
        public const string ROOT = "root";
        /// <summary>Self name</summary>
        public const string SELF = "humn";
        /// <summary>Regex matcher</summary>
        private static readonly Regex match = new(@"([a-z]{4}): (?:(\d+)|([a-z]{4}) ([\+\-\*\/]) ([a-z]{4}))", RegexOptions.Compiled);

        public readonly Dictionary<string, Monkey> monkeys;
        public readonly string? firstName;
        public readonly string? secondName;
        public readonly string? operation;

        /// <summary>
        /// Monkey name
        /// </summary>
        public string Name { get; }

        private long? value;
        /// <summary>
        /// Monkey value
        /// </summary>
        public long Value
        {
            get => this.value ?? FetchValue();
            set => this.value = value;
        }

        /// <summary>
        /// If this monkey is the root
        /// </summary>
        public bool IsRoot => this.Name is ROOT;

        /// <summary>
        /// If this monkey is self
        /// </summary>
        public bool IsSelf => this.Name is SELF;

        /// <summary>
        /// Creates a new monkey
        /// </summary>
        /// <param name="line">Value data</param>
        /// <param name="monkeys">Monkeys reference</param>
        public Monkey(string line, Dictionary<string, Monkey> monkeys)
        {
            this.monkeys = monkeys;
            string[] groups = match.Match(line)
                                   .GetCapturedGroups()
                                   .Select(g => g.Value)
                                   .ToArray();

            this.Name = groups[0];
            if (groups.Length is 2)
            {
                this.Value = int.Parse(groups[1]);
                return;
            }

            this.firstName  = groups[1];
            this.secondName = groups[3];
            this.operation = groups[2];
        }

        /// <summary>
        /// Fetches the value for this monkey
        /// </summary>
        /// <returns>This monkey's value</returns>
        private long FetchValue()
        {
            long first  = this.monkeys[this.firstName!].Value;
            long second = this.monkeys[this.secondName!].Value;
            return OperationResult(first, second);
        }

        /// <summary>
        /// Tries to fetch the value for this monkey, or gets the equation stack if unable to
        /// </summary>
        /// <param name="result">Found value</param>
        /// <param name="stack">Equation stack if value not found</param>
        /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
        public bool TryFetchValue(out long result, out string? stack)
        {
            result = 0L;
            stack = null;

            // If self, only return a variable name
            if (this.IsSelf)
            {
                stack = "x";
                return false;
            }

            // If no computation, return value
            if (this.value is not null)
            {
                result = this.value.Value;
                return true;
            }

            // Get values on each side
            bool firstCheck  = this.monkeys[this.firstName!].TryFetchValue(out long first, out string? firstStack);
            bool secondCheck = this.monkeys[this.secondName!].TryFetchValue(out long second, out string? secondStack);

            if (firstCheck)
            {
                if (secondCheck)
                {
                    // If both valid, compute
                    result = OperationResult(first, second);
                    return true;
                }

                // Otherwise return the correct stack
                stack = this.IsRoot ? $"{first}={secondStack}" : $"({first}{this.operation}{secondStack})";
                return false;
            }

            // Otherwise return the correct stack
            stack = this.IsRoot ? $"{second}={firstStack}" : $"({firstStack}{this.operation}{second})";
            return false;
        }

        /// <summary>
        /// Computes the value of this monkey from it's operation
        /// </summary>
        /// <param name="first">First value of the operation</param>
        /// <param name="second">Second value of the operation</param>
        /// <returns>The operation result</returns>
        private long OperationResult(long first, long second) => this.operation switch
        {
            "+" => first + second,
            "-" => first - second,
            "*" => first * second,
            "/" => first / second,
            _   => throw new UnreachableException("Unknown operation")
        };
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver for 2022 - 21 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day21(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        Monkey root = this.Data["root"];
        AoCUtils.LogPart1(root.Value);

        // Just process the equation out with Wolfram after
        root.TryFetchValue(out long _, out string? stack);
        AoCUtils.LogPart2(stack!);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Monkey> Convert(string[] lines)
    {
        Dictionary<string, Monkey> monkeys = new(lines.Length);
        foreach (string line in lines)
        {
            Monkey monkey = new(line, monkeys);
            monkeys.Add(monkey.Name, monkey);
        }

        return monkeys;
    }
    #endregion
}
