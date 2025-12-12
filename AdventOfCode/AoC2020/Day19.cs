using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 19
/// </summary>
public sealed partial class Day19 : Solver<(Day19.Rule[] rules, string[] messages)>
{
    /// <summary>
    /// Matching rule
    /// </summary>
    public sealed partial class Rule
    {
        /// <summary>
        /// Rule match pattern
        /// </summary>
        [GeneratedRegex("""^(\d+): (?:"(a|b)"|(\d+(?: \d+)?)|(\d+(?: \d+)?) \| (\d+(?: \d+)?))$""")]
        public static partial Regex Matcher { get; }

        /// <summary>
        /// Rule index
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Rule match value
        /// </summary>
        public string Pattern { get; set; } = string.Empty;

        /// <summary>
        /// Values of the first match
        /// </summary>
        public int[]? FirstMatch { get; }

        /// <summary>
        /// Values of the second match
        /// </summary>
        public int[]? SecondMatch { get; }

        /// <summary>
        /// Creates a new Rule and sets it's index
        /// </summary>
        /// <param name="index">Index of the rule</param>
        private Rule(int index) => this.Index = index;

        /// <summary>
        /// Creates a new rule from a single value
        /// </summary>
        /// <param name="index">Index of the rule</param>
        /// <param name="value">Value of the rule</param>
        public Rule(int index, string value) : this(index)
        {
            if (value is "a" or "b")
            {
                //Base rules
                this.Pattern = value;
            }
            else
            {
                //Pattern rules with a single match
                this.FirstMatch = value.Split(' ').ConvertAll(int.Parse);
            }
        }

        /// <summary>
        /// Creates a new rule from two patterns
        /// </summary>
        /// <param name="index">Index of the rule</param>
        /// <param name="first">First pattern</param>
        /// <param name="second">Second pattern</param>
        public Rule(int index, string first, string second) : this(index)
        {
            this.FirstMatch = first.Split(' ').ConvertAll(int.Parse);
            this.SecondMatch = second.Split(' ').ConvertAll(int.Parse);
        }

        /// <summary>
        /// Setups the pattern for this rule by looking at it's matches
        /// </summary>
        /// <param name="rules">Array of current rules</param>
        /// ReSharper disable once CognitiveComplexity
        public void SetupPattern(Rule[] rules)
        {
            //Make sure the first match is not null
            if (this.FirstMatch is not null)
            {
                StringBuilder builder = new(10);
                //Setup string builder
                foreach (Rule rule in this.FirstMatch.Select(i => rules[i]))
                {
                    if (string.IsNullOrEmpty(rule.Pattern))
                    {
                        rule.SetupPattern(rules);
                    }

                    builder.Append(rule.Pattern);
                }

                //Check for the second part of the match
                if (this.SecondMatch is not null)
                {
                    //Wrap in a non capturing group with an or
                    builder.Insert(0, "(?:").Append('|');
                    foreach (Rule rule in this.SecondMatch.Select(i => rules[i]))
                    {
                        if (string.IsNullOrEmpty(rule.Pattern))
                        {
                            rule.SetupPattern(rules);
                        }

                        builder.Append(rule.Pattern);
                    }

                    //Add final group parenthesis
                    builder.Append(')');
                }

                //Setup final pattern
                this.Pattern = builder.ToString();
            }
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Pattern;
    }

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input, options: StringSplitOptions.TrimEntries) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Add start/end matches to the pattern
        Rule[] rules = this.Data.rules;
        Rule origin = rules[0];
        origin.SetupPattern(rules);
        Regex match = new($"^{origin.Pattern}$", RegexOptions.Compiled);
        AoCUtils.LogPart1(this.Data.messages.Count(match.IsMatch));

        //Setup the new special patterns
        string first = rules[42].Pattern;
        string second = rules[31].Pattern;
        rules[8].Pattern = $"(?:{first})+";
        rules[11].Pattern = $"(?<first>{first})+(?<-first>{second})+(?(first)(?!))"; //Gotta love balanced constructs
        rules.Where(r => r.Index is not 8 and not 11).ForEach(r => r.Pattern = string.Empty);

        //Setup for the matches again
        origin.SetupPattern(rules);
        match = new Regex($"^{origin.Pattern}$", RegexOptions.Compiled);
        AoCUtils.LogPart2(this.Data.messages.Count(match.IsMatch));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Rule[], string[]) Convert(string[] rawInput)
    {
        //Get all rules
        int maxIndex = 0, i;
        List<Rule> ruleList = [];
        RegexFactory<Rule> ruleFactory = new(Rule.Matcher);
        for (i = 0; i < rawInput.Length; i++)
        {
            //Stop matching rules when we get to an empty line
            string line = rawInput[i];
            if (string.IsNullOrEmpty(line)) break;

            //Get and keep rule
            Rule rule = ruleFactory.ConstructObject(line);
            ruleList.Add(rule);
            maxIndex = Math.Max(maxIndex, rule.Index);
        }

        //Move the rules to an array
        Rule[] rules = ruleList.OrderBy(r => r.Index).ToArray();

        //Return the rules with the input to match
        return (rules, rawInput[(i + 1)..]);
    }
}
