using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Maths;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 18
/// </summary>
public sealed partial class Day18 : Solver<Day18.DigInstruction[]>
{
    public readonly struct DigInstruction
    {
        public readonly Vector2<int> instruction;
        public readonly Vector2<long> longInstruction;

        public DigInstruction(char direction, int length, string longLength, char longDir)
        {
            this.instruction = Direction.ParseDirection(direction).ToVector(length);
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

    [GeneratedRegex(@"([UDLR]) (\d+) \(#([0-9a-f]{5})([0-3])\)")]
    private static partial Regex InstructionMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="DigInstruction"/>[] fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int area = CalculateShapeSize(this.Data.Select(d => d.instruction));
        AoCUtils.LogPart1(area);

        long longArea = CalculateShapeSize(this.Data.Select(d => d.longInstruction));
        AoCUtils.LogPart2(longArea);
    }

    public T CalculateShapeSize<T>(IEnumerable<Vector2<T>> verticesInstructions) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
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

    /// <inheritdoc />
    protected override DigInstruction[] Convert(string[] rawInput) => RegexFactory<DigInstruction>.ConstructObjects(InstructionMatcher, rawInput);
}
