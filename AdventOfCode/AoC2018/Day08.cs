using System.Collections.Immutable;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 08
/// </summary>
public sealed class Day08 : Solver<Day08.Node>
{
    public record Node(ImmutableArray<Node> Children, ImmutableArray<int> Metadata)
    {
        private int? metadataSum;
        public int MetadataSum => this.metadataSum ??= this.Metadata.Sum() + this.Children.Sum(c => c.MetadataSum);

        private int? value;
        public int Value
        {
            get
            {
                if (this.value.HasValue) return this.value.Value;

                if (this.Children.IsEmpty)
                {
                    this.value = this.MetadataSum;
                }
                else
                {
                    this.value = this.Metadata
                                     .Where(i => i >= 1 && i <= this.Children.Length)
                                     .Sum(i => this.Children[i - 1].Value);
                }
                return this.value.Value;
            }
        }

        public static Node ParseNode(ref ReadOnlySpan<int> data)
        {
            int childCount = data[0];
            int metadataCount = data[1];
            data = data[2..];

            ImmutableArray<Node> children;
            if (childCount is 0)
            {
                children = ImmutableArray<Node>.Empty;
            }
            else
            {
                ImmutableArray<Node>.Builder childrenBuilder = ImmutableArray.CreateBuilder<Node>(childCount);
                foreach (int _ in ..childCount)
                {
                    childrenBuilder.Add(ParseNode(ref data));
                }
                children = childrenBuilder.ToImmutable();
            }

            ImmutableArray<int> metadata = metadataCount is not 0 ? [..data[..metadataCount]] : ImmutableArray<int>.Empty;
            data = data[metadataCount..];
            return new Node(children, metadata);
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        AoCUtils.LogPart1(this.Data.MetadataSum);
        AoCUtils.LogPart2(this.Data.Value);
    }

    /// <inheritdoc />
    protected override Node Convert(string[] rawInput)
    {
        ReadOnlySpan<char> line = rawInput[0];
        int count = line.Count(' ') + 1;
        Span<Range> splits = stackalloc Range[count];
        count = line.Split(splits, ' ');
        Span<int> data = stackalloc int[count];
        foreach (int i in ..count)
        {
            data[i] = int.Parse(line[splits[i]]);
        }

        ReadOnlySpan<int> parseData = data;
        return Node.ParseNode(ref parseData);
    }
}
