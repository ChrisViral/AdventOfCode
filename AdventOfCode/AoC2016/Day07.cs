using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 7
/// </summary>
[Solver(2016, 7)]
public sealed partial class Day07 : ArraySolver<string>
{
    [GeneratedRegex(@"^(?=(?:[a-z\[\]]+\])?[a-z]*([a-z])(?!\1)([a-z])\2\1)(?![a-z\[\]]*\[[a-z]*([a-z])(?!\3)([a-z])\4\3)[a-z\[\]]+$")]
    private static partial Regex TLSMatcher { get; }

    [GeneratedRegex(@"(?:^|(?<=\]))[a-z]+(?=$|\[)")]
    private static partial Regex OutsideBracketsMatcher { get; }

    [GeneratedRegex(@"([a-z])(?=(?!\1)([a-z])\1)")]
    private static partial Regex ThreeCharPolindromeMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int supportsTLS = this.Data.Count(TLSMatcher.IsMatch);
        LogAnswer(supportsTLS);

        int suportsSSL = this.Data.Count(SupportsSSL);
        LogAnswer(suportsSSL);
    }

    private static bool SupportsSSL(string ip)
    {
        foreach (ValueMatch outsideBrackets in OutsideBracketsMatcher.EnumerateMatches(ip))
        {
            string current = ip.Substring(outsideBrackets.Index, outsideBrackets.Length);
            foreach (Match match in ThreeCharPolindromeMatcher.Matches(current))
            {
                if (Regex.IsMatch(ip, $@"(?<=\[)[a-z]*{match.Groups[2]}{match.Groups[1]}{match.Groups[2]}"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
