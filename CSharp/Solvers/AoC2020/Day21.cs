using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 21
/// </summary>
public sealed class Day21 : Solver<Day21.IngredientList[]>
{
    /// <summary>
    /// Ingredient list
    /// </summary>
    public sealed class IngredientList
    {
            /// <summary>
        /// Ingredient list regex pattern
        /// </summary>
        public const string PATTERN = @"([a-z ]+) \(contains ([a-z, ]+)\)";

            /// <summary>
        /// Ingredients
        /// </summary>
        public HashSet<string> Ingredients { get; }
        /// <summary>
        /// Possible allergens
        /// </summary>
        public HashSet<string> Allergens { get; }

            /// <summary>
        /// Creates a new IngredientList from the given ingredients and allergens
        /// </summary>
        /// <param name="ingredients">Space separated ingredients</param>
        /// <param name="allergens">Comma separated allergens</param>
        public IngredientList(string ingredients, string allergens)
        {
            this.Ingredients = new HashSet<string>(ingredients.Split(' '));
            this.Allergens = new HashSet<string>(allergens.Split(", "));
        }
        }

    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IngredientList"/>[] fails</exception>
    public Day21(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Get all existing ingredients and allergens
        Dictionary<string, int> ingredientCount = new();
        HashSet<string> allergens = [];
        foreach (IngredientList list in this.Data)
        {
            //Loop through list ingredients
            foreach (string ingredient in list.Ingredients)
            {
                //Keep track of how many times we've seen the ingredient
                ingredientCount.TryGetValue(ingredient, out int amount);
                ingredientCount[ingredient] = amount + 1;
            }

            //Add all children allergens
            allergens.UnionWith(list.Allergens);
        }

        //Get impossible ingredients
        HashSet<string> impossible = new(ingredientCount.Keys);
        Dictionary<string, HashSet<string>> possibilities = new();
        foreach (string allergen in allergens)
        {
            //Remove all ingredients not present every time
            HashSet<string> ingredients = new(ingredientCount.Keys);
            foreach (IngredientList list in this.Data.Where(l => l.Allergens.Contains(allergen)))
            {
                ingredients.IntersectWith(list.Ingredients);
            }
            //Remove possible ingredients from impossible list
            impossible.ExceptWith(ingredients);
            possibilities.Add(allergen, ingredients);
        }
        AoCUtils.LogPart1(impossible.Sum(i => ingredientCount[i]));

        //Get definitive allergens
        SortedDictionary<string, string> sortedAllergens = new();
        while (!possibilities.IsEmpty)
        {
            //Get first known allergen
            (string allergen, HashSet<string> ingredients) = possibilities.First(p => p.Value.Count is 1);
            possibilities.Remove(allergen);
            string ingredient = ingredients.First();
            //Add to final sorted list and remove from other lists
            sortedAllergens.Add(allergen, ingredient);
            possibilities.ForEach(p => p.Value.Remove(ingredient));
        }
        AoCUtils.LogPart2(string.Join(',', sortedAllergens.Values));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override IngredientList[] Convert(string[] rawInput) => RegexFactory<IngredientList>.ConstructObjects(IngredientList.PATTERN, rawInput, RegexOptions.Compiled);
}
