using System.Diagnostics;
using System.Text;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Utils.Extensions.Strings;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 13
/// </summary>
public sealed class Day13 : Solver<(Day13.PacketList left, Day13.PacketList right)[]>
{
    /// <summary>
    /// Packet element marking interface
    /// </summary>
    public interface IPacketElement : IComparable<IPacketElement> { }

    /// <summary>
    /// Packet integer value
    /// </summary>
    /// <param name="Value">Value of this packet element</param>
    private sealed record PacketValue(int Value) : IPacketElement
    {
        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(IPacketElement? other) => other switch
        {
            PacketValue value => this.Value.CompareTo(value.Value),
            PacketList  list  => new PacketList(this).CompareTo(list),
            _                 => throw new UnreachableException("Unknown packet element type")
        };

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => this.Value.ToString();
    }

    /// <summary>
    /// Packet element list
    /// </summary>
    public sealed class PacketList : IPacketElement, IComparable<PacketList>
    {
        /// <summary>StringBuilder instance for ToString implementation</summary>
        private static readonly StringBuilder ToStringBuilder = new();

        /// <summary>
        /// Packet elements list
        /// </summary>
        public List<IPacketElement> Elements { get; } = [];

        /// <summary>
        /// Creates a new packet list from a string
        /// </summary>
        /// <param name="line">String list data</param>
        public PacketList(string line) : this(line.AsSpan()[1..^1]) { }

        /// <summary>
        /// Creates a new packet list from a char span
        /// </summary>
        /// <param name="data">List data</param>
        private PacketList(ReadOnlySpan<char> data)
        {
            int? sliceStart = 0;
            int depth       = 0;
            foreach (int i in ..data.Length)
            {
                switch (data[i])
                {
                    case '[':
                        if (depth is 0) sliceStart = i + 1;
                        depth++;
                        break;

                    case ']':
                        depth--;
                        if (depth is 0 && sliceStart is not null)
                        {
                            this.Elements.Add(new PacketList(data[sliceStart.Value..i]));
                            sliceStart = null;
                        }
                        break;

                    case ',' when depth is 0:
                        if (sliceStart is not null)
                        {
                            this.Elements.Add(new PacketValue(int.Parse(data[sliceStart.Value..i])));
                        }

                        sliceStart = i + 1;
                        break;
                }
            }

            if (sliceStart is not null && data.Length > 0)
            {
                this.Elements.Add(new PacketValue(int.Parse(data[sliceStart.Value..])));
            }
        }

        /// <summary>
        /// Creates a new packet list from a given single element
        /// </summary>
        /// <param name="element">Element to populate the list with</param>
        public PacketList(IPacketElement element) => this.Elements.Add(element);

        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(IPacketElement? other) => other switch
        {
            PacketValue value => CompareTo(new PacketList(value)),
            PacketList  list  => CompareTo(list),
            _                 => throw new UnreachableException("Unknown packet element type")
        };

        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(PacketList? other)
        {
            if (other is null) return 1;

            int length = Math.Min(this.Elements.Count, other.Elements.Count);
            foreach (int i in ..length)
            {
                IPacketElement left  = this.Elements[i];
                IPacketElement right = other.Elements[i];
                int comp = left.CompareTo(right);
                if (comp is not 0) return comp;
            }

            return this.Elements.Count.CompareTo(other.Elements.Count);
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => ToStringBuilder.Append('[')
                                                            .AppendJoin(',', this.Elements)
                                                            .Append(']')
                                                            .ToStringAndClear();
    }

    /// <summary>
    /// Creates a new <see cref="Day13"/> Solver for 2022 - 13 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day13(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int inOrder = 0;
        List<PacketList> packets = new((this.Data.Length * 2) + 2)
        {
            new PacketList(new PacketList(new PacketValue(2))),
            new PacketList(new PacketList(new PacketValue(6)))
        };
        foreach (int i in ..this.Data.Length)
        {
            (PacketList left, PacketList right) = this.Data[i];
            packets.Add(left);
            packets.Add(right);
            if (left.CompareTo(right) is -1)
            {
                inOrder += i + 1;
            }
        }

        AoCUtils.LogPart1(inOrder);

        packets.Sort();
        int firstDivider  = packets.FindIndex(p => p.Elements is [PacketList { Elements: [PacketValue { Value: 2 }] }]);
        int secondDivider = packets.FindIndex(p => p.Elements is [PacketList { Elements: [PacketValue { Value: 6 }] }]);
        AoCUtils.LogPart2((firstDivider + 1) * (secondDivider + 1));
    }

    /// <inheritdoc />
    protected override (PacketList, PacketList)[] Convert(string[] lines)
    {
        (PacketList, PacketList)[] pairs = new (PacketList, PacketList)[lines.Length / 2];
        foreach (int i in ..pairs.Length)
        {
            int j = i * 2;
            pairs[i] = (new PacketList(lines[j]), new PacketList(lines[j + 1]));
        }
        return pairs;
    }
}
