using System.Collections.Immutable;
using System.Text.RegularExpressions;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 09
/// </summary>
public sealed partial class Day09 : Solver<(ImmutableArray<string> locations, Dictionary<UnorderedPair<string>, int> distances)>
{
    [GeneratedRegex(@"(\w+) to (\w+) = (\d+)")]
    private static partial Regex Matcher { get; }

    private BitVector8 doneState;

    /// <summary>
    /// Creates a new <see cref="Day09"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day09(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Span<bool> doneBitArray = stackalloc bool[this.Data.locations.Length];
        doneBitArray.Fill(true);
        this.doneState = BitVector8.FromBitArray(doneBitArray);

        int globalMin = int.MaxValue;
        int globalMax = int.MinValue;
        foreach (int i in ..this.Data.locations.Length)
        {
            string location = this.Data.locations[i];
            BitVector8 visited = new() { [i] = true };
            (int min, int max) = GetCriticalPaths(location, visited, 0);

            globalMin = Math.Min(globalMin, min);
            globalMax = Math.Max(globalMax, max);
        }
        AoCUtils.LogPart1(globalMin);
        AoCUtils.LogPart2(globalMax);
    }

    private (int min, int max) GetCriticalPaths(string currentLocation, BitVector8 visited, int travelled)
    {
        if (visited == this.doneState) return (travelled, travelled);

        int min = int.MaxValue;
        int max = int.MinValue;

        foreach (int i in ..this.Data.locations.Length)
        {
            if (visited[i]) continue;

            string newLocation = this.Data.locations[i];
            int distance = this.Data.distances[(currentLocation, newLocation)];
            BitVector8 newVisited = visited;
            newVisited[i] = true;

            (int subMin, int subMax) = GetCriticalPaths(newLocation, newVisited, travelled + distance);

            min = Math.Min(min, subMin);
            max = Math.Max(max, subMax);
        }

        return (min, max);
    }

    /// <inheritdoc />
    protected override (ImmutableArray<string>, Dictionary<UnorderedPair<string>, int>) Convert(string[] rawInput)
    {
        RegexFactory<(string, string, int)> factory = new(Matcher);
        HashSet<string> locations = new(rawInput.Length);
        Dictionary<UnorderedPair<string>, int> distances = new(rawInput.Length);
        foreach (string line in rawInput)
        {
            (string from, string to, int distance) = factory.ConstructObject(line);
            locations.Add(from);
            locations.Add(to);
            distances.Add((from, to), distance);
        }
        return ([..locations], distances);
    }
}
