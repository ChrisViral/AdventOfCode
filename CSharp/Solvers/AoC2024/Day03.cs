using System;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 03
/// </summary>
public partial class Day03 : Solver<string>
{
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)", RegexOptions.Compiled)]
    private static partial Regex MulRegex { get; }

    [GeneratedRegex(@"do(?:n't)?\(\)", RegexOptions.Compiled)]
    private static partial Regex ConditionalRegex { get; }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day03(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int result = 0;
        MatchCollection mulMatches = MulRegex.Matches(this.Data);
        (int value, int index)[] operations = new (int, int)[mulMatches.Count];
        foreach (int i in ..mulMatches.Count)
        {
            Match mulMatch = mulMatches[i];
            int x = int.Parse(mulMatch.Groups[1].ValueSpan);
            int y = int.Parse(mulMatch.Groups[2].ValueSpan);
            int value = x * y;
            operations[i] = (value, mulMatch.Index);
            result += value;
        }
        AoCUtils.LogPart1(result);

        MatchCollection conditionalMatches = ConditionalRegex.Matches(this.Data);
        (int index, bool state)[] conditionals = new (int index, bool state)[conditionalMatches.Count + 1];
        conditionals[0] = (0, true);
        foreach (int i in ..conditionalMatches.Count)
        {
            Match conditionalMatch = conditionalMatches[i];
            conditionals[i + 1] = (conditionalMatch.Index, conditionalMatch.Value == "do()");
        }
        conditionals.Reversed();

        foreach ((int value, int index) in operations)
        {
            int i = conditionals.FindIndex(c => c.index < index);
            if (!conditionals[i].state)
            {
                result -= value;
            }
        }
        AoCUtils.LogPart2(result);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] rawInput) => string.Join(string.Empty, rawInput);
    #endregion
}