using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 4
/// </summary>
[Solver(2020, 4)]
public sealed partial class Day04 : Solver<Day04.Passport[]>
{
    /// <summary>
    /// Passport info
    /// </summary>
    public sealed partial class Passport
    {
        [GeneratedRegex(@"^(\d{2,3})(cm|in)$", RegexOptions.Singleline)]
        private static partial Regex HeightMatcher { get; }
        [GeneratedRegex(@"^#[\da-f]{6}$", RegexOptions.Singleline)]
        private static partial Regex HairMatcher   { get; }
        [GeneratedRegex(@"^\d{9}$", RegexOptions.Singleline)]
        private static partial Regex IDMatcher     { get; }

        private static readonly HashSet<string> ValidEyeColours = ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"];

        public string? byr;
        public string? iyr;
        public string? eyr;
        public string? hgt;
        public string? hcl;
        public string? ecl;
        public string? pid;

        public bool IsValid => this.byr is not null
                            && this.iyr is not null
                            && this.eyr is not null
                            && this.hgt is not null
                            && this.hcl is not null
                            && this.ecl is not null
                            && this.pid is not null;

        // ReSharper disable once CognitiveComplexity
        public bool Validate()
        {
            //Check the years
            if (!int.TryParse(this.byr, out int birthYear) || birthYear is < 1920 or > 2002) return false;
            if (!int.TryParse(this.iyr, out int issueYear) || issueYear is < 2010 or > 2020) return false;
            if (!int.TryParse(this.eyr, out int expYear)   || expYear   is < 2020 or > 2030) return false;

            //Check height
            Match match = HeightMatcher.Match(this.hgt!);
            if (!match.Success || match.Groups.Count is not 3 || !int.TryParse(match.Groups[1].Value, out int height)) return false;
            switch (match.Groups[2].Value)
            {
                case "cm":
                    if (height is < 150 or > 193) return false;
                    break;
                case "in":
                    if (height is < 59 or > 76) return false;
                    break;

                default:
                    return false;
            }

            //Check colours and Passport ID
            return HairMatcher.IsMatch(this.hcl!) && IDMatcher.IsMatch(this.pid!) && ValidEyeColours.Contains(this.ecl!);
        }
    }

    [GeneratedRegex("([a-z]{3}):([#a-z0-9]+)")]
    private static partial Regex PassportMatcher { get; }

    /// <inheritdoc />
    public Day04(ILogger logger) : base(logger, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Calculate valid for Part 1
        List<Passport> valid = this.Data.Where(p => p.IsValid).ToList();
        LogAnswer(valid.Count);

        //Validate for Part 2
        LogAnswer(valid.Count(p => p.Validate()));
    }

    /// <inheritdoc />
    protected override Passport[] Convert(string[] rawInput) => RegexFactory<Passport>.PopulateObjects(PassportMatcher, CombineLines(rawInput).Select(l => string.Join(' ', l))
                                                                                                                                              .ToList());
}
