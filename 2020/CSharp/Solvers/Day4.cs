using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers
{
    public class Day4 : Solver<Day4.Passport[]>
    {
        public class Passport
        {
            private const RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.Singleline;
            private static readonly Regex heightMatch = new(@"^(\d{2,3})(cm|in)$", OPTIONS);
            private static readonly Regex hairMatch   = new(@"^#[\da-f]{6}$", OPTIONS);
            private static readonly Regex idMatch     = new(@"^\d{9}$", OPTIONS);
            private static readonly HashSet<string> validEyeColours = new() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
            
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

            public bool Validate()
            {
                if (!int.TryParse(this.byr, out int birthYear) || birthYear is < 1920 or > 2002) return false;
                if (!int.TryParse(this.iyr, out int issueYear) || issueYear is < 2010 or > 2020) return false;
                if (!int.TryParse(this.eyr, out int expYear)   || expYear   is < 2020 or > 2030) return false;

                Match match = heightMatch.Match(this.hgt!);
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

                return hairMatch.IsMatch(this.hcl!) && idMatch.IsMatch(this.pid!) && validEyeColours.Contains(this.ecl!);
            }
        }

        private const string PATTERN = "([a-z]{3}):([a-z0-9#]+)";

        /// <summary>
        /// Creates a new <see cref="Day4"/> with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <typeparamref name="T"/> fails</exception>
        public Day4(FileInfo file) : base(file, options: StringSplitOptions.TrimEntries) { }

        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            //Calculate valid for Part 1
            Passport[] valid = this.Input.Where(p => p.IsValid).ToArray();
            Trace.WriteLine(valid.Length);
            
            //Validate for Part 2
            Trace.WriteLine(valid.Count(p => p.Validate()));
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override Passport[] Convert(string[] rawInput)
        {
            List<string> input = new();
            string line = string.Empty;
            foreach (string inputLine in rawInput)
            {
                if (string.IsNullOrWhiteSpace(inputLine))
                {
                    input.Add(line);
                    line = string.Empty;
                }
                else
                {
                    line += " " + inputLine;
                }
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                input.Add(line);
            }
            
            return RegexUtils.PopulateObjects<Passport>(PATTERN, input, RegexOptions.Compiled);
        }
    }
}
