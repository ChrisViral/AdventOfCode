using System.Collections.Frozen;
using System.Diagnostics;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 24
/// </summary>
public sealed class Day24 : Solver<FrozenDictionary<int, List<Day24.Pipe>>>
{
    [DebuggerDisplay("{Input}/{Output}")]
    public sealed record Pipe(int Input, int Output)
    {
        public int Strength { get; } = Input + Output;
    }

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int strongest = 0;
        HashSet<Pipe> used = new(this.Data.Count);
        foreach (Pipe start in this.Data[0])
        {
            used.Add(start);
            int strength = GetStrongestBridge(start, 0, used);
            strongest = Math.Max(strongest, strength);
            used.Remove(start);
        }
        AoCUtils.LogPart1(strongest);

        strongest = 0;
        int longest = 0;
        foreach (Pipe start in this.Data[0])
        {
            used.Add(start);
            (int length, int strength) = GetLongestBridge(start, 0, used);
            if (longest < length || longest == length && strongest < strength)
            {
                longest = length;
                strongest = strength;
            }
            used.Remove(start);
        }
        AoCUtils.LogPart2(strongest);
    }

    private int GetStrongestBridge(Pipe current, int arrivalPort, HashSet<Pipe> used)
    {
        int departurePort = current.Input == arrivalPort ? current.Output : current.Input;
        List<Pipe> targets = this.Data[departurePort];

        int maxStrength = 0;
        foreach (Pipe target in targets)
        {
            if (!used.Add(target)) continue;

            int strength = GetStrongestBridge(target, departurePort, used);
            maxStrength = Math.Max(maxStrength, strength);
            used.Remove(target);
        }
        return maxStrength + current.Strength;
    }

    private (int length, int strength) GetLongestBridge(Pipe current, int arrivalPort, HashSet<Pipe> used)
    {
        int departurePort = current.Input == arrivalPort ? current.Output : current.Input;
        List<Pipe> targets = this.Data[departurePort];

        int maxLength   = 0;
        int maxStrength = 0;
        foreach (Pipe target in targets)
        {
            if (!used.Add(target)) continue;

            (int length, int strength) = GetLongestBridge(target, departurePort, used);
            if (maxLength < length || maxLength == length && maxStrength < strength)
            {
                maxLength = length;
                maxStrength = strength;
            }
            used.Remove(target);
        }
        return (maxLength + 1, maxStrength + current.Strength);
    }

    /// <inheritdoc />
    protected override FrozenDictionary<int, List<Pipe>> Convert(string[] rawInput)
    {
        Dictionary<int, List<Pipe>> connections = new(rawInput.Length);
        foreach (ReadOnlySpan<char> line in rawInput)
        {
            // Parse input and output
            int separator = line.IndexOf('/');
            int input  = int.Parse(line[..separator]);
            int output = int.Parse(line[(separator + 1)..]);
            Pipe pipe = new(input, output);

            // Add input side to connections
            if (!connections.TryGetValue(input, out List<Pipe>? pipes))
            {
                pipes = [];
                connections[input] = pipes;
            }
            pipes.Add(pipe);

            // Add output side to connections
            if (!connections.TryGetValue(output, out  pipes))
            {
                pipes = [];
                connections[output] = pipes;
            }
            pipes.Add(pipe);
        }
        return connections.ToFrozenDictionary();
    }
}
