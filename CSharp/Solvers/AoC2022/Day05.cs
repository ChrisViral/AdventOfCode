using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 05
/// </summary>
public sealed class Day05 : Solver<(Stack<char>[] stacks, Day05.Move[] moves)>
{
    /// <summary>
    /// Crane move struct
    /// </summary>
    public readonly struct Move
    {
        /// <summary>Move parse pattern</summary>
        private const string PATTERN = @"move (\d+) from (\d+) to (\d+)";
        /// <summary>Regex matcher</summary>
        private static readonly Regex Matcher = new(PATTERN, RegexOptions.Compiled);

        /// <summary>
        /// Move amount
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// Move from stack index
        /// </summary>
        public int From { get; }

        /// <summary>
        /// Move to stack index
        /// </summary>
        public int To { get; }

        /// <summary>
        /// Creates a new move from a command line
        /// </summary>
        /// <param name="line">Line to parse the move from</param>
        public Move(string line)
        {
            // Extract values
            int[] values = Matcher.Match(line).CapturedGroups
                                  .Select(group => int.Parse(group.ValueSpan))
                                  .ToArray();
            Amount = values[0];
            From   = values[1] - 1;
            To     = values[2] - 1;
        }

        /// <summary>
        /// Displays the move under its command form
        /// </summary>
        /// <returns>A string representation of the move</returns>
        public override string ToString() => $"move {this.Amount} from {this.From + 1} to {this.To + 1}";
    }

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver for 2022 - 05 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day05(string input) : base(input, options: StringSplitOptions.None) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Create a copy of the stacks
        Stack<char>[] stacks = CopyStacks();
        foreach (Move move in this.Data.moves)
        {
            // Execute the moves
            Stack<char> from = stacks[move.From];
            Stack<char> to   = stacks[move.To];
            foreach (int _ in ..move.Amount)
            {
                to.Push(from.Pop());
            }
        }

        // Get message from top of stacks
        char[] message = new char[stacks.Length];
        foreach (int i in ..message.Length)
        {
            message[i] = stacks[i].Peek();
        }

        AoCUtils.LogPart1(new string(message));

        // Create another copy
        stacks = CopyStacks();
        Stack<char> crane = new();

        foreach (Move move in this.Data.moves)
        {
            Stack<char> from = stacks[move.From];
            Stack<char> to = stacks[move.To];
            foreach (int _ in ..move.Amount)
            {
                // Move from stack to crane
                crane.Push(from.Pop());
            }

            while (crane.TryPop(out char item))
            {
                // And back from crane to target
                to.Push(item);
            }
        }

        // Get message from top of stacks
        foreach (int i in ..message.Length)
        {
            message[i] = stacks[i].Peek();
        }
        AoCUtils.LogPart2(new string(message));
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Stack<char>[], Move[]) Convert(string[] lines)
    {
        // Create necessary stacks
        int stackCount = (lines[0].Length + 1) / 4;
        Stack<char>[] stacks = new Stack<char>[stackCount];
        stacks.Fill(() => new Stack<char>());

        // Find gap line
        int moveIndex = lines.FindIndex(string.IsNullOrWhiteSpace);

        // Work up from the bottom of stacks
        foreach (string line in lines[..(moveIndex - 1)].AsEnumerable().Reverse())
        {
            // Fill out stacks
            foreach (int stackIndex in ..stackCount)
            {
                int position = (stackIndex * 4) + 1;
                char value = line[position];
                if (value is not ' ')
                {
                    stacks[stackIndex].Push(value);
                }
            }
        }

        // Parse moves
        Move[] moves = new Move[lines.Length - ++moveIndex];
        moves.Fill(() => new Move(lines[moveIndex++]));
        return (stacks, moves);
    }

    /// <summary>
    /// Creates a shallow copy of the loaded stacks
    /// </summary>
    /// <returns>A shallow copy of the stacks</returns>
    private Stack<char>[] CopyStacks()
    {
        Stack<char>[] stacks = new Stack<char>[this.Data.stacks.Length];
        foreach (int i in ..stacks.Length)
        {
            stacks[i] = this.Data.stacks[i].CreateCopy();
        }
        return stacks;
    }
}
