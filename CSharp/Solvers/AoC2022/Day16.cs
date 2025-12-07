using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Extensions.Regexes;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 16
/// </summary>
public partial class Day16 : Solver<(Day16.Valve start, Day16.Valve[] valves)>
{
    /// <summary>
    /// Valve object
    /// </summary>
    /// <param name="id">Valve ID</param>
    /// <param name="flowRate">Valve flow rate</param>
    /// <param name="connectionsCount">Valve connections count</param>
    public sealed class Valve(string id, int flowRate, int connectionsCount) : IEquatable<Valve>
    {
        /// <summary>
        /// Valve ID
        /// </summary>
        private string ID { get; } = id;

        /// <summary>
        /// Valve flow rate
        /// </summary>
        public int FlowRate { get; } = flowRate;

        /// <summary>
        /// Valve connections
        /// </summary>
        public Valve[] Connections { get; } = new Valve[connectionsCount];

        /// <summary>
        /// If this valve is currently open
        /// </summary>
        public bool IsOpen { get; set; }

        /// <inheritdoc/>
        public bool Equals(Valve? other) => this.ID == other?.ID;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Valve other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => this.ID.GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => $"{this.ID} ({this.FlowRate})";
    }

    /// <summary>
    /// Time for part 1
    /// </summary>
    private const int PART1_TIME = 30;
    /// <summary>
    /// Time for part 2
    /// </summary>
    private const int PART2_TIME = 26;

    [GeneratedRegex(@"Valve ([A-Z]{2}) has flow rate=(\d{1,2}); (?:tunnel leads to valve ([A-Z]{2})|tunnels lead to valves ([A-Z, ]+))", RegexOptions.Compiled)]
    private static partial Regex Pattern { get; }

    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver for 2022 - 16 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day16(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Precalculate all path lengths
        Dictionary<(Valve, Valve), int> pathLengthsTemp = new(this.Data.valves.Length * this.Data.valves.Length);
        foreach (int i in ..(this.Data.valves.Length - 1))
        {
            Valve from = this.Data.valves[i];
            foreach (int j in ^i..this.Data.valves.Length)
            {
                Valve to = this.Data.valves[j];
                int pathLength = SearchUtils.GetPathLengthBFS(from, to, v => v.Connections)!.Value + 1; // Add one for time to open valve
                pathLengthsTemp.Add((from, to), pathLength);
                pathLengthsTemp.Add((to, from), pathLength);
            }
        }

        // Setup helper collections
        FrozenDictionary<(Valve, Valve), int> pathLengths = pathLengthsTemp.ToFrozenDictionary();
        ImmutableArray<Valve> validValves = [..this.Data.valves.Where(v => v.FlowRate > 0)];

        // Part 1
        int pressure = ExploreTunnels(this.Data.start, PART1_TIME, pathLengths, validValves);
        AoCUtils.LogPart1(pressure);

        // Part 2
        pressure = ExploreTunnelsPair(this.Data.start, PART2_TIME, pathLengths, validValves);
        AoCUtils.LogPart2(pressure);
    }

    /// <summary>
    /// Explore tunnels and release valves
    /// </summary>
    /// <param name="current">Current valve</param>
    /// <param name="remainingTime">Remaining time</param>
    /// <param name="pathLengths">Paths length map</param>
    /// <param name="validValves">Valid valves array</param>
    /// <returns>The maximum released pressure</returns>
    private static int ExploreTunnels(Valve current, int remainingTime, FrozenDictionary<(Valve, Valve), int> pathLengths, ImmutableArray<Valve> validValves)
    {
        // Out of time
        if (remainingTime <= 2) return 0;

        int maxReleased = 0;
        foreach (Valve valve in validValves)
        {
            // Make sure the valve is valid
            if (!IsValidTarget(valve, current, remainingTime, pathLengths, out int distance)) continue;

            // Calculate time left and pressure released
            int timeLeftAfterMove = remainingTime - distance;
            int releasedByMove    = timeLeftAfterMove * valve.FlowRate;

            // Proceed with valve opening and then explore again
            valve.IsOpen = true;
            maxReleased  = Math.Max(maxReleased, ExploreTunnels(valve, timeLeftAfterMove, pathLengths, validValves) + releasedByMove);
            valve.IsOpen = false;
        }

        return maxReleased;
    }

    /// <summary>
    /// Explore tunnels in pairs and release valves
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="startTime">Starting allowed time</param>
    /// <param name="pathLengths">Paths length map</param>
    /// <param name="validValves">Valid valves array</param>
    /// <returns>The maximum pressure released</returns>
    private static int ExploreTunnelsPair(Valve start, int startTime, FrozenDictionary<(Valve, Valve), int> pathLengths, ImmutableArray<Valve> validValves)
    {
        int ExploreTunnelsPairInternal(Valve current, int remainingTime, int depth, bool isSecond)
        {
            // Out of valves to turn on
            if (depth == validValves.Length) return 0;

            // Check if we're out of time
            if (remainingTime <= 2)
            {
                // If we are the second explorer, that's it
                if (isSecond) return 0;

                // Else reset the current state and carry on
                current       = start;
                remainingTime = startTime;
                isSecond      = true;
            }

            int maxReleased = 0;
            bool depthHeuristic = !isSecond && depth >= validValves.Length / 3; // We at least want the first explorer to do *some* work
            foreach (Valve valve in validValves)
            {
                // Make sure the valve is valid
                if (!IsValidTarget(valve, current, remainingTime, pathLengths, out int distance)) continue;

                // Calculate time left and pressure released
                int timeLeftAfterMove = remainingTime - distance;
                int releasedByMove    = timeLeftAfterMove * valve.FlowRate;

                // Proceed with valve opening and then explore again
                valve.IsOpen = true;
                maxReleased  = Math.Max(maxReleased, ExploreTunnelsPairInternal(valve, timeLeftAfterMove, depth + 1, isSecond) + releasedByMove);
                if (depthHeuristic)
                {
                    // Try stopping early and going to the second explorer if enough work has been done
                    maxReleased = Math.Max(maxReleased, ExploreTunnelsPairInternal(start, startTime, depth + 1, true) + releasedByMove);
                }
                valve.IsOpen = false;
            }

            // Return best result
            return maxReleased;
        }

        // Initialize search
        return ExploreTunnelsPairInternal(start, startTime, 0, false);
    }

    /// <summary>
    /// Checks if a valve is a valid target to go to
    /// </summary>
    /// <param name="target">Target valve</param>
    /// <param name="current">Current valve</param>
    /// <param name="remainingTime">Remaining time</param>
    /// <param name="pathLengths">Paths length map</param>
    /// <param name="distance">Distance between valves output</param>
    /// <returns><see langword="true"/> if the target valve is valid, otherwise <see langword="false"/></returns>
    private static bool IsValidTarget(Valve target, Valve current, int remainingTime, FrozenDictionary<(Valve, Valve), int> pathLengths, out int distance)
    {
        // Make sure the valve is available
        if (target.IsOpen)
        {
            distance = 0;
            return false;
        }

        // And not so far that it's unreachable
        distance = pathLengths[(current, target)];
        return distance < remainingTime;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Valve, Valve[]) Convert(string[] lines)
    {
        Dictionary<string, Valve> valves = new(lines.Length);
        Dictionary<string, string[]> valveConnections = new(lines.Length);
        foreach (string line in lines)
        {
            // Parse valves
            string[] captures = Pattern.Match(line).CapturedGroups
                                       .Select(g => g.Value)
                                       .ToArray();
            string id    = captures[0];
            int flowRate = int.Parse(captures[1]);
            string[] connections = captures[2].Split(',', StringSplitOptions.TrimEntries);

            valves.Add(id, new Valve(id, flowRate, connections.Length));
            valveConnections.Add(id, connections);
        }

        foreach ((string id, Valve valve) in valves)
        {
            // Assign connections
            string[] connections = valveConnections[id];
            foreach (int i in ..connections.Length)
            {
                valve.Connections[i] = valves[connections[i]];
            }
        }

        return (valves["AA"], valves.Values.ToArray());
    }
}
