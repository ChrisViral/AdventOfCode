using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 10
/// </summary>
[Solver(2021, 10)]
public sealed partial class Day10 : Solver
{
    /// <summary>Points for broken chunks</summary>
    private static readonly Dictionary<char, int> BrokenPoints = new(4)
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137
    };
    /// <summary>Points for incomplete chunks</summary>
    private static readonly Dictionary<char, int> IncompletePoints = new(4)
    {
        ['('] = 1,
        ['['] = 2,
        ['{'] = 3,
        ['<'] = 4,
    };
    /// <summary>Matching closing brackets</summary>
    private static readonly Dictionary<char, char> Matching = new(4)
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
        ['>'] = '<',
    };

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int brokenScore = 0;
        Stack<char> brackets = new();
        SortedList<long> incompleteScores = [];
        foreach (string line in this.Data)
        {
            foreach (char c in line)
            {
                if (c is '(' or '[' or '{' or '<')
                {
                    // Push opening brackets
                    brackets.Push(c);
                }
                else if (brackets.Pop() != Matching[c])
                {
                    // If popped closing bracket mismatched, add score
                    brokenScore += BrokenPoints[c];
                    brackets.Clear();
                    break;
                }
            }

            // If some brackets were not closed
            if (brackets.IsEmpty) continue;

            // Calculate score for incomplete brackets
            long incompleteScore = 0L;
            while (brackets.TryPop(out char c))
            {
                incompleteScore *= 5L;
                incompleteScore += IncompletePoints[c];
            }
            incompleteScores.Add(incompleteScore);
        }

        LogAnswer(brokenScore);
        LogAnswer(incompleteScores[incompleteScores.Count / 2]);
    }
}
