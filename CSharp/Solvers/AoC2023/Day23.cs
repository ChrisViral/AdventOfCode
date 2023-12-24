using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2023;

/// <summary>
/// Solver for 2023 Day 23
/// </summary>
public class Day23 : GridSolver<Day23.Element>
{
    public enum Element
    {
        PATH = '.',
        FOREST = '#',
        UP = '^',
        DOWN = 'v',
        LEFT = '<',
        RIGHT = '>'
    }

    public class Node(Vector2<int> value) : IEquatable<Node>
    {
        public Vector2<int> Value { get; } = value;

        public Dictionary<Node, int> Neighbours { get; } = [];

        /// <inheritdoc />
        public bool Equals(Node? other) => !ReferenceEquals(null, other)
                                        && (ReferenceEquals(this, other)
                                         || this.Value.Equals(other.Value));

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Node other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Value.GetHashCode();

        public override string ToString() => this.Value.ToString();

        public static IEnumerable<(Node, double)> GetNeighbours(Node node) => node.Neighbours.Select(p => (p.Key, (double)p.Value));
    }

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day23(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Element[] row = new Element[this.Data.Width];
        this.Data.GetRowNoAlloc(0, row);
        Vector2<int> startPosition = new(row.IndexOf(Element.PATH), 0);

        int last = this.Data.Height - 1;
        this.Data.GetRowNoAlloc(last, row);
        Vector2<int> endPosition = new(row.IndexOf(Element.PATH), last);

        int longestPath = (int)Math.Round(SearchUtils.GetMaxPathLengthDFS(startPosition, endPosition, GetNeighboursWithSlopes)!.Value);
        AoCUtils.LogPart1(longestPath);

        Dictionary<Vector2<int>, Node> nodes = this.Data.Dimensions
                                                   .EnumerateOver()
                                                   .Where(p => this.Data[p] is not Element.FOREST)
                                                   .ToDictionary(p => p, p => new Node(p));

        foreach ((Vector2<int> position, Node node) in nodes)
        {
            foreach (Vector2<int> neighbour in position.Adjacent()
                                                       .Where(p => this.Data.WithinGrid(p)
                                                                && this.Data[p] is not Element.FOREST))

            {
                node.Neighbours.Add(nodes[neighbour], 1);
            }
        }

        foreach (Node node in nodes.Values.Where(n => n.Neighbours.Count is 2))
        {
            KeyValuePair<Node, int>[] neighbours = node.Neighbours.ToArray();
            (Node first, int firstDistance) = neighbours[0];
            (Node second, int secondDistance) = neighbours[1];
            int distance = firstDistance + secondDistance;

            first.Neighbours.Remove(node);
            first.Neighbours.Add(second, distance);
            second.Neighbours.Remove(node);
            second.Neighbours.Add(first, distance);
        }

        Node start = nodes[startPosition];
        Node end   = nodes[endPosition];
        longestPath = (int)Math.Round(SearchUtils.GetMaxPathLengthDFS(start, end, Node.GetNeighbours)!.Value);
        AoCUtils.LogPart2(longestPath);
    }

    public IEnumerable<(Vector2<int>, double)> GetNeighboursWithSlopes(Vector2<int> position)
    {
        return (this.Data[position] switch
        {
            Element.PATH  => position.Adjacent().Where(p => this.Data.WithinGrid(p) && this.Data[p] is not Element.FOREST),
            Element.UP    => Enumerable.Repeat(position + Direction.UP, 1),
            Element.DOWN  => Enumerable.Repeat(position + Direction.DOWN, 1),
            Element.LEFT  => Enumerable.Repeat(position + Direction.LEFT, 1),
            Element.RIGHT => Enumerable.Repeat(position + Direction.RIGHT, 1),
            _             => throw new UnreachableException("Unknown current tile")
        }).Select(p => (p, 1d));
    }

    /// <inheritdoc />
    protected override Element[] LineConverter(string line) => line.Select(c => (Element)c).ToArray();
    #endregion
}