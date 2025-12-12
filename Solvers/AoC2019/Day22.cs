using System.ComponentModel;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Extensions.Numbers;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 22
/// </summary>
public sealed class Day22 : ArraySolver<Day22.Instruction>
{
    public enum InstructionType
    {
        REVERSE,
        CUT,
        DEAL
    }

    public sealed record Instruction(InstructionType Type, int Value);

    private const int DECK_SIZE1 = 10_007;
    private const int CARD1      = 2019;

    private const long DECK_SIZE2 = 119_315_717_514_047L;
    private const long SHUFFLES   = 101_741_582_076_661;
    private const long CARD2      = 2020L;

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int position = CARD1;
        foreach (Instruction instruction in this.Data)
        {
            position = instruction.Type switch
            {
                InstructionType.REVERSE => DECK_SIZE1 - position - 1,
                InstructionType.CUT     => (position - instruction.Value).Mod(DECK_SIZE1),
                InstructionType.DEAL    => (position * instruction.Value) % DECK_SIZE1,
                _                       => throw new InvalidEnumArgumentException(nameof(instruction.Type), (int)instruction.Type, typeof(InstructionType))
            };
        }
        AoCUtils.LogPart1(position);

        (long a, long b) = (1L, 0L);
        foreach (Instruction instruction in this.Data)
        {
            switch (instruction.Type)
            {
                case InstructionType.REVERSE:
                    a = (-a).Mod(DECK_SIZE2);
                    b = (-b - 1L).Mod(DECK_SIZE2);
                    break;

                case InstructionType.CUT:
                    b = (b - instruction.Value).Mod(DECK_SIZE2);
                    break;

                case InstructionType.DEAL:
                    a = (a * instruction.Value) % DECK_SIZE2;
                    b = (b * instruction.Value) % DECK_SIZE2;
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(instruction.Type), (int)instruction.Type, typeof(InstructionType));
            }
        }

        Int128 inverseA = MathUtils.ModularInverse((Int128)a, DECK_SIZE2);
        Int128 inverseB = (-b * inverseA).Mod(DECK_SIZE2);

        Span2D<Int128> matrix = stackalloc Int128[4].AsSpan2D(2, 2);
        matrix[0, 0] = inverseA;
        matrix[0, 1] = inverseB;
        matrix[1, 0] = 0L;
        matrix[1, 1] = 1L;

        Span2D<Int128> exp = stackalloc Int128[4].AsSpan2D(2, 2);
        MathUtils.Matrix2x2Power(matrix, ref exp, SHUFFLES, DECK_SIZE2);

        Span2D<Int128> card = stackalloc Int128[2].AsSpan2D(2, 1);
        card[0, 0] = CARD2;
        card[1, 0] = 1L;

        Span2D<Int128> result = stackalloc Int128[2].AsSpan2D(2, 1);
        MathUtils.MatrixMultiplication(exp, card, ref result, DECK_SIZE2);
        AoCUtils.LogPart2(result[0, 0]);
    }

    /// <inheritdoc />
    protected override Instruction ConvertLine(string line)
    {
        if (line == "deal into new stack") return new Instruction(InstructionType.REVERSE, 0);
        ReadOnlySpan<char> span = line;
        return span.StartsWith("cut")
                   ? new Instruction(InstructionType.CUT, int.Parse(span[4..]))
                   : new Instruction(InstructionType.DEAL, int.Parse(span[20..]));
    }
}
