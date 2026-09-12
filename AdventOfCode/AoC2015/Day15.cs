using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 15
/// </summary>
public sealed partial class Day15 : RegexSolver<Day15.Ingredient>
{
    public readonly record struct Ingredient(string Name, int Capacity, int Durability, int Flavour, int Texture, int Calories);

    private const int TOTAL_AVAILABLE = 100;
    private const int CALORY_TARGET = 500;

    /// <inheritdoc />
    [GeneratedRegex(@"(\w+): capacity (-?\d), durability (-?\d), flavor (-?\d), texture (-?\d), calories (-?\d)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day15"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day15(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<int> recipe = stackalloc int[this.Data.Length];
        int score = FindBestRecipe(0, recipe);
        AoCUtils.LogPart1(score);

        score = FindBestRecipe(0, recipe, CALORY_TARGET);
        AoCUtils.LogPart2(score);
    }

    private int FindBestRecipe(int ingredientIndex, Span<int> recipe, int? caloryTarget = null)
    {
        int available = TOTAL_AVAILABLE - recipe.Sum();
        if (available is 0)
        {
            (int score, int calories) = CalculateScore(recipe);
            return caloryTarget is null || calories == caloryTarget ? score : 0;
        }
        if (ingredientIndex == this.Data.Length - 1)
        {
            recipe[ingredientIndex] = available;
            (int score, int calories) = CalculateScore(recipe);
            recipe[ingredientIndex] = 0;
            return caloryTarget is null || calories == caloryTarget ? score : 0;
        }

        int bestScore = 0;
        foreach (int amount in ..^available)
        {
            recipe[ingredientIndex] = amount;
            int score = FindBestRecipe(ingredientIndex + 1, recipe, caloryTarget);
            bestScore = Math.Max(bestScore, score);
        }

        recipe[ingredientIndex] = 0;
        return bestScore;
    }

    private (int score, int calories) CalculateScore(Span<int> recipe)
    {
        int capacity   = 0;
        int durability = 0;
        int flavour    = 0;
        int texture    = 0;
        int calories   = 0;
        foreach (int i in ..recipe.Length)
        {
            Ingredient ingredient = this.Data[i];
            int amount  = recipe[i];
            capacity   += ingredient.Capacity * amount;
            durability += ingredient.Durability * amount;
            flavour    += ingredient.Flavour * amount;
            texture    += ingredient.Texture * amount;
            calories   += ingredient.Calories * amount;
        }

        int score = Math.Max(0, capacity)
                  * Math.Max(0, durability)
                  * Math.Max(0, flavour)
                  * Math.Max(0, texture);
        return (score, calories);
    }
}
