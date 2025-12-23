using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using CommunityToolkit.HighPerformance;
using SpanLinq;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 07
/// </summary>
public sealed partial class Day07 : RegexSolver<Day07.Program>
{
    [DebuggerDisplay("{Name} ({Weight}), TotalWeight: {TotalWeight}, IsBalanced: {IsBalanced}")]
    public sealed class Program(string name, int weight)
    {
        private readonly string[] childrenNames = [];

        public string Name { get; } = name;

        public int Weight { get; } = weight;

        public Program? Parent { get; private set; }

        public List<Program> Children { get; } = [];

        private int? totalWeight;
        public int TotalWeight => this.totalWeight ??= this.Weight + this.Children.Sum(c => c.TotalWeight);

        public bool IsBalanced => this.Children.Count <= 1 || this.Children.AsSpan()[1..].All(c => c.TotalWeight == this.Children[0].TotalWeight);

        public Program(string name, int weight, string children) : this(name, weight)
        {
            this.childrenNames = children.Split(", ", DEFAULT_OPTIONS);
            this.Children = new List<Program>(this.childrenNames.Length);
        }

        public void ResolveChildren(Dictionary<string, Program> programs)
        {
            foreach (string childName in this.childrenNames)
            {
                Program child = programs[childName];
                this.Children.Add(child);
                child.Parent = this;
            }
        }
    }

    [GeneratedRegex(@"(\w+) \((\d+)\)(?: -> ([\w, ]+))?")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day07"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day07(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Dictionary<string, Program> programs = this.Data.ToDictionary(p => p.Name, p => p);
        this.Data.ForEach(p => p.ResolveChildren(programs));
        Program root = this.Data.First(p => p.Parent is null);
        AoCUtils.LogPart1(root.Name);

        Program problem = root;
        while (!problem.IsBalanced)
        {
            problem = problem.Children.GroupBy(c => c.TotalWeight).First(g => g.Count() is 1).First();
        }

        int expectedWeight = problem.Parent!.Children.First(c => c != problem).TotalWeight;
        int diff = expectedWeight - problem.TotalWeight;
        AoCUtils.LogPart2(problem.Weight + diff);
    }
}
