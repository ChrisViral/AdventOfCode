using System.Text.RegularExpressions;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Collections;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 22
/// </summary>
public sealed partial class Day22 : Solver<Day22.Node[]>
{
    public enum NodeData
    {
        EMPTY = '_',
        USED = '.',
        FULL = '#',
        GOAL = '*'
    }

    public readonly record struct Node(int X, int Y, int Size, int Used, int Available);

    [GeneratedRegex(@"/dev/grid/node-x(\d+)-y(\d+)\s+(\d+)T\s+(\d+)T\s+(\d+)T\s+\d+%")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Sort first by used total, descending
        this.Data.Sort((a, b) => b.Used.CompareTo(a.Used));
        // Count pairs that have used, and that can be moved
        int valid = this.Data.EnumeratePairs()
                             .Where(p => p.first.Used is not 0)
                             .Count(p => p.first.Used <= p.second.Available);
        AoCUtils.LogPart1(valid);

        // Get grid size and smallest available space size
        int minSize = int.MaxValue;
        Vector2<int> maxPosition = Vector2<int>.Zero;
        foreach (Node node in this.Data)
        {
            minSize = Math.Min(minSize, node.Size);
            maxPosition = Vector2<int>.Max(maxPosition, (node.X, node.Y));
        }

        Grid<NodeData> system = new(maxPosition.X + 1, maxPosition.Y + 1, n => ((char)n).ToString());
        foreach (Node node in this.Data)
        {
            system[node.X, node.Y] = node.Used switch
            {
                0                      => NodeData.EMPTY, // Node has no data
                int u when u < minSize => NodeData.USED,  // Node can fit in smallest available node
                _                      => NodeData.FULL   // Not cannot be moved
            };
        }

        // Get current empty node position, and current goal node position
        Vector2<int> emptyPosition = system.PositionOf(NodeData.EMPTY);
        Vector2<int> goalPosition = new(maxPosition.X, 0);
        system[goalPosition] = NodeData.GOAL;

        // Calculate path from empty to node
        int toGoal = SearchUtils.GetPathLengthBFS(emptyPosition, goalPosition, p => p.AsAdjacentEnumerable()
                                                                                     .Where(a => system.WithinGrid(a)
                                                                                              && system[a] is not NodeData.FULL))!.Value;
        // Shifting the node towards the start requires five moves per single nudge
        int toStart = (goalPosition.X - 1) * 5;

        // Result is moving the empty to goal, and then goal to start
        AoCUtils.LogPart2(toGoal + toStart);
    }

    /// <inheritdoc />
    protected override Node[] Convert(string[] rawInput)
    {
        RegexFactory<Node> factory = new(Matcher);
        return factory.ConstructObjects(rawInput[2..]);
    }
}
