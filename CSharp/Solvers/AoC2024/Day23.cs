using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using SpanLinq;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 23
/// </summary>
public sealed class Day23 : Solver<Day23.NetworkNode[]>
{
    /// <summary>
    /// Network node object
    /// </summary>
    /// <param name="id">Node ID</param>
    public sealed class NetworkNode(string id) : IEquatable<NetworkNode>
    {
        /// <summary>
        /// Network node ID
        /// </summary>
        public string ID { get; } = id;

        /// <summary>
        /// Network node connections
        /// </summary>
        public HashSet<NetworkNode> Connections { get; } = [];

        /// <inheritdoc />
        public bool Equals(NetworkNode? other) => this.ID == other?.ID;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is NetworkNode node && Equals(node);

        /// <inheritdoc />
        public override int GetHashCode() => this.ID.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => this.ID;

        /// <summary>
        /// Connects two nodes
        /// </summary>
        /// <param name="a">First node</param>
        /// <param name="b">Second node</param>
        public static void Connect(NetworkNode a, NetworkNode b)
        {
            a.Connections.Add(b);
            b.Connections.Add(a);
        }
    }

    /// <summary>
    /// Unordered network group
    /// </summary>
    /// <param name="a">First node</param>
    /// <param name="b">Second node</param>
    /// <param name="c">Third node</param>
    public readonly struct NetworkGroup(NetworkNode a, NetworkNode b, NetworkNode c) : IEquatable<NetworkGroup>
    {
        /// <summary>
        /// First ID
        /// </summary>
        public string A { get; } = a.ID;

        /// <summary>
        /// Second ID
        /// </summary>
        public string B { get; } = b.ID;

        /// <summary>
        /// Third ID
        /// </summary>
        public string C { get; } = c.ID;

        /// <inheritdoc />
        // ReSharper disable once CognitiveComplexity
        public bool Equals(NetworkGroup other)
        {
            if (this.A == other.A)
            {
                return (this.B == other.B && this.C == other.C) || (this.B == other.C && this.C == other.B);
            }
            if (this.A == other.B)
            {
                return (this.B == other.A && this.C == other.C) || (this.B == other.C && this.C == other.A);
            }
            if (this.A == other.C)
            {
                return (this.B == other.A && this.C == other.B) || (this.B == other.B && this.C == other.A);
            }
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is NetworkGroup other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => unchecked(this.A.GetHashCode() + this.B.GetHashCode() + this.C.GetHashCode());

        /// <inheritdoc />
        public override string ToString() => $"({this.A}, {this.B}, {this.C})";

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="a">First group</param>
        /// <param name="b">Second group</param>
        /// <returns><see langword="true"/> if both groups are equal, otherwise <see langword="false"/></returns>
        public static bool operator ==(in NetworkGroup a, in NetworkGroup b) => a.Equals(b);

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="a">First group</param>
        /// <param name="b">Second group</param>
        /// <returns><see langword="true"/> if both groups are unequal, otherwise <see langword="false"/></returns>
        public static bool operator !=(in NetworkGroup a, in NetworkGroup b) => !a.Equals(b);
    }

    /// <summary>
    /// Creates a new <see cref="Day23"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day23(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<NetworkNode> current = new(32);
        HashSet<NetworkNode> used = new(this.Data.Length);
        HashSet<NetworkGroup> groups = new(this.Data.Length);
        foreach (NetworkNode node in this.Data)
        {
            // Add to used nodes set
            used.Add(node);
            foreach (NetworkNode connection in node.Connections)
            {
                // Add all connections
                current.AddRange(connection.Connections);
                // Only keep those also in original node
                current.IntersectWith(node.Connections);
                // Remove all already used nodes
                current.ExceptWith(used);

                // Nodes remaining form a trio, create groups for them
                foreach (NetworkNode final in current)
                {
                    groups.Add(new NetworkGroup(node, connection, final));
                }
                current.Clear();
            }
        }

        // Count groups with a node starting with t
        int validGroups = groups.Count(g => g.A[0] is 't' || g.B[0] is 't' || g.C[0] is 't');
        AoCUtils.LogPart1(validGroups);

        // Run algorithm and find largest group
        HashSet<NetworkNode> nodes = [..this.Data];
        List<NetworkNode[]> cliques = FindAllCliques(nodes);
        NetworkNode[] largestGroup = cliques.MaxBy(c => c.Length)!;
        AoCUtils.LogPart2(string.Join(',', largestGroup.AsEnumerable().OrderBy(n => n.ID)));
    }

    /// <summary>
    /// Finds all cliques using the Bron-Kerbosch algorithm
    /// </summary>
    /// <param name="allNodes">Original set of all valid nodes</param>
    /// <returns></returns>
    private static List<NetworkNode[]> FindAllCliques(HashSet<NetworkNode> allNodes)
    {
        // Inner recursive function
        static void FindCliques(HashSet<NetworkNode> nodes, HashSet<NetworkNode> included, HashSet<NetworkNode> excluded, List<NetworkNode[]> cliques)
        {
            // If included and excluded sets are empty, we found a clique
            if (included.Count is 0 && excluded.Count is 0)
            {
                cliques.Add(nodes.ToArray());
                return;
            }

            // Loop through all possibly included nodes
            foreach (NetworkNode node in included)
            {
                // Add current node to clique
                nodes.Add(node);

                // Make new set of included nodes
                HashSet<NetworkNode> newIncluded = ObjectPool<HashSet<NetworkNode>>.Shared.Rent();
                newIncluded.EnsureCapacity(16);
                newIncluded.AddRange(included);
                newIncluded.IntersectWith(node.Connections);

                // Make new set of excluded nodes
                HashSet<NetworkNode> newExcluded = ObjectPool<HashSet<NetworkNode>>.Shared.Rent();
                newIncluded.EnsureCapacity(16);
                newExcluded.AddRange(excluded);
                newExcluded.IntersectWith(node.Connections);

                // Recurse to find further cliques
                FindCliques(nodes, newIncluded, newExcluded, cliques);

                // Return sets to pool
                newIncluded.Clear();
                newExcluded.Clear();
                ObjectPool<HashSet<NetworkNode>>.Shared.Return(newIncluded);
                ObjectPool<HashSet<NetworkNode>>.Shared.Return(newExcluded);

                // Remove back added node
                nodes.Remove(node);

                // Update included and excluded sets
                included.Remove(node);
                excluded.Add(node);
            }
        }

        // Recursion initialization
        List<NetworkNode[]> result = new(16);
        FindCliques(new HashSet<NetworkNode>(16), allNodes, new HashSet<NetworkNode>(16), result);
        return result;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override NetworkNode[] Convert(string[] rawInput)
    {
        Dictionary<string, NetworkNode> nodes = new(rawInput.Length);
        Dictionary<string, NetworkNode>.AlternateLookup<ReadOnlySpan<char>> nodesLookup = nodes.GetAlternateLookup<ReadOnlySpan<char>>();
        foreach (ReadOnlySpan<char> line in rawInput)
        {
            ReadOnlySpan<char> firstID  = line[..2];
            ReadOnlySpan<char> secondID = line[3..];

            if (!nodesLookup.TryGetValue(firstID, out NetworkNode? first))
            {
                first = new NetworkNode(firstID.ToString());
                nodesLookup[firstID] = first;
            }
            if (!nodesLookup.TryGetValue(secondID, out NetworkNode? second))
            {
                second = new NetworkNode(secondID.ToString());
                nodesLookup[secondID] = second;
            }

            NetworkNode.Connect(first, second);
        }

        return nodes.Values.ToArray();
    }
}
