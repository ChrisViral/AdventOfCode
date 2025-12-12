using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 01
/// </summary>
public sealed class Day01 : Solver<int[]>
{
    /// <summary>
    /// Target total
    /// </summary>
    private const int TARGET = 2020;

    private readonly HashSet<int> values;

    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input) : base(input) => this.values = [..this.Data];

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        FindTwoMatching();
        FindThreeMatching();
    }

    ///<inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput.ConvertAll(int.Parse);

    /// <summary>
    /// First part solving
    /// </summary>
    private void FindTwoMatching()
    {
        foreach (int expense in this.Data)
        {
            int match = TARGET - expense;
            if (this.values.Contains(match))
            {
                AoCUtils.LogPart1(expense * match);
                return;
            }
        }
    }

    /// <summary>
    /// Second part solving
    /// </summary>
    private void FindThreeMatching()
    {
        this.Data.Sort();
        for (int i = 0; i < this.Data.Length - 2; /*i++*/)
        {
            int first = this.Data[i];
            foreach (int second in this.Data[++i..^1])
            {
                int total = first + second;

                if (total >= TARGET)
                {
                    break;
                }

                int third = TARGET - total;
                if (this.values.Contains(third))
                {
                    AoCUtils.LogPart2(first * second * third);
                    return;
                }
            }
        }
    }
}
