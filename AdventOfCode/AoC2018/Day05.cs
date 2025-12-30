using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 05
/// </summary>
public sealed class Day05 : Solver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        LinkedList<char> polymer = new(this.Data);
        SimplifyPolymer(polymer);
        AoCUtils.LogPart1(polymer.Count);

        int minSize = polymer.Count;
        foreach (char toRemove in StringUtils.ASCII_LOWER)
        {
            char toRemoveUpper = char.ToUpperInvariant(toRemove);
            polymer = new LinkedList<char>(this.Data.AsEnumerable().Where(c => c != toRemove && c != toRemoveUpper));
            SimplifyPolymer(polymer);
            minSize = Math.Min(minSize, polymer.Count);
        }
        AoCUtils.LogPart2(minSize);
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
