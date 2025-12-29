using System.Text.RegularExpressions;
using AdventOfCode.AoC2018.ElfCode;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Ranges;
using FastEnumUtility;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 16
/// </summary>
public sealed partial class Day16 : Solver<(Day16.Sample[] samples, Instruction[] program)>
{
    public sealed record Sample(in Registers Before, in Registers After, in Instruction Instruction);

    private const int OPCODE_COUNT = 16;

    [GeneratedRegex(@"\[(\d), (\d), (\d), (\d)\]")]
    private static partial Regex RegistersMatcher { get; }

    [GeneratedRegex(@"(1?\d) (\d) (\d) (\d)")]
    private static partial Regex InstructionMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Keep track of possible opcodes per value
        HashSet<Opcode>[] possibleOpcodes = new HashSet<Opcode>[OPCODE_COUNT];
        possibleOpcodes.Fill(() => [..FastEnum.GetValues<Opcode>()]);

        int threefold = 0;
        foreach (Sample sample in this.Data.samples)
        {
            int possibleCount = 0;
            HashSet<Opcode> possible = possibleOpcodes[(int)sample.Instruction.Opcode];
            foreach (Opcode opcode in FastEnum.GetValues<Opcode>())
            {
                // Run instruction
                Registers sampleRegisters = sample.Before;
                VirtualMachine.RunInstruction(sample.Instruction with { Opcode = opcode }, ref sampleRegisters);
                if (sampleRegisters == sample.After)
                {
                    // Valid, increment
                    possibleCount++;
                }
                else
                {
                    // Invalid, remove from possibilities
                    possible.Remove(opcode);
                }
            }

            // More than three possibilities, increment
            if (possibleCount >= 3)
            {
                threefold++;
            }

        }
        AoCUtils.LogPart1(threefold);

        // Create final opcode map
        Opcode[] opcodeMap = new Opcode[OPCODE_COUNT];
        foreach (int _ in ..opcodeMap.Length)
        {
            foreach (int i in ..possibleOpcodes.Length)
            {
                HashSet<Opcode> possible = possibleOpcodes[i];
                if (possible.Count is 1)
                {
                    // If only one is possible, map it
                    Opcode opcode = possible.First();
                    opcodeMap[i] = opcode;
                    // Then remove it from others possibilities
                    possibleOpcodes.ForEach(p => p.Remove(opcode));
                    break;
                }
            }
        }

        // Run program
        Registers registers = new();
        foreach (Instruction instruction in this.Data.program)
        {
            Opcode opcode = opcodeMap[(int)instruction.Opcode];
            VirtualMachine.RunInstruction(instruction with { Opcode = opcode }, ref registers);
        }
        AoCUtils.LogPart2(registers[0]);
    }

    /// <inheritdoc />
    protected override (Sample[], Instruction[]) Convert(string[] rawInput)
    {
        RegexFactory<Registers> registersFactory     = new(RegistersMatcher);
        RegexFactory<Instruction> instructionFactory = new(InstructionMatcher);
        List<Sample> samples = new(rawInput.Length / 3);
        int i;
        for (i = 0; rawInput[i][0] is 'B'; i += 3)
        {
            Sample sample = new(registersFactory.ConstructObject(rawInput[i]),
                                registersFactory.ConstructObject(rawInput[i + 2]),
                                instructionFactory.ConstructObject(rawInput[i + 1]));
            samples.Add(sample);
        }

        Instruction[] program = instructionFactory.ConstructObjects(rawInput[i..]);
        return (samples.ToArray(), program);
    }
}
