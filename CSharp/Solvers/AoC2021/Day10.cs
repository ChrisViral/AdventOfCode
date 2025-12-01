using System;
using System.Collections.Generic;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 10
/// </summary>
public class Day10 : Solver
{
    #region Constants
    /// <summary>Points for broken chunks</summary>
    private static readonly Dictionary<char, int> brokenPoints = new(4)
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137
    };
    /// <summary>Points for incomplete chunks</summary>
    private static readonly Dictionary<char, int> incompletePoints = new(4)
    {
        ['('] = 1,
        ['['] = 2,
        ['{'] = 3,
        ['<'] = 4,
    };
    /// <summary>Matching closing brackets</summary>
    private static readonly Dictionary<char, char> matching = new(4)
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
        ['>'] = '<',
    };
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver for 2021 - 10 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day10(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int brokenScore = 0;
        Stack<char> brackets = new();
        SortedList<long> incompleteScores = new();
        foreach (string line in this.Data)
        {
            foreach (char c in line)
            {
                if (c is '(' or '[' or '{' or '<')
                {
                    // Push opening brackets
                    brackets.Push(c);
                }
                else if (brackets.Pop() != matching[c])
                {
                    // If popped closing bracket mismatched, add score
                    brokenScore += brokenPoints[c];
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
                incompleteScore += incompletePoints[c];
            }
            incompleteScores.Add(incompleteScore);
        }

        AoCUtils.LogPart1(brokenScore);
        AoCUtils.LogPart2(incompleteScores[incompleteScores.Count / 2]);
    }
    #endregion
}
