using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 15
/// </summary>
public class Day15 : Solver<Day15.Instruction[]>
{
    public enum Operation
    {
        REMOVE = '-',
        INSERT = '='
    }

    public readonly struct Instruction(string code, char operation, int strength)
    {
        public readonly string code         = code;
        public readonly Operation operation = (Operation)operation;
        public readonly int strength        = strength;

        public Instruction(string code, char operation) : this(code, operation, -1) { }

        public override string ToString() => this.operation is Operation.INSERT ? $"{code}={this.strength}" : $"{code}-";
    }

    public record struct Lens(string Label, int Strength);

    private const int BOXES = 256;
    private const string INSTRUCTION_PATTERN = @"([a-z]+)(=|-)(\d)?";

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Instruction"/>[] fails</exception>
    public Day15(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int total = this.Data.Select(i => i.ToString()).Sum(HashCode);
        AoCUtils.LogPart1(total);

        List<Lens>[] boxes = new List<Lens>[BOXES];
        boxes.Fill(() => []);

        foreach (Instruction instruction in this.Data)
        {
            int hash = HashCode(instruction.code);
            List<Lens> box = boxes[hash];
            int i = box.FindIndex(l => l.Label == instruction.code);
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (instruction.operation)
            {
                case Operation.REMOVE when i is not -1:
                    box.RemoveAt(i);
                    break;

                case Operation.INSERT when i is -1:
                    box.Add(new(instruction.code, instruction.strength));
                    break;

                case Operation.INSERT:
                    box[i] = new(instruction.code, instruction.strength);
                    break;
            }
        }

        int power = 0;
        foreach (int boxNumber in 1..^BOXES)
        {
            List<Lens> box = boxes[boxNumber - 1];
            if (box.IsEmpty()) continue;

            foreach (int slotNumber in 1..^box.Count)
            {
                power += boxNumber * slotNumber * box[slotNumber - 1].Strength;
            }
        }

        AoCUtils.LogPart2(power);
    }

    public int HashCode(string code)
    {
        int hash = 0;
        foreach (char c in code)
        {
            HashValue(ref hash, c);
        }
        return hash;
    }

    public void HashValue(ref int hash, char c)
    {
        hash += c;
        hash *= 17;
        hash %= BOXES;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Instruction[] Convert(string[] rawInput) => RegexFactory<Instruction>.ConstructObjects(INSTRUCTION_PATTERN,
                                                                                                              rawInput[0].Split(','),
                                                                                                              RegexOptions.Compiled);
    #endregion
}