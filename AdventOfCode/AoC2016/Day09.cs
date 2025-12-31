using System.Text.RegularExpressions;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 09
/// </summary>
public sealed partial class Day09 : Solver<string>
{
    [GeneratedRegex(@"\((\d+)x(\d+)\)")]
    private static partial Regex MarkerMatcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int decompressedLength = GetDecompressedLength();
        AoCUtils.LogPart1(decompressedLength);

        long expandedLength = GetExpandedLength(0, this.Data.Length);
        AoCUtils.LogPart2(expandedLength);
    }

    private int GetDecompressedLength()
    {
        int length = 0;
        int currentIndex = 0;
        for (Match match = MarkerMatcher.Match(this.Data, currentIndex); match.Success; match = MarkerMatcher.Match(this.Data, currentIndex))
        {
            length += match.Index - currentIndex;

            int count   = int.Parse(match.Groups[1].Value);
            int repeats = int.Parse(match.Groups[2].Value);
            length += count * repeats;
            currentIndex = match.Index + match.Length + count;
        }
        return length + (this.Data.Length - currentIndex);
    }

    private long GetExpandedLength(int startIndex, int sectionLength)
    {
        long length = 0L;
        int currentIndex = startIndex;
        int remainingSectionLength = sectionLength;
        for (Match match = MarkerMatcher.Match(this.Data, currentIndex, remainingSectionLength); match.Success; match = MarkerMatcher.Match(this.Data, currentIndex, remainingSectionLength))
        {
            int preMatchLength = match.Index - currentIndex;
            length += preMatchLength;
            remainingSectionLength -= preMatchLength + match.Length;

            int count   = int.Parse(match.Groups[1].Value);
            int repeats = int.Parse(match.Groups[2].Value);
            int matchEnd = match.Index + match.Length;
            length += GetExpandedLength(matchEnd, count) * repeats;
            remainingSectionLength -= count;
            currentIndex = matchEnd + count;
        }
        return length + (startIndex + sectionLength) - currentIndex;
    }

    /// <inheritdoc />
    protected override string Convert(string[] rawInput) => rawInput[0];
}
