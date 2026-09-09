using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 20
/// </summary>
public sealed class Day20 : ArraySolver<Day20.IPRange>
{
    public readonly record struct IPRange(uint Start, uint End) : IComparable<IPRange>
    {
        /// <inheritdoc />
        public int CompareTo(IPRange other) => this.Start.CompareTo(other.Start);
    }

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Sort ranges
        this.Data.Sort();
        List<IPRange> merged = new(200);

        // Merge them together
        IPRange current = this.Data[0];
        foreach (IPRange next in this.Data.AsSpan(1))
        {
            if (current.End != uint.MaxValue && next.Start > current.End + 1)
            {
                // No overlap, finish previous range and start new one
                merged.Add(current);
                current = next;
            }
            else if (next.End > current.End)
            {
                // Overlap, merge ranges
                current = current with { End = next.End };
            }
        }
        merged.Add(current);

        // Check earliest available spot
        IPRange firstBlocked = merged[0];
        uint lowest = firstBlocked.Start is not 0 ? 0 : firstBlocked.End + 1;
        AoCUtils.LogPart1(lowest);

        // Add allowed spots at start/end
        uint allowed = firstBlocked.Start + (uint.MaxValue - merged[^1].End);
        // Sum all gaps
        current = firstBlocked;
        foreach (int i in 1..merged.Count)
        {
            IPRange next = merged[i];
            allowed += next.Start - current.End - 1;
            current = next;
        }
        AoCUtils.LogPart2(allowed);
    }

    /// <inheritdoc />
    protected override IPRange ConvertLine(string line)
    {
        ReadOnlySpan<char> range = line;
        int dashIndex = range.IndexOf('-');
        uint start = uint.Parse(range[..dashIndex]);
        uint end = uint.Parse(range[(dashIndex + 1)..]);
        return new IPRange(start, end);
    }
}
