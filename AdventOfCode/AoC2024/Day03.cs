using System.Text.RegularExpressions;
using Challenge.Solvers;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 3
/// </summary>
[Solver(2024, 3)]
public sealed partial class Day03 : Solver<string>
{
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do(?:n't)?\(\)")]
    private static partial Regex MulPattern { get; }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int result = 0;
        int conditionalResult = 0;
        bool flag = true;
        foreach (Match match in MulPattern.Matches(this.Data))
        {
            switch (match.ValueSpan)
            {
                case "do()":
                    flag = true;
                    break;

                case "don't()":
                    flag = false;
                    break;

                default:
                    int x = int.Parse(match.Groups[1].ValueSpan);
                    int y = int.Parse(match.Groups[2].ValueSpan);
                    int value = x * y;
                    result += value;
                    if (flag)
                    {
                        conditionalResult += value;
                    }
                    break;
            }
        }

        LogAnswer(result);
        LogAnswer(conditionalResult);
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => string.Join(string.Empty, rawInput);
}
