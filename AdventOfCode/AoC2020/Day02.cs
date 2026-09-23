using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 2
/// </summary>
[Solver(2020, 2)]
public sealed partial class Day02 : Solver<Day02.PasswordData[]>
{
    /// <summary>
    /// Password info data
    /// </summary>
    public sealed record PasswordData(int Min, int Max, char Target, string Password);

    [GeneratedRegex(@"(\d+)-(\d+) ([a-z]): ([a-z]+)")]
    private static partial Regex PasswordDataMatcher { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int part1 = 0;
        int part2 = 0;
        foreach ((int min, int max, char target, string password) in this.Data)
        {
            //Part 1
            int occurrences = password.Count(c => c == target);
            if (occurrences >= min && occurrences <= max)
            {
                part1++;
            }

            //Part 2
            if (password[min - 1] == target)
            {
                if (password[max - 1] != target)
                {
                    part2++;
                }
            }
            else if (password[max - 1] == target)
            {
                part2++;
            }
        }

        LogAnswer(part1);
        LogAnswer(part2);
    }

    /// <inheritdoc />
    protected override PasswordData[] Convert(string[] rawInput) => RegexFactory<PasswordData>.ConstructObjects(PasswordDataMatcher, rawInput);
}
