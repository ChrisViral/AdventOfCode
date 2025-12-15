using System.Text;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 14
/// </summary>
public sealed class Day14 : Solver<int>
{
    private const int PART1_SIZE = 10;

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        StringBuilder recipes = new(1000);
        recipes.Append("37");
        int firstIndex  = 0;
        int secondIndex = 1;
        do
        {
            GenerateRecipes(recipes, ref firstIndex, ref secondIndex);
        }
        while (recipes.Length < (this.Data + PART1_SIZE));

        string score = recipes.ToString(this.Data, PART1_SIZE);
        AoCUtils.LogPart1(score);

        Span<char> value = stackalloc char[this.Data.DigitCount];
        Span<char> test = stackalloc char[value.Length + 1];
        this.Data.TryFormat(value, out _);
        int matchIndex;
        int testStart;
        do
        {
            int added = GenerateRecipes(recipes, ref firstIndex, ref secondIndex);
            int testLength = value.Length + added - 1;
            testStart = recipes.Length - testLength;
            recipes.CopyTo(testStart, test, testLength);
            matchIndex = test[..testLength].IndexOf(value, StringComparison.Ordinal);
        }
        while (matchIndex is -1);
        AoCUtils.LogPart2(testStart + matchIndex);
    }

    private static int GenerateRecipes(StringBuilder recipes, ref int firstIndex, ref int secondIndex)
    {
        // Generate new recipe
        int firstValue  = recipes[firstIndex]  - '0';
        int secondValue = recipes[secondIndex] - '0';
        int createdRecipe = firstValue + secondValue;

        int added;
        if (createdRecipe >= 10)
        {
            recipes.Append('1').Append((char)((createdRecipe % 10) + '0'));
            added = 2;
        }
        else
        {
            recipes.Append((char)(createdRecipe + '0'));
            added = 1;
        }

        // Update indices
        firstIndex = (firstIndex + firstValue + 1) % recipes.Length;
        secondIndex = (secondIndex + secondValue + 1) % recipes.Length;
        return added;
    }

    /// <inheritdoc />
    protected override int Convert(string[] rawInput) => int.Parse(rawInput[0]);
}
