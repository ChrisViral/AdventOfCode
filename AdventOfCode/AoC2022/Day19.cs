using System.Text.RegularExpressions;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 19
/// </summary>
public sealed partial class Day19 : Solver<Day19.Blueprint[]>
{
    /// <summary>
    /// Robot building blueprint
    /// </summary>
    /// <param name="ID">Blueprint ID</param>
    /// <param name="OreCost">Cost to build an Ore robot</param>
    /// <param name="ClayCost">Cost to build a Clay robot</param>
    /// <param name="ObsidianOreCost">Ore cost to build an Obsidian robot</param>
    /// <param name="ObsidianClayCost">Clay cost to build an Obsidian robot</param>
    /// <param name="GeodeOreCost">Ore cost to build a Geode robot</param>
    /// <param name="GeodeObsidianCost">Obsidian cost to build a Geode robot</param>
    public sealed record Blueprint(int ID, int OreCost, int ClayCost, int ObsidianOreCost, int ObsidianClayCost, int GeodeOreCost, int GeodeObsidianCost)
    {
        /// <summary>
        /// Search state struct
        /// </summary>
        /// <param name="Ore">Current ore total</param>
        /// <param name="Clay">Current clay total</param>
        /// <param name="Obsidian">Current obsidian total</param>
        /// <param name="Geode">Current geode total</param>
        /// <param name="OreRobots">Ore robots count</param>
        /// <param name="ClayRobots">Clay robots count</param>
        /// <param name="ObsidianRobots">Obsidian robots count</param>
        /// <param name="GeodeRobots">Geode robots count</param>
        private readonly record struct State(int Ore, int Clay, int Obsidian, int Geode, int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots)
        {
            /// <summary>
            /// Default search state
            /// </summary>
            public static readonly State Default = new() { OreRobots = 1 };

            /// <summary>
            /// Updates this state by one time increment
            /// </summary>
            /// <returns>The new, updated state</returns>
            public State Update() => this with
            {
                Ore      = this.Ore + this.OreRobots,
                Clay     = this.Clay + this.ClayRobots,
                Obsidian = this.Obsidian + this.ObsidianRobots,
                Geode    = this.Geode + this.GeodeRobots
            };
        }

        /// <summary>
        /// Cache for the total amount of opened geodes in the previous search
        /// </summary>
        public int OpenedGeodesCache { get; private set; }

        /// <summary>
        /// Calculates the maximum amount of opened geodes in a certain time frame by this blueprint
        /// </summary>
        /// <param name="maxTime">Maximum allotted time</param>
        public void CalculateMaxOpenedGeodes(int maxTime)
        {
            int GetMaxOpenedGeodesInternal(int time, in State state)
            {
                // Tick down time
                State newState = state.Update();
                time--;
                if (time is 0) return newState.Geode;

                // Create geode robot
                if (state.Ore >= this.GeodeOreCost && state.Obsidian >= this.GeodeObsidianCost)
                {
                    // If we can make a geode robot, always make one
                    State withNewGeodeRobot = newState with
                    {
                        Ore         = newState.Ore - this.GeodeOreCost,
                        Obsidian    = newState.Obsidian - this.GeodeObsidianCost,
                        GeodeRobots = newState.GeodeRobots + 1
                    };
                    return Math.Max(newState.Geode, GetMaxOpenedGeodesInternal(time, withNewGeodeRobot));
                }

                int maxGeodes = newState.Geode;
                // Do nothing
                if (state.Ore < 5)
                {
                    // Only if we have a lot amount of ore, otherwise always buy a robot
                    maxGeodes = Math.Max(newState.Geode, GetMaxOpenedGeodesInternal(time, newState));
                }

                // Create ore robot
                if (state.Ore >= this.OreCost && state.OreRobots < 4)
                {
                    // No need to have more than four of these as the cost per minute is never greater than four
                    State withNewOreRobot = newState with
                    {
                        Ore       = newState.Ore - this.OreCost,
                        OreRobots = newState.OreRobots + 1
                    };
                    maxGeodes = Math.Max(maxGeodes, GetMaxOpenedGeodesInternal(time, withNewOreRobot));
                }

                // Create clay robot
                if (state.Ore >= this.ClayCost)
                {
                    State withNewClayRobot = newState with
                    {
                        Ore        = newState.Ore - this.ClayCost,
                        ClayRobots = newState.ClayRobots + 1
                    };
                    maxGeodes = Math.Max(maxGeodes, GetMaxOpenedGeodesInternal(time, withNewClayRobot));
                }

                // Create obsidian robot
                if (state.Ore >= this.ObsidianOreCost && state.Clay >= this.ObsidianClayCost)
                {
                    State withNewObsidianRobot = newState with
                    {
                        Ore            = newState.Ore - this.ObsidianOreCost,
                        Clay           = newState.Clay - this.ObsidianClayCost,
                        ObsidianRobots = newState.ObsidianRobots + 1
                    };
                    maxGeodes = Math.Max(maxGeodes, GetMaxOpenedGeodesInternal(time, withNewObsidianRobot));
                }

                return maxGeodes;
            }

            this.OpenedGeodesCache = GetMaxOpenedGeodesInternal(maxTime, State.Default);
            AoCUtils.Log($"Blueprint {this.ID} opened geodes: {this.OpenedGeodesCache}");
        }
    }

    /// <summary>
    /// Regex match
    /// </summary>
    [GeneratedRegex(@"Blueprint (\d+): Each ore robot costs (\d+) ore\. Each clay robot costs (\d+) ore\. Each obsidian robot costs (\d+) ore and (\d+) clay\. Each geode robot costs (\d+) ore and (\d+) obsidian\.")]
    private static partial Regex Matcher { get; }

    /// <summary>Allotted time for the first part</summary>
    private const int FIRST_TIME  = 24;
    /// <summary>Allotted time for the second part</summary>
    private const int SECOND_TIME = 32;

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver for 2022 - 19 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc cref="Solver{T}.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Process all blueprints
        Parallel.ForEach(this.Data, b => b.CalculateMaxOpenedGeodes(FIRST_TIME));
        int qualityLevels = this.Data.Sum(b => b.OpenedGeodesCache * b.ID);
        AoCUtils.LogPart1(qualityLevels);

        // Process first three blueprints blueprints
        Blueprint[] remaining = this.Data[..3];
        Parallel.ForEach(remaining, b => b.CalculateMaxOpenedGeodes(SECOND_TIME));
        int maxOpened = remaining.Multiply(b => b.OpenedGeodesCache);
        AoCUtils.LogPart2(maxOpened);
    }

    /// <inheritdoc />
    protected override Blueprint[] Convert(string[] lines) => RegexFactory<Blueprint>.ConstructObjects(Matcher, lines);
}
