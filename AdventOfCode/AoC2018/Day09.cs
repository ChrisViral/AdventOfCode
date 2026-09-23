using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Collections;
using Challenge.Utils.Extensions.Numbers;
using Challenge.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 9
/// </summary>
[Solver(2018, 9)]
public sealed partial class Day09 : Solver<(int players, int topMarble)>
{
    [GeneratedRegex(@"(\d+) players; last marble is worth (\d+) points")]
    private static partial Regex RulesMatcher { get; }

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
        LogAnswer(scores.Max());

        int end = this.Data.topMarble * 100;
        foreach (int marble in ^this.Data.topMarble..end)
        {
            int score = PlaceMarble(marble, circle, ref current);
            scores[player] += score;
            player = (player + 1) % scores.Length;
        }
        LogAnswer(scores.Max());
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
