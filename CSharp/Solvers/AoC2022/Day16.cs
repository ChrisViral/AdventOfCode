using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Search;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2022;

/// <summary>
/// Solver for 2022 Day 16
/// </summary>
public class Day16 : Solver<Dictionary<string, Day16.Valve>>
{
    public class Valve : IEquatable<Valve>
    {
        public string Name { get; }

        public int FlowRate { get; }

        public Valve[] Connections { get; }

        public bool IsOpen { get; set; }

        public bool CanRelease => !this.IsOpen && this.FlowRate > 0;

        private readonly string[] connections;

        public Valve(string name, int flowRate, string[] connections)
        {
            this.Name        = name;
            this.FlowRate    = flowRate;
            this.connections = connections;
            this.Connections = new Valve[connections.Length];
        }

        public void SetupConnections(Dictionary<string, Valve> valves)
        {
            foreach (int i in ..this.Connections.Length)
            {
                this.Connections[i] = valves[this.connections[i]];
            }
        }

        public bool Equals(Valve? other) => other?.Name == this.Name;

        public override string ToString() => $"{this.Name} ({this.FlowRate})";
    }

    private static readonly Regex pattern = new(@"Valve ([A-Z]{2}) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z, ]+)", RegexOptions.Compiled);

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver for 2022 - 16 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day16(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver{T}.Run"/>
    public override void Run()
    {
        int pressure = ExploreTunnels(this.Data["AA"], 0, 30, new());
        AoCUtils.LogPart1(pressure);

        AoCUtils.LogPart2("");
    }

    private static IEnumerable<(Valve value, double distance)> Neighbours(Valve node) => node.Connections.Select(connection => (connection, 1d));

    private int ExploreTunnels(Valve current, int currentPressure, int time, Dictionary<(Valve, Valve), int> distances)
    {
        if (time == 0) return currentPressure;

        int maxReleased = currentPressure;
        foreach (Valve valve in this.Data.Values.Where(v => v.CanRelease))
        {
            int timeSpent = SearchUtils.GetPathLengthBFS(current, valve, Neighbours)!.Value + 1;
            distances.Clear();
            if (timeSpent > time) continue;

            int timeLeft = time - timeSpent;
            int released = timeLeft * valve.FlowRate;
            valve.IsOpen = true;
            maxReleased  = Math.Max(maxReleased, ExploreTunnels(valve, currentPressure + released, timeLeft, distances));
            valve.IsOpen = false;
        }

        return maxReleased;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Dictionary<string, Valve> Convert(string[] lines)
    {
        Dictionary<string, Valve> valves = new(lines.Length);
        foreach (string line in lines)
        {
            string[] captures = pattern.Match(line)
                                       .GetCapturedGroups()
                                       .Select(g => g.Value)
                                       .ToArray();
            string name          = captures[0];
            int flowRate         = int.Parse(captures[1]);
            string[] connections = captures[2].Split(',', DEFAULT_OPTIONS);
            valves.Add(name, new(name, flowRate, connections));
        }

        valves.Values.ForEach(v => v.SetupConnections(valves));
        return valves;
    }
    #endregion
}

