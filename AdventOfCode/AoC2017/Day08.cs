using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 08
/// </summary>
public sealed partial class Day08 : RegexSolver<Day08.Instruction>
{
    public enum Change
    {
        INC,
        DEC
    }

    public enum Condition
    {
        LESS,
        GREATER,
        LESS_EQUALS,
        GREATER_EQUALS,
        EQUALS,
        NOT_EQUALS
    }

    [DebuggerDisplay("{Register} {Change} {Value} if {ConditionRegister} {Condition} {ConditionValue}")]
    public sealed class Instruction(string register, Change change, int value, string conditionRegister, string condition, int conditionValue)
    {
        public string Register { get; } = register;

        public Change Change { get; } = change;

        public int Value { get; } = value;

        public string ConditionRegister { get; } = conditionRegister;

        public Condition Condition { get; } = condition switch
        {
            "<"  => Condition.LESS,
            ">"  => Condition.GREATER,
            "<=" => Condition.LESS_EQUALS,
            ">=" => Condition.GREATER_EQUALS,
            "==" => Condition.EQUALS,
            "!=" => Condition.NOT_EQUALS,
            _    => throw new ArgumentException("Unknown condition type", nameof(condition))
        };

        public int ConditionValue { get; } = conditionValue;

        public bool Execute(Counter<string> registers)
        {
            int conditionRegister = registers[this.ConditionRegister];
            bool conditionValid = this.Condition switch
            {
                Condition.LESS           => conditionRegister < this.ConditionValue,
                Condition.GREATER        => conditionRegister > this.ConditionValue,
                Condition.LESS_EQUALS    => conditionRegister <= this.ConditionValue,
                Condition.GREATER_EQUALS => conditionRegister >= this.ConditionValue,
                Condition.EQUALS         => conditionRegister == this.ConditionValue,
                Condition.NOT_EQUALS     => conditionRegister != this.ConditionValue,
                _                        => throw this.Condition.Invalid()
            };

            if (!conditionValid) return false;

            switch (this.Change)
            {
                case Change.INC:
                    registers[this.Register] += this.Value;
                    return true;

                case Change.DEC:
                    registers[this.Register] -= this.Value;
                    return true;

                default:
                    throw this.Change.Invalid();
            }
        }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"([a-z]+) (inc|dec) (-?\d+) if ([a-z]+) (<|>|<=|>=|==|!=) (-?\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc />
    public override void Run()
    {
        int max = int.MinValue;
        Counter<string> registers = new(100);
        foreach (Instruction instruction in this.Data)
        {
            if (instruction.Execute(registers))
            {
                max = Math.Max(max, registers[instruction.Register]);
            }
        }
        int maxRegister = registers.Counts.Max();
        AoCUtils.LogPart1(maxRegister);
        AoCUtils.LogPart2(max);
    }
}
