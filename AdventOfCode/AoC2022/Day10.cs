using AdventOfCode.Collections;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 10
/// </summary>
public sealed class Day10 : ArraySolver<(Day10.Operation op, int arg)>
{
    // ReSharper disable once IdentifierTypo
    public enum Operation
    {
        NOOP,
        ADDX
    }

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver for 2022 - 10 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day10(string input) : base(input) { }

    /// <summary>
    /// X register
    /// </summary>
    private int X { get; set; } = 1;

    /// <summary>
    /// Current clock cycle
    /// </summary>
    private int Cycle { get; set; }

    /// <summary>
    /// Cycles signal sum
    /// </summary>
    private int CyclesSum { get; set; }

    /// <summary>
    /// CRT position
    /// </summary>
    private Vector2<int> Position { get; set; }

    /// <summary>
    /// CRT grid
    /// </summary>
    private Grid<bool> Crt { get; } = new(40, 6, v => v ? "█" : " ");

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        foreach ((Operation op, int arg) in this.Data)
        {
            ProcessCycle();
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (op)
            {
                case Operation.NOOP:
                    // Do nothing
                    break;

                case Operation.ADDX:
                    ProcessCycle();
                    this.X += arg;
                    break;
            }
        }

        AoCUtils.LogPart1(this.CyclesSum);
        AoCUtils.LogPart2(string.Empty);
        AoCUtils.Log(this.Crt);
    }

    /// <inheritdoc />
    protected override (Operation, int) ConvertLine(string line)
    {
        string[] splits = line.Split(' ', DEFAULT_OPTIONS);
        Operation op = Enum.Parse<Operation>(splits[0], true);
        if (splits.Length <= 1 || !int.TryParse(splits[1], out int arg))
        {
            arg = 0;
        }

        return new ValueTuple<Operation, int>(op, arg);
    }

    private void ProcessCycle()
    {
        // Check if the sum must be updated
        if ((++this.Cycle + 20).IsMultiple(40))
        {
            this.CyclesSum += this.X * this.Cycle;
        }

        // Update the CRT value
        this.Crt[this.Position] = this.Position.X >= this.X - 1
                               && this.Position.X <= this.X + 1;

        // Update the position
        this.Position = this.Cycle.IsMultiple(40)
                            ? new Vector2<int>(0, this.Position.Y + 1)
                            : new Vector2<int>(this.Position.X + 1, this.Position.Y);
    }
}
