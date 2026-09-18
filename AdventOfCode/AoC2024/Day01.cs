using System.Text.RegularExpressions;
using Challenge.Collections;
using Challenge.Utils;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Ranges;
using Challenge.Solvers;
using ZLinq;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 01
/// </summary>
public sealed partial class Day01 : Solver<(int[] leftList, int[] rightList)>
{
    [GeneratedRegex(@"(\d+)   (\d+)")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        this.Data.leftList.Sort();
        this.Data.rightList.Sort();
        int distance = this.Data.leftList.Zip(this.Data.rightList).Sum(d => Math.Abs(d.First - d.Second));
        ChallengeUtils.LogPart1(distance);

        Counter<int> left = new(this.Data.leftList);
        Counter<int> right = new(this.Data.rightList);
        int similarity = left.Sum<int>(v => v * left[v] * right.GetValueOrDefault(v));
        ChallengeUtils.LogPart2(similarity);
    }

    /// <inheritdoc />
    protected override (int[] leftList, int[] rightList) Convert(string[] rawInput)
    {
        (int left, int right)[] listData = RegexFactory<(int, int)>.ConstructObjects(Matcher, rawInput);
        (int[] leftList, int[] rightList) = (new int[listData.Length], new int[listData.Length]);
        foreach (int i in ..listData.Length)
        {
            leftList[i]  = listData[i].left;
            rightList[i] = listData[i].right;
        }
        return (leftList, rightList);
    }
}
