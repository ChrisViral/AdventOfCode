using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 19
/// </summary>
public sealed class Day19 : Solver<(Dictionary<string, Day19.Workflow> workflows, Day19.Part[] parts)>
{
    public enum Category { X, M, A, S }

    public enum Operation
    {
        LESS    = '<',
        GREATER = '>'
    }

    public readonly struct Workflow(string label, string rules, string noMatch)
    {
        public readonly string label   = label;
        public readonly Rule[] rules   = rules.Split(',').Select(r => new Rule(r)).ToArray();
        public readonly string noMatch = noMatch;

        public string TestPart(in Part part)
        {
            foreach (Rule rule in this.rules)
            {
                if (rule.IsPartValid(part))
                {
                    return rule.target;
                }
            }

            return this.noMatch;
        }

        public override string ToString() => $"{this.label}{{{string.Join(",", this.rules)},{this.noMatch}}}";
    }

    public readonly struct Rule
    {
        private delegate bool RuleTest(in Part part);

        private const string RULE_PATTERN = @"([xmas])([<>])(\d+):([a-z]+|A|R)";
        private static readonly Regex ruleMatch = new(RULE_PATTERN, RegexOptions.Compiled);

        public readonly Category category;
        public readonly Operation operation;
        public readonly int value;
        public readonly string target;

        private readonly RuleTest test;

        public Rule(string rule)
        {
            string[] sections = ruleMatch.Match(rule).CapturedGroups.Select(g => g.Value).ToArray();
            this.category     = Enum.Parse<Category>(sections[0], true);
            this.operation    = (Operation)sections[1][0];
            this.value        = int.Parse(sections[2]);
            this.target       = sections[3];

            this.test = this.operation is Operation.LESS ? LessThanRule : GreaterThanRule;
        }

        private bool LessThanRule(in Part part) => part[this.category] < this.value;

        private bool GreaterThanRule(in Part part) => part[this.category] > this.value;

        public bool IsPartValid(in Part part) => this.test(part);

        public override string ToString() => $"{this.category.ToString().ToLower()}{(this.operation is Operation.LESS ? "<" : ">")}{value}:{target}";
    }

    public record struct Part(int X, int M, int A, int S)
    {
        public int Rating => this.X + this.M + this.A + this.S;

        public int this[Category category] => category switch
        {
            Category.X => this.X,
            Category.M => this.M,
            Category.A => this.A,
            Category.S => this.S,
            _          => throw new UnreachableException("Invalid category detected")
        };
    }

    public record struct PartRange(Range X, Range M, Range A, Range S)
    {
        public long ValidCount => (this.X.End.Value - this.X.Start.Value + 1L)
                                * (this.M.End.Value - this.M.Start.Value + 1L)
                                * (this.A.End.Value - this.A.Start.Value + 1L)
                                * (this.S.End.Value - this.S.Start.Value + 1L);

        public bool HasAnyValid => (this.X.End.Value >= this.X.Start.Value)
                                && (this.M.End.Value >= this.M.Start.Value)
                                && (this.A.End.Value >= this.A.Start.Value)
                                && (this.S.End.Value >= this.S.Start.Value);

        public Range this[Category category] => category switch
        {
            Category.X => this.X,
            Category.M => this.M,
            Category.A => this.A,
            Category.S => this.S,
            _          => throw new UnreachableException("Invalid category detected")
        };

        public (PartRange valid, PartRange invalid) SplitRange(in Rule rule)
        {
            Range range = this[rule.category];
            return rule.operation switch
            {
                Operation.LESS    => (ReplaceRange(range.Start..(rule.value - 1), rule.category),
                                      ReplaceRange(rule.value..range.End, rule.category)),
                Operation.GREATER => (ReplaceRange((rule.value + 1)..range.End, rule.category),
                                      ReplaceRange(range.Start..rule.value, rule.category)),
                _                 => throw new UnreachableException("Unknown operation")
            };
        }

        private PartRange ReplaceRange(in Range newRange, Category category) => category switch
        {
            Category.X => this with { X = newRange },
            Category.M => this with { M = newRange },
            Category.A => this with { A = newRange },
            Category.S => this with { S = newRange },
            _          => throw new UnreachableException("Invalid category detected")
        };
    }

    private const string PART_PATTERN     = @"{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}";
    private const string WORKFLOW_PATTERN = "([a-z]+){([a-z0-9AR:,<>]+),([a-z]+|A|R)}";

    private const string START    = "in";
    private const string ACCEPTED = "A";
    private const string REJECTED = "R";

    private const int MIN = 1;
    private const int MAX = 4000;

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day19(string input) : base(input.Trim(), options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int total = 0;
        foreach (Part part in this.Data.parts)
        {
            string current;
            for (current = START;
                 current is not ACCEPTED and not REJECTED;
                 current = this.Data.workflows[current].TestPart(part)) { }

            if (current is ACCEPTED)
            {
                total += part.Rating;
            }
        }

        AoCUtils.LogPart1(total);

        Range defaultRange = MIN..MAX;
        PartRange range    = new(defaultRange, defaultRange, defaultRange, defaultRange);
        Workflow start     = this.Data.workflows[START];
        long variations    = CountRangeSize(range, start);
        AoCUtils.LogPart2(variations);
    }

    public long CountRangeSize(PartRange partRange, in Workflow workflow)
    {
        long valid = 0L;
        foreach (Rule rule in workflow.rules)
        {
            (PartRange accepted, partRange) = partRange.SplitRange(rule);
            if (accepted.HasAnyValid)
            {
                valid += rule.target switch
                {
                    ACCEPTED => accepted.ValidCount,
                    REJECTED => 0L,
                    { } next => CountRangeSize(accepted, this.Data.workflows[next]),
                    _        => throw new UnreachableException("Invalid workflow detected")
                };
            }

            if (!partRange.HasAnyValid) return valid;
        }

        valid += workflow.noMatch switch
        {
            ACCEPTED => partRange.ValidCount,
            REJECTED => 0L,
            { } next => CountRangeSize(partRange, this.Data.workflows[next]),
            _        => throw new UnreachableException("Invalid workflow detected")
        };

        return valid;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Dictionary<string, Workflow>, Part[]) Convert(string[] rawInput)
    {
        int separation = rawInput.IndexOf(string.Empty);
        Workflow[] workflows = RegexFactory<Workflow>.ConstructObjects(WORKFLOW_PATTERN, rawInput[..separation++], RegexOptions.Compiled);
        Dictionary<string, Workflow> workflowMap = workflows.ToDictionary(w => w.label, w => w);
        Part[] parts = RegexFactory<Part>.ConstructObjects(PART_PATTERN, rawInput[separation..], RegexOptions.Compiled);
        return (workflowMap, parts);
    }
}
