using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 23
/// </summary>
public class Day23 : Solver<int[]>
{
    #region Constants
    /// <summary>
    /// Number of moves done in part 1
    /// </summary>
    private const int PART1_MOVES = 100;
    /// <summary>
    /// Number of moves done in part 2
    /// </summary>
    private const int PART2_MOVES = 10_000_000;
    /// <summary>
    /// Cups amount for part 2
    /// </summary>
    private const int AMOUNT = 1000_000;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
    public Day23(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Start moving the cups
        LinkedListNode<int> current = MoveCups(this.Data, PART1_MOVES).NextCircular();
        //Get resulting string
        StringBuilder builder = new(this.Data.Length - 1);
        while (current.Value is not 1)
        {
            builder.Append(current.Value);
            current = current.NextCircular();
        }
        AoCUtils.LogPart1(builder);

        //Create large cups array
        int[] largeData = Enumerable.Range(1, AMOUNT).ToArray();
        this.Data.CopyTo(largeData, 0);
        //Move cups and get result
        current = MoveCups(largeData, PART2_MOVES).NextCircular();
        AoCUtils.LogPart2((long)current.Value * current.NextCircular().Value);
    }

    /// <summary>
    /// Moves the given cups around for a set number of moves
    /// </summary>
    /// <param name="labels">Cup labels</param>
    /// <param name="moves">Amount of moves to play</param>
    /// <returns>The final Node with label 1</returns>
    private static LinkedListNode<int> MoveCups(IEnumerable<int> labels, int moves)
    {
        //Store nodes in a quick access dictionary, and get min/max
        int min = int.MaxValue;
        int max = int.MinValue;
        LinkedList<int> cups = new(labels);
        Dictionary<int, LinkedListNode<int>> nodes = new(cups.Count);
        for (LinkedListNode<int>? node = cups.First; node is not null; node = node.Next)
        {
            nodes.Add(node.Value, node);
            min = Math.Min(min, node.Value);
            max = Math.Max(max, node.Value);
        }

        //Setup current cup as the first
        LinkedListNode<int> current = cups.First!;
        foreach (int _ in ..moves)
        {
            //Get three next nodes
            LinkedListNode<int> a = current.NextCircular();
            LinkedListNode<int> b = a.NextCircular();
            LinkedListNode<int> c = b.NextCircular();
            //Remove them
            cups.Remove(a);
            cups.Remove(b);
            cups.Remove(c);

            //Get next target value
            int target = current.Value;
            do
            {
                //Make sure we're in range and not the value of a removed node
                if (--target < min)
                {
                    target = max;
                }
            }
            while (target == a.Value || target == b.Value || target == c.Value);

            //Get target node and add after
            LinkedListNode<int> destination = nodes[target];
            cups.AddAfter(destination, a);
            cups.AddAfter(a, b);
            cups.AddAfter(b, c);
            //Next node is the one after the current
            current = current.NextCircular();
        }

        //Return the node with label 1
        return nodes[1];
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput[0].ToCharArray().ConvertAll(c => c - '0');
    #endregion
}
