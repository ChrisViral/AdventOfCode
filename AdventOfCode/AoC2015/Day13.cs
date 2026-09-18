using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Challenge.Utils;
using Challenge.Utils.Extensions.Ranges;
using Challenge.Maths.Vectors.BitVectors;
using Challenge.Solvers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 13
/// </summary>
public sealed partial class Day13 : Solver<(ImmutableArray<string> people, Dictionary<UnorderedPair<string>, int> happiness)>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private enum Change
    {
        LOSE,
        GAIN
    }

    private readonly record struct Relationship(string From, Change Change, int Amount, string To);

    [GeneratedRegex(@"(\w+) would (lose|gain) (\d+) happiness units by sitting next to (\w+)\.")]
    private static partial Regex RelationshipMatcher { get; }

    private BitVector16 doneState;

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day13(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int peopleCount = this.Data.people.Length;
        Span<bool> doneBitArray = stackalloc bool[peopleCount];
        doneBitArray.Fill(true);
        this.doneState = BitVector16.FromBitArray(doneBitArray);

        string startingPerson = this.Data.people[0];
        BitVector16 startingState = new()
        {
            [0] = true,
            [peopleCount- 1] = true
        };

        int max = GetMaxHappiness(startingPerson, startingState, 0, this.Data.people.AsSpan(0, peopleCount - 1));
        LogAnswer(max);

        startingState[peopleCount - 1] = false;
        max = GetMaxHappiness(startingPerson, startingState, 0, this.Data.people.AsSpan());
        LogAnswer(max);
    }

    private int GetMaxHappiness(string currentPerson, BitVector16 state, int happiness, ReadOnlySpan<string> people)
    {
        if (state == this.doneState)
        {
            // Add final happiness
            return happiness + this.Data.happiness[(currentPerson, people[0])];
        }

        int max = int.MinValue;
        foreach (int i in 1..people.Length)
        {
            if (state[i]) continue;

            string newPerson = people[i];
            int addedHappiness = this.Data.happiness[(currentPerson, newPerson)];
            BitVector16 newState = state;
            newState[i] = true;

            int subMax = GetMaxHappiness(newPerson, newState, happiness + addedHappiness, people);
            max = Math.Max(max, subMax);
        }

        return max;
    }

    /// <inheritdoc />
    protected override (ImmutableArray<string>, Dictionary<UnorderedPair<string>, int>) Convert(string[] rawInput)
    {
        HashSet<string> people = new(rawInput.Length);
        Dictionary<UnorderedPair<string>, int> happiness = new(rawInput.Length);

        RegexFactory<Relationship> factory = new(RelationshipMatcher);
        foreach (string line in rawInput)
        {
            Relationship relationship = factory.ConstructObject(line);
            people.Add(relationship.From);
            people.Add(relationship.To);

            int amount = relationship.Change is Change.GAIN ? relationship.Amount : -relationship.Amount;
            UnorderedPair<string> pair = (relationship.From, relationship.To);
            if (happiness.TryGetValue(pair, out int existingAmount))
            {
                amount += existingAmount;
            }
            happiness[pair] = amount;
        }

        foreach (string person in people)
        {
            happiness.Add((string.Empty, person), 0);
        }
        people.Add(string.Empty);
        return ([..people.OrderDescending()], happiness);
    }
}
