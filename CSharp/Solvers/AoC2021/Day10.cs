using System;
using System.Collections.Generic;
using AdventOfCode.Collections;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 10
/// </summary>
public class Day10 : Solver
{
    private static readonly Dictionary<char, int> brokenPoints = new(4)
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137
    };
    private static readonly Dictionary<char, int> incompletePoints = new(4)
    {
        ['('] = 1,
        ['['] = 2,
        ['{'] = 3,
        ['<'] = 4,
    };
    private static readonly Dictionary<char, char> matching = new(4)
    {
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
        ['>'] = '<',
    };

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver for 2021 - 10 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
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
                    brackets.Push(c);
                }
                else if (brackets.Pop() != matching[c])
                {
                    brokenScore += brokenPoints[c];
                    brackets.Clear();
                    break;
                }
            }

            if (brackets.IsEmpty()) continue;

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
