using System.ComponentModel;
using System.Runtime.InteropServices;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 17
/// </summary>
public sealed class Day17 : Solver<(long a, long b, long c, int[] program)>
{
    private enum Opcode
    {
        ADV = 0, // A   <- A / 2^Op
        BXL = 1, // B   <- B Xor Lit
        BST = 2, // B   <- Op Mod 8
        JNZ = 3, // A != 0 => Jump Lit
        BXC = 4, // B   <- B Xor C
        OUT = 5, // Out <- B Mod 8
        BDV = 6, // B   <- A / 2^Op
        CDV = 7  // C   <- A / 2^Op
    }

    private readonly List<int> output = new(16);

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Run with initial params
        RunProgram(this.Data.a, this.Data.b, this.Data.c);
        AoCUtils.LogPart1(string.Join(',', this.output));

        // Search from 0
        long minInitialA = long.MaxValue;
        Queue<(int chunkSize, long initialA)> search = new();
        search.Enqueue((1, 0L));
        while (search.TryDequeue(out (int chunkSize, long initialA) test))
        {
            // Try matching only last few values from output
            Index startIndex = ^test.chunkSize;
            ReadOnlySpan<int> programChunk = this.Data.program.AsSpan(startIndex);
            // Test eight options from current initial A value
            foreach (int offset in ..8)
            {
                // Run program with specified parameters
                long initialA = test.initialA + offset;
                RunProgram(initialA, this.Data.b, this.Data.c);
                if (this.output.Count < test.chunkSize) continue;

                // Check if the last output chunk matches
                ReadOnlySpan<int> outputChunk = CollectionsMarshal.AsSpan(this.output)[startIndex..];
                if (!programChunk.SequenceEqual(outputChunk)) continue;

                if (test.chunkSize == this.Data.program.Length)
                {
                    // If we have the same total output size, we have a candidate
                    minInitialA = Math.Min(minInitialA, initialA);
                }
                else
                {
                    // Else increase chunk size and add to search queue
                    search.Enqueue((test.chunkSize + 1, initialA * 8L));
                }
            }
        }
        AoCUtils.LogPart2(minInitialA);
    }

    // ReSharper disable once CognitiveComplexity
    private void RunProgram(long a, long b, long c)
    {
        long GetOperand(long operand) => operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4                => a,
            5                => b,
            6                => c,
            7                => throw new NotSupportedException("Combo operator 7 is not supported"),
            _                => throw new InvalidOperationException("Invalid combo operator")
        };

        this.output.Clear();
        for (int ip = 0; ip < this.Data.program.Length; /* ip += 2 */)
        {
            Opcode opcode = (Opcode)this.Data.program[ip++];
            int operand   = this.Data.program[ip++];
            switch (opcode)
            {
                case Opcode.ADV:
                    a >>= (int)GetOperand(operand);
                    break;

                case Opcode.BXL:
                    b ^= operand;
                    break;

                case Opcode.BST:
                    b = GetOperand(operand) % 8L;
                    break;

                case Opcode.JNZ when a is not 0L:
                    ip = operand;
                    break;

                case Opcode.JNZ:
                    break;

                case Opcode.BXC:
                    b ^= c;
                    break;

                case Opcode.OUT:
                    this.output.Add((int)(GetOperand(operand) % 8L));
                    break;

                case Opcode.BDV:
                    b = a >> (int)GetOperand(operand);
                    break;

                case Opcode.CDV:
                    c = a >> (int)GetOperand(operand);
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(opcode), (int)opcode, typeof(Opcode));
            }
        }
    }

    /// <inheritdoc />
    protected override (long, long, long, int[]) Convert(string[] rawInput)
    {
        // Registers
        long a = long.Parse(rawInput[0].AsSpan(12));
        long b = long.Parse(rawInput[1].AsSpan(12));
        long c = long.Parse(rawInput[2].AsSpan(12));

        // Bytecode
        ReadOnlySpan<char> programSpan = rawInput[3].AsSpan(9);
        int[] program = new int[(programSpan.Length + 1) / 2];
        foreach (int i in ..program.Length)
        {
            program[i] = programSpan[i * 2] - '0';
        }
        return (a, b, c, program);
    }
}
