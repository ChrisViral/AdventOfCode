using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 07
/// </summary>
public class Day07 : Solver<Dictionary<string, Day07.Bag>>
{
    /// <summary>
    /// Bag object
    /// </summary>
    public class Bag : IEquatable<Bag>
    {
        #region Constants
        private const RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.Singleline;
        private static readonly Regex bagNameMatch     = new(@"^([a-z ]+) bags contain", OPTIONS);
        private static readonly Regex bagContentsMatch = new(@"(\d+) ([a-z ]+) bags?", OPTIONS);
        #endregion

        #region Fields
        private (string, int)[]? containedBagNames;
        #endregion
            
        #region Properties
        /// <summary>
        /// Bag Name
        /// </summary>
        public string Name { get; }
            
        /// <summary>
        /// Contents of the Bag, keyed by name and paired by amount
        /// </summary>
        public Dictionary<string, (Bag, int)> Contents { get; }

        /// <summary>
        /// Bags containing an amount of this Bag
        /// </summary>
        public HashSet<Bag> ContainedBy { get; } = [];
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new bag from a given definition
        /// </summary>
        /// <param name="definition">Bag definition</param>
        /// <exception cref="ArgumentException">Thrown if the definition is empty, or does not match for the bag name</exception>
        public Bag(string definition)
        {
            if (string.IsNullOrEmpty(definition)) throw new ArgumentException("Definition is an empty string.", nameof(definition));
                
            Match nameMatch = bagNameMatch.Match(definition);
            if (!nameMatch.Success) throw new ArgumentException($"Bag name could not be found in definition \"{definition}\".", nameof(definition));

            this.Name = nameMatch.Groups[1].Value;
            MatchCollection contentMatches = bagContentsMatch.Matches(definition, nameMatch.Length);
            this.containedBagNames = new (string, int)[contentMatches.Count];
            this.Contents = new Dictionary<string, (Bag, int)>(contentMatches.Count);
            for (int i = 0; i < contentMatches.Count; i++)
            {
                GroupCollection contents = contentMatches[i].Groups;
                this.containedBagNames[i] = (contents[2].Value, int.Parse(contents[1].Value));
            }
        }
        #endregion
            
        #region Methods
        /// <summary>
        /// Sets up the references of this Bag
        /// </summary>
        /// <param name="definitions">List of bag references available</param>
        public void SetupContents(IReadOnlyDictionary<string, Bag> definitions)
        {
            if (this.containedBagNames is null) return;
                
            foreach ((string bagName, int amount) in this.containedBagNames)
            {
                Bag contained = definitions[bagName];
                this.Contents.Add(bagName, (contained, amount));
                contained.ContainedBy.Add(this);
            }

            this.containedBagNames = null;
        }

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => this.Name.GetHashCode();
            
        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object? obj) => obj is Bag bag && Equals(bag);
            
        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(Bag? other) => other is not null && (ReferenceEquals(this, other) || this.Name == other.Name);

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Name;
        #endregion
    }
        
    #region Constants
    /// <summary>
    /// Bag owned in the problem
    /// </summary>
    private const string PERSONAL_BAG = "shiny gold";
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver from the specified file
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Dictionary{TKey,TValue}"/> fails</exception>
    public Day07(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Bag personalBag = this.Data[PERSONAL_BAG];
        HashSet<Bag> canContain = new(personalBag.ContainedBy);
        Queue<Bag> toCheck = new(canContain);
        while (toCheck.TryDequeue(out Bag? bag))
        {
            foreach (Bag b in bag.ContainedBy.Where(canContain.Add))
            {
                toCheck.Enqueue(b);
            }
        }
        AoCUtils.LogPart1(canContain.Count);

        int result = 0;
        Queue<(Bag, int)> contained = new();
        contained.Enqueue((personalBag, 1));
        while (contained.TryDequeue(out (Bag bag, int amount) bags))
        {
            foreach ((Bag bag, int amount) in bags.bag.Contents.Values)
            {
                int total = amount * bags.amount;
                result += total;
                contained.Enqueue((bag, total));
            }
        }
            
        AoCUtils.LogPart2(result);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Bag> Convert(string[] rawInput)
    {
        Dictionary<string, Bag> bags = new(rawInput.Length);
        foreach (string line in rawInput)
        {
            Bag bag = new(line);
            bags.Add(bag.Name, bag);
        }
            
        foreach (Bag bag in bags.Values)
        {
            bag.SetupContents(bags);
        }

        return bags;
    }
    #endregion
}
