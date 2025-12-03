using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day dd
/// </summary>
public class Day14 : Solver<Dictionary<string, Day14.Chemical>>
{
    /// <summary>
    /// Reactant record, chemical and amount required
    /// </summary>
    public record Reactant(Chemical Chemical, int Amount);

    /// <summary>
    /// Chemical compound, with all it's required reactants
    /// </summary>
    public class Chemical : IEquatable<Chemical>
    {
        #region Properties
        /// <summary>
        /// Name of the Chemical
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Amount produced when created
        /// </summary>
        public int Produced { get; }

        /// <summary>
        /// Required reactants
        /// </summary>
        public Reactant[] Reactants { get; set; } = Array.Empty<Reactant>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Chemical as specified
        /// </summary>
        /// <param name="name">Name of the chemical</param>
        /// <param name="produced">Amount produced on reaction</param>
        public Chemical(string name, int produced)
        {
            this.Name = name;
            this.Produced = produced;
        }
        #endregion

        #region Methods
        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object? obj) => obj is Chemical chemical && Equals(chemical);

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(Chemical? other) => this.Name == other?.Name;

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => this.Name.GetHashCode();

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Name;
        #endregion
    }

    #region Constants
    /// <summary>
    /// Full reaction pattern
    /// </summary>
    private const string REACTION_PATTERN = @"^([A-Z\d, ]+) => (\d+) ([A-Z]+)$";
    /// <summary>
    /// Reactant specific pattern
    /// </summary>
    private const string REACTANTS_PATTERN = @"(\d+) ([A-Z]+)";
    /// <summary>
    /// Fuel chemical name
    /// </summary>
    private const string FUEL = "FUEL";
    /// <summary>
    /// Ore chemical name
    /// </summary>
    private const string ORE = "ORE";
    /// <summary>
    /// Total ore in the cargo
    /// </summary>
    private const long CARGO = 1_000_000_000_000L;
    #endregion

    #region Fields
    private readonly Dictionary<Chemical, int> toProduce = new();
    private readonly Dictionary<Chemical, int> surplus = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day14"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Dictionary{TKey,TValue}"/> fails</exception>
    public Day14(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Produce one fuel
        int ore = ProduceOneFuel();
        AoCUtils.LogPart1(ore);

        //Produce until no cargo left
        //I know this is bad but honestly this problem broke me
        //and I just want to get it over with. Maybe I'll revisit
        //it another time.
        int produced = 0;
        long cargo = CARGO - ore;
        long countdown = 900000000000L;
        while (cargo > 0L)
        {
            if (cargo < countdown)
            {
                Console.WriteLine("Currently at " + cargo);
                countdown -= 100000000000L;
            }
            produced++;
            cargo -= ProduceOneFuel();
        }

        AoCUtils.LogPart2(produced);
    }

    /// <summary>
    /// Produces one unit of fuel
    /// </summary>
    /// <returns>The amount of ore used</returns>
    private int ProduceOneFuel()
    {
        Chemical fuel = this.Data[FUEL];
        this.toProduce.Add(fuel, 1);
        int totalOre = 0;
        while (this.toProduce.Count > 0)
        {
            //Get the first reaction to take care of
            (Chemical product, int needed) = this.toProduce.First();

            //Check if there is any product surplus
            if (RemoveSurplus(product, ref needed))
            {
                //Get amount of times we must run the reaction
                int multiplier = (int)Math.Ceiling(needed / (double)product.Produced);
                int amountProduced = product.Produced * multiplier;
                //Set any extra produced
                if (amountProduced > needed)
                {
                    this.surplus.TryGetValue(product, out int productSurplus);
                    this.surplus[product] = productSurplus + amountProduced - needed;
                }

                foreach ((Chemical reactant, int required) in product.Reactants)
                {
                    //Get total amount of reactant needed
                    int amount = required * multiplier;
                    //Check if we have any leftover surplus
                    if (!RemoveSurplus(reactant, ref amount)) continue;

                    //If Ore is the reactant
                    if (reactant.Name is ORE)
                    {
                        //Add to the total amount of Ore needed
                        totalOre += amount;
                    }
                    else
                    {
                        //Adjust amount to produce of the reactant
                        if (this.toProduce.ContainsKey(reactant))
                        {
                            this.toProduce[reactant] += amount;
                        }
                        else
                        {
                            this.toProduce[reactant] = amount;
                        }
                    }
                }
            }

            //Remove product from to produce list
            this.toProduce.Remove(product);
        }

        return totalOre;
    }

    /// <summary>
    /// Removes any available stored surplus
    /// </summary>
    /// <param name="chemical">Chemical to get surplus for</param>
    /// <param name="needed">Amount of the chemical needed, the finalized amount will be stored there</param>
    /// <returns>True if there is still some chemical needed, false if the surplus covered it</returns>
    private bool RemoveSurplus(Chemical chemical, ref int needed)
    {
        if (!this.surplus.TryGetValue(chemical, out int extra)) return true;

        if (needed >= extra)
        {
            this.surplus.Remove(chemical);
            needed -= extra;
        }
        else
        {
            this.surplus[chemical] -= needed;
            needed = 0;
            return false;
        }

        return true;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Chemical> Convert(string[] rawInput)
    {
        //Prep reactions
        Dictionary<string, Chemical> chemicals = new(rawInput.Length) { ["ORE"] = new("ORE", 0) };
        List<(Chemical chemical, (int amount, string name)[] reactants)> reactions = new(rawInput.Length);
        //Parse reactions
        RegexFactory<(int, string)> reactantsFactory = new(REACTANTS_PATTERN, RegexOptions.Compiled);
        foreach ((string input, int count, string output) in RegexFactory<(string, int, string)>.ConstructObjects(REACTION_PATTERN, rawInput, RegexOptions.Compiled))
        {
            Chemical chemical = new(output, count);
            chemicals.Add(output, chemical);
            reactions.Add((chemical, reactantsFactory.ConstructObjects(input)));
        }

        //Link all reactions
        reactions.ForEach(r => r.chemical.Reactants = r.reactants.Select(c => new Reactant(chemicals[c.name], c.amount)).ToArray());
        return chemicals;
    }
    #endregion
}
