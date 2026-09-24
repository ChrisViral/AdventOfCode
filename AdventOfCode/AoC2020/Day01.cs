using Challenge.Solvers;
using Challenge.Utils.Extensions.Arrays;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 1
/// </summary>
[Solver(2020, 1)]
public sealed class Day01 : Solver<int[]>
{
    /// <summary>
    /// Target total
    /// </summary>
    private const int TARGET = 2020;

    private HashSet<int> values = [];

    /// <inheritdoc />
    public override void ParseInput(string input)
    {
        base.ParseInput(input);
        this.values = [..this.Data];
    }

    /// <inheritdoc />
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
                LogAnswer(expense * match);
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
                    LogAnswer(first * second * third);
                    return;
                }
            }
        }
    }
}
