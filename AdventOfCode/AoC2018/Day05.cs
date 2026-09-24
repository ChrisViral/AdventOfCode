using Challenge.Solvers;
using Challenge.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 5
/// </summary>
[Solver(2018, 5)]
public sealed class Day05 : Solver<string>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        LinkedList<char> polymer = new(this.Data);
        SimplifyPolymer(polymer);
        LogAnswer(polymer.Count);

        int minSize = polymer.Count;
        foreach (char toRemove in StringUtils.ASCII_LOWER)
        {
            char toRemoveUpper = char.ToUpperInvariant(toRemove);
            polymer = new LinkedList<char>(this.Data.AsEnumerable().Where(c => c != toRemove && c != toRemoveUpper));
            SimplifyPolymer(polymer);
            minSize = Math.Min(minSize, polymer.Count);
        }
        LogAnswer(minSize);
    }

    private static void SimplifyPolymer(LinkedList<char> polymer)
    {
        LinkedListNode<char>? current = polymer.First;
        while(current?.Next is not null)
        {
            LinkedListNode<char> previous = current;
            current = current.Next;
            if (char.IsAsciiLetterLower(previous.Value) != char.IsAsciiLetterLower(current.Value)
             && char.ToLowerInvariant(previous.Value) == char.ToLowerInvariant(current.Value))
            {
                polymer.Remove(current);
                current = previous.Previous ?? previous.Next;
                polymer.Remove(previous);
            }
        }
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
