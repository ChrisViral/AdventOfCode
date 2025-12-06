using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 18
/// </summary>
public sealed class Day18 : Solver<Day18.DigInstruction[]>
{
    public readonly struct DigInstruction
    {
        public readonly Vector2<int> instruction;
        public readonly Vector2<long> longInstruction;

        public DigInstruction(char direction, int length, string longLength, char longDir)
        {
            this.instruction = Direction.Parse(direction).ToVector(length);
            Direction longDirection = longDir switch
            {
                '0' => Direction.RIGHT,
                '1' => Direction.DOWN,
                '2' => Direction.LEFT,
                '3' => Direction.UP,
                _   => throw new UnreachableException("Unknown direction")
            };
            this.longInstruction = longDirection.ToVector(System.Convert.ToInt64(longLength, 16));
        }

        public override string ToString() => $"{this.instruction} ({this.longInstruction})";
    }

    private const string INSTRUCTION_PATTERN = @"([UDLR]) (\d+) \(#([0-9a-f]{5})([0-3])\)";

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="DigInstruction"/>[] fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int area = CalculateShapeSize(this.Data.Select(d => d.instruction));
        AoCUtils.LogPart1(area);

        long longArea = CalculateShapeSize(this.Data.Select(d => d.longInstruction));
        AoCUtils.LogPart2(longArea);
    }

    public T CalculateShapeSize<T>(IEnumerable<Vector2<T>> verticesInstructions) where T : IBinaryInteger<T>, IMinMaxValue<T>
    {
        Vector2<T> current = Vector2<T>.Zero;
        List<Vector2<T>> vertices = new(this.Data.Length + 1) { current };

        T perimeter = T.Zero;
        foreach (Vector2<T> instruction in verticesInstructions)
        {
            current += instruction;
            vertices.Add(current);
            perimeter += T.Abs(instruction.X + instruction.Y);
        }

        T interior = MathUtils.Shoelace(vertices);
        return MathUtils.Picks(interior, perimeter);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override DigInstruction[] Convert(string[] rawInput) => RegexFactory<DigInstruction>.ConstructObjects(INSTRUCTION_PATTERN, rawInput, RegexOptions.Compiled);
}
