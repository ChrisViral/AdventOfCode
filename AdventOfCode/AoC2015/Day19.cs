using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 19
/// </summary>
[Solver(2015, 19)]
public sealed partial class Day19 : Solver<(ImmutableArray<Day19.Replacement> replacements, string molecule)>
{
    public readonly record struct Replacement(string From, string To)
    {
        public override string ToString() => $"{this.From} => {this.To}";
    }

    [GeneratedRegex(@"(\w+) => (\w+)")]
    private static partial Regex SubstitutionMatcher { get; }

    [GeneratedRegex(@"[A-Z](?:[a-z])?")]
    private static partial Regex ElementMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input, ILogger logger) : base(input, logger, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        ReadOnlySpan<char> molecule;
        StringBuilder builder = new(this.Data.molecule);
        HashSet<string> molecules = new(100);
        foreach (Replacement replacement in this.Data.replacements)
        {
            int offset = 0;
            molecule = this.Data.molecule;
            for (int i = molecule.IndexOf(replacement.From); i is not -1; i = molecule.IndexOf(replacement.From))
            {
                builder.Replace(replacement.From, replacement.To, i + offset, replacement.From.Length);
                molecules.Add(builder.ToString());
                builder.Replace(replacement.To, replacement.From, i + offset, replacement.To.Length);
                offset += i + 1;
                molecule = molecule[(i + 1)..];
            }
        }
        LogAnswer(molecules.Count);

        molecule = this.Data.molecule;
        int steps = ElementMatcher.Count(molecule) - 1;
        steps -= molecule.Count("Rn");
        steps -= molecule.Count("Ar");
        steps -= molecule.Count('Y') * 2;
        LogAnswer(steps);
    }

    /// <inheritdoc />
    protected override (ImmutableArray<Replacement>, string) Convert(string[] rawInput)
    {
        int emptyIndex = rawInput.IndexOf(string.Empty);
        RegexFactory<Replacement> replacementFactory = new(SubstitutionMatcher);
        Replacement[] replacements = replacementFactory.ConstructObjects(rawInput[..emptyIndex]);
        string molecule = rawInput[emptyIndex + 1];
        return ([..replacements], molecule);
    }
}
