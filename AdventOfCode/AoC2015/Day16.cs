using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 16
/// </summary>
public sealed partial class Day16 : RegexSolver<Day16.Sue>
{
    public readonly record struct Compound(string Name, int Amount);

    public readonly struct Sue
    {
        public int Number { get; }

        public ImmutableArray<Compound> Compounds { get; }

        public Sue(int number, string firstCompound, int firstAmount, string secondCompound, int secondAmount, string thirdCompound, int thirdAmount)
        {
            this.Number = number;
            Compound first  = new(firstCompound, firstAmount);
            Compound second = new(secondCompound, secondAmount);
            Compound third  = new(thirdCompound, thirdAmount);
            this.Compounds = [first, second, third];
        }
    }

    private static readonly FrozenDictionary<string, int> KnownData = FrozenDictionary.Create([
        new KeyValuePair<string, int>("children", 3),
        new KeyValuePair<string, int>("cats", 7),
        new KeyValuePair<string, int>("samoyeds", 2),
        new KeyValuePair<string, int>("pomeranians", 3),
        new KeyValuePair<string, int>("akitas", 0),
        new KeyValuePair<string, int>("vizslas", 0),
        new KeyValuePair<string, int>("goldfish", 5),
        new KeyValuePair<string, int>("trees", 3),
        new KeyValuePair<string, int>("cars", 2),
        new KeyValuePair<string, int>("perfumes", 1),
    ]);

    /// <inheritdoc />
    [GeneratedRegex(@"Sue (\d+): ([a-z]+): (\d+), ([a-z]+): (\d+), ([a-z]+): (\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Sue sue = this.Data.First(sue => sue.Compounds.All(c => KnownData[c.Name] == c.Amount));
        AoCUtils.LogPart1(sue.Number);

        sue = this.Data.First(IsValidSue);
        AoCUtils.LogPart2(sue.Number);
    }

    // ReSharper disable once CognitiveComplexity
    private static bool IsValidSue(Sue sue)
    {
        foreach (Compound compound in sue.Compounds)
        {
            int expectedAmount = KnownData[compound.Name];
            switch (compound.Name)
            {
                case "cats":
                case "trees":
                    if (expectedAmount >= compound.Amount) return false;
                    break;

                case "pomeranians":
                case "goldfish":
                    if (expectedAmount <= compound.Amount) return false;
                    break;

                default:
                    if (expectedAmount != compound.Amount) return false;
                    break;
            }
        }

        return true;
    }
}
