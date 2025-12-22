using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 25
/// </summary>
public sealed class Day25 : Solver<Dictionary<string, Day25.Component>>
{
    public sealed class Component(string name) : IEquatable<Component>
    {
        public string Name { get; } = name;

        public HashSet<Component> Connections { get; } = [];

        public static void AddConnection(Component a, Component b)
        {
            a.Connections.Add(b);
            b.Connections.Add(a);
        }

        public static void RemoveConnection(Component a, Component b)
        {
            a.Connections.Remove(b);
            b.Connections.Remove(a);
        }

        public static IEnumerable<MoveData<Component, double>> GetNeighbours(Component component) => component.Connections.Select(c => new MoveData<Component, double>(c, 1d));

        /// <inheritdoc />
        public bool Equals(Component? other) => !ReferenceEquals(null, other)
                                             && (ReferenceEquals(this, other)
                                              || this.Name == other.Name);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Component other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => this.Name.GetHashCode();
    }

    private readonly struct Edge(string a, string b) : IEquatable<Edge>
    {
        public readonly string a = a;
        public readonly string b = b;

        /// <inheritdoc />
        public bool Equals(Edge other) => (this.a == other.a && this.b == other.b)
                                       || (this.a == other.b && this.b == other.a);

        public override int GetHashCode() => unchecked(this.a.GetHashCode() + this.b.GetHashCode());

        public override string ToString() => $"({this.a}, {this.b})";
    }

    private const int SAMPLES = 10;

    /// <summary>
    /// Creates a new <see cref="Day25"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day25(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int result;
        do
        {
            Counter<Edge> edges = [];
            Component[] components = this.Data.Values.ToArray();
            foreach (int _ in ..SAMPLES)
            {
                int i = Random.Shared.Next(components.Length);
                int j = i;
                while (i == j)
                {
                    j = Random.Shared.Next(components.Length);
                }

                Component a = components[i];
                Component b = components[j];

                Component[] path = SearchUtils.Search(a, b, null, Component.GetNeighbours, MinSearchComparer<double>.Comparer, out double _)!;
                Component previous = path[0];
                foreach (int k in ..(path.Length - 1))
                {
                    Component current = path[k];
                    edges.Add(new Edge(previous.Name, current.Name));
                    previous = current;
                }
            }

            foreach (Edge edge in edges.AsDictionary()
                                       .OrderByDescending(p => p.Value)
                                       .Take(3)
                                       .Select(p => p.Key))
            {
                Component a = this.Data[edge.a];
                Component b = this.Data[edge.b];
                Component.RemoveConnection(a, b);
            }

            Queue<Component> open = [];
            HashSet<Component> visited = [];
            open.Enqueue(components[0]);
            while (open.TryDequeue(out Component? current))
            {
                current.Connections
                       .Where(n => !visited.Contains(n))
                       .ForEach(open.Enqueue);
                visited.Add(current);
            }

            int m = visited.Count;
            int n = this.Data.Count - m;
            result = m * n;
        }
        while (result is 0);

        AoCUtils.LogPart1(result);

        AoCUtils.LogPart2("Merry Christmas!");
    }

    /// <inheritdoc />
    protected override Dictionary<string, Component> Convert(string[] rawInput)
    {
        Dictionary<string, Component> components = new(rawInput.Length);

        foreach (string line in rawInput)
        {
            string[] splits  = line.Split(':', DEFAULT_OPTIONS);
            string fromName  = splits[0];
            string[] toNames = splits[1].Split(' ');
            if (!components.TryGetValue(fromName, out Component? from))
            {
                from = new Component(fromName);
                components.Add(fromName, from);
            }

            foreach (string toName in toNames)
            {
                if (!components.TryGetValue(toName, out Component? to))
                {
                    to = new Component(toName);
                    components.Add(toName, to);
                }

                Component.AddConnection(from, to);
            }
        }

        return components;
    }
}
