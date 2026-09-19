using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 17
/// </summary>
[Solver(2015, 17)]
public sealed class Day17 : ArraySolver<int>
{
    private const int TARGET_AMOUNT = 150;

    /// <summary>
    /// Creates a new <see cref="Day17"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day17(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int possibilities = PossibleCombinations(0, 0, TARGET_AMOUNT);
        LogAnswer(possibilities);

        Counter<int> uses = new(this.Data.Length);
        MinimalCombinations(0, 0, 0, TARGET_AMOUNT, uses);
        int minUses = uses.AsDictionary().MinBy(p => p.Key).Value;
        LogAnswer(minUses);
    }

    private int PossibleCombinations(int containerIndex, int amountUsed, int targetAmount)
    {
        int possibilities = 0;
        int containerAmount = this.Data[containerIndex];
        int newAmountUsed = amountUsed + containerAmount;
        if (newAmountUsed == targetAmount)
        {
            possibilities++;
        }

        int nextContainerIndex = containerIndex + 1;
        if (nextContainerIndex == this.Data.Length) return possibilities;

        possibilities += PossibleCombinations(nextContainerIndex, amountUsed, targetAmount);
        if (newAmountUsed < targetAmount)
        {
            possibilities += PossibleCombinations(nextContainerIndex, newAmountUsed, targetAmount);
        }
        return possibilities;
    }

    private void MinimalCombinations(int containerIndex, int amountUsed, int containersUsed, int targetAmount, Counter<int> uses)
    {
        int containerAmount = this.Data[containerIndex];
        int newAmountUsed = amountUsed + containerAmount;
        if (newAmountUsed == targetAmount)
        {
            uses.Add(containersUsed + 1);
        }

        int nextContainerIndex = containerIndex + 1;
        if (nextContainerIndex == this.Data.Length) return;

        MinimalCombinations(nextContainerIndex, amountUsed, containersUsed, targetAmount, uses);
        if (newAmountUsed < targetAmount)
        {
            MinimalCombinations(nextContainerIndex, newAmountUsed, containersUsed + 1, targetAmount, uses);
        }
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
