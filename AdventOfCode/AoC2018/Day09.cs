using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 09
/// </summary>
public sealed partial class Day09 : Solver<(int players, int topMarble)>
{
    [GeneratedRegex(@"(\d+) players; last marble is worth (\d+) points")]
    private static partial Regex RulesMatcher { get; }

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
        int player = 0;
        long[] scores = new long[this.Data.players];
        LinkedList<int> circle = new();
        LinkedListNode<int> current = circle.AddFirst(0);
        foreach (int marble in 1..^this.Data.topMarble)
        {
            int score = PlaceMarble(marble, circle, ref current);
            scores[player] += score;
            player = (player + 1) % scores.Length;
        }
        AoCUtils.LogPart1(scores.Max());

        int end = this.Data.topMarble * 100;
        foreach (int marble in ^this.Data.topMarble..end)
        {
            int score = PlaceMarble(marble, circle, ref current);
            scores[player] += score;
            player = (player + 1) % scores.Length;
        }
        AoCUtils.LogPart2(scores.Max());
    }

    private static int PlaceMarble(int marble, LinkedList<int> circle, ref LinkedListNode<int> current)
    {
        if (!marble.IsMultiple(23))
        {
            current = circle.AddAfter(current.NextCircular(), marble);
            return 0;
        }

        current = current.PreviousCircular()
                         .PreviousCircular()
                         .PreviousCircular()
                         .PreviousCircular()
                         .PreviousCircular()
                         .PreviousCircular();
        LinkedListNode<int> toRemove = current.PreviousCircular();
        circle.Remove(toRemove);
        return toRemove.Value + marble;
    }

    /// <inheritdoc />
    protected override (int, int) Convert(string[] rawInput)
    {
        Match match = RulesMatcher.Match(rawInput[0]);
        return (int.Parse(match.Groups[1].ValueSpan), int.Parse(match.Groups[2].ValueSpan));
    }
}
