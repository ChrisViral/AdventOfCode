using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 14
/// </summary>
public partial class Day14 : Solver<Day14.Chemical>
{
    /// <summary>
    /// Reaction reactant
    /// </summary>
    /// <param name="Chemical">Chemical reacted</param>
    /// <param name="Amount">Amount required</param>
    public readonly record struct Reactant(Chemical Chemical, int Amount)
    {
        /// <summary>
        /// String representation of this reactant
        /// </summary>
        /// <returns>String representation of this reactant</returns>
        public override string ToString() => $"{this.Amount} {this.Chemical.Name}";
    }

    /// <summary>
    /// Chemical product
    /// </summary>
    /// <param name="name">Product name</param>
    public sealed class Chemical(string name) : IEquatable<Chemical>
    {
        /// <summary>
        /// Product name
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Recipe to make product
        /// </summary>
        public Reactant[] Recipe { get; set; } = [];

        /// <summary>
        /// Recipe production amount
        /// </summary>
        public int Produced { get; set; }

        /// <summary>
        /// How much of this item needs to be produced
        /// </summary>
        public int RequiredProduction { get; set; }

        /// <summary>
        /// How much byproduct of this item currently exists
        /// </summary>
        public int ByproductCount { get; set; }

        /// <summary>
        /// Consumes byproducts before production
        /// </summary>
        /// <returns><see langword="true"/> if enough byproducts were consumed to satisfy the entire production, otherwise <see langword="false"/></returns>
        public bool ConsumeByproducts()
        {
            // If there's any byproducts
            if (this.ByproductCount > 0)
            {
                // If there are more byproducts than must be produced
                if (this.ByproductCount > this.RequiredProduction)
                {
                    // Decrease byproducts, set production to 0, cancel further production
                    this.ByproductCount    -= this.RequiredProduction;
                    this.RequiredProduction = 0;
                    return true;
                }

                // Decrease required production and consume all byproducts
                this.RequiredProduction -= this.ByproductCount;
                this.ByproductCount      = 0;
            }
            return false;
        }

        /// <inheritdoc />
        public bool Equals(Chemical? other) => this.Name == other?.Name;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Chemical other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Name.GetHashCode();

        /// <summary>
        /// String representation of this chemical
        /// </summary>
        /// <returns>String representation of this chemical</returns>
        public override string ToString() => $"{string.Join(", ", this.Recipe)} => {this.Produced} {this.Name}";
    }

    /// <summary>
    /// Ore chemical name
    /// </summary>
    private const string ORE = "ORE";
    /// <summary>
    /// Total ore cargo
    /// </summary>
    private const long CARGO = 1_000_000_000_000L;

    /// <summary>
    /// Chemical reaction regex
    /// </summary>
    [GeneratedRegex(@"^([\dA-Z, ]+) => ([\dA-Z ]+)$", RegexOptions.Compiled)]
    private static partial Regex ReactionRegex { get; }

    /// <summary>
    /// Reagent regex
    /// </summary>
    [GeneratedRegex(@"(\d+) ([A-Z]+)", RegexOptions.Compiled)]
    private static partial Regex ReagentRegex { get; }

    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day14(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup
        Queue<Chemical> productionQueue = new(100);
        int oreRequired = ProduceOneFuel(productionQueue);
        AoCUtils.LogPart1(oreRequired);

        // Setup to churn numbers
        int fuelProduced  = 1;
        long oreRemaining = CARGO - oreRequired;

        // Ugly but runs in about five seconds
        while (oreRemaining > 0L)
        {
            oreRemaining -= ProduceOneFuel(productionQueue);
            fuelProduced++;
        }

        // If we have negative ore remaining, we overproduced by one
        if (oreRemaining < 0L)
        {
            fuelProduced--;
        }

        AoCUtils.LogPart2(fuelProduced);
    }

    /// <summary>
    /// Produces one unit of fuel
    /// </summary>
    /// <param name="productionQueue">Production queue</param>
    /// <returns>The ore consumed to produce the unit of fuel</returns>
    /// ReSharper disable once CognitiveComplexity
    private int ProduceOneFuel(Queue<Chemical> productionQueue)
    {
        // Setup
        int oreRequired = 0;
        this.Data.RequiredProduction = 1;
        productionQueue.Enqueue(this.Data);

        // Produce until we run out of stuff to make
        while (productionQueue.TryDequeue(out Chemical? product))
        {
            // Check for byproducts
            if (product.ConsumeByproducts()) continue;

            // Check how many times we need to run the recipe
            int productions = ((product.RequiredProduction - 1) / product.Produced) + 1;
            int leftover = (productions * product.Produced) - product.RequiredProduction;

            // Add byproducts
            product.ByproductCount     += leftover;
            product.RequiredProduction =  0;

            // Add required reactants to queue
            foreach (Reactant reactant in product.Recipe)
            {
                // If reactant is ore, just add amount
                int toProduce = reactant.Amount * productions;
                if (reactant.Chemical.Name is ORE)
                {
                    oreRequired += toProduce;
                    break;
                }

                if (reactant.Chemical.RequiredProduction is 0)
                {
                    // If there's isn't an amount awaiting to be produced, add to queue if more need to be produced
                    reactant.Chemical.RequiredProduction = toProduce;
                    productionQueue.Enqueue(reactant.Chemical);
                }
                else
                {
                    //Add required production amount
                    reactant.Chemical.RequiredProduction += toProduce;
                }
            }
        }
        return oreRequired;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Chemical Convert(string[] rawInput)
    {
        Dictionary<string, Chemical> chemicals = new(rawInput.Length);
        var chemicalsLookup = chemicals.GetAlternateLookup<ReadOnlySpan<char>>();
        foreach (string line in rawInput)
        {
            Match reactionMatch   = ReactionRegex.Match(line);

            // Match the product
            string productString    = reactionMatch.Groups[2].Value;
            Match productMatch      = ReagentRegex.Match(productString);
            int amount              = int.Parse(productMatch.Groups[1].ValueSpan);
            ReadOnlySpan<char> name = productMatch.Groups[2].ValueSpan;

            // Try and get product chemical
            if (!chemicalsLookup.TryGetValue(name, out Chemical? chemical))
            {
                chemical                 = new Chemical(name.ToString());
                chemicals[chemical.Name] = chemical;
            }

            // Setup product recipe
            chemical.Produced = amount;

            if (chemical.Recipe.Length is not 0) throw new UnreachableException();

            // Get reactants list
            string reactantString = reactionMatch.Groups[1].Value;
            MatchCollection reactantMatches = ReagentRegex.Matches(reactantString);
            Reactant[] reactants = new Reactant[reactantMatches.Count];
            chemical.Recipe = reactants;

            // Fetch reactants
            foreach (int i in ..reactants.Length)
            {
                // Get amount and name
                Match reactantMatch = reactantMatches[i];
                amount = int.Parse(reactantMatch.Groups[1].ValueSpan);
                name = reactantMatch.Groups[2].ValueSpan;

                // Try and get chemical
                if (!chemicalsLookup.TryGetValue(name, out chemical))
                {
                    chemical = new Chemical(name.ToString());
                    chemicals[chemical.Name] = chemical;
                }

                // Store reactant
                reactants[i] = new Reactant(chemical, amount);
            }
        }


        return chemicals["FUEL"];
    }
}
