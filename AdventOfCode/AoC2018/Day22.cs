using System.ComponentModel;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Spans;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 22
/// </summary>
public sealed class Day22 : Solver<(int depth, Vector2<int> target)>
{
    private enum Region
    {
        ROCKY  = 0,
        WET    = 1,
        NARROW = 2
    }

    private enum Gear
    {
        NONE,
        TORCH,
        CLIMBING
    }

    private readonly record struct SearchState(Vector2<int> Position, Gear Gear);

    private const int XMUL = 16807;
    private const int YMUL = 48271;
    private const int MOD  = 20183;
    private const int SWITCH_DELAY = 7;

    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day22(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Add a buffer size around
        Vector2<int> dimensions = this.Data.target + (SWITCH_DELAY * 3, SWITCH_DELAY * 3);
        Grid<int> erosionLevels = new(dimensions.X, dimensions.Y);
        Grid<Region> map = new(dimensions.X, dimensions.Y);
        foreach (Vector2<int> position in map.Dimensions.Enumerate())
        {
            // Calculate geologic index
            int geologicIndex = position switch
            {
                { X: 0, Y: 0 }                        => 0,
                { } when position == this.Data.target => 0,
                { Y: 0 }                              => (position.X * XMUL) % MOD,
                { X: 0 }                              => (position.Y * YMUL) % MOD,
                _                                     => (erosionLevels[position.X - 1, position.Y]
                                                        * erosionLevels[position.X, position.Y - 1]) % MOD
            };

            // Calculate erosion level
            int erosionLevel = (geologicIndex + this.Data.depth) % MOD;
            erosionLevels[position] = erosionLevel;

            // Set region type
            map[position] = (Region)(erosionLevel % 3);
        }

        // Get risk level across map
        int riskLevel = map.AsSpan2D(this.Data.target.X + 1, this.Data.target.Y + 1)
                           .Sum(t => (int)t);
        AoCUtils.LogPart1(riskLevel);

        // Search path to target
        SearchState start = new(Vector2<int>.Zero, Gear.TORCH);
        SearchState goal  = new(this.Data.target, Gear.TORCH);
        SearchUtils.Search(start, goal, null,
                           s => FindTargetRegions(s, map),
                           MinSearchComparer<int>.Comparer,
                           out int totalTime);
        AoCUtils.LogPart2(totalTime);
    }

    // ReSharper disable once CognitiveComplexity
    private static IEnumerable<MoveData<SearchState, int>> FindTargetRegions(SearchState state, Grid<Region> map)
    {
        // Get which gear we could switch to
        Region currentRegion = map[state.Position];
        Gear gearSwitch = currentRegion switch
        {
            Region.ROCKY => state.Gear switch
            {
                Gear.NONE     => throw new InvalidOperationException("Invalid gear for the region"),
                Gear.TORCH    => Gear.CLIMBING,
                Gear.CLIMBING => Gear.TORCH,
                _             => throw new InvalidEnumArgumentException(nameof(state.Gear), (int)state.Gear, typeof(Gear))
            },
            Region.WET => state.Gear switch
            {
                Gear.NONE     => Gear.CLIMBING,
                Gear.TORCH    => throw new InvalidOperationException("Invalid gear for the region"),
                Gear.CLIMBING => Gear.NONE,
                _             => throw new InvalidEnumArgumentException(nameof(state.Gear), (int)state.Gear, typeof(Gear))
            },
            Region.NARROW => state.Gear switch
            {
                Gear.NONE     => Gear.TORCH,
                Gear.TORCH    => Gear.NONE,
                Gear.CLIMBING => throw new InvalidOperationException("Invalid gear for the region"),
                _             => throw new InvalidEnumArgumentException(nameof(state.Gear), (int)state.Gear, typeof(Gear))
            },
            _ => throw new InvalidEnumArgumentException(nameof(currentRegion), (int)currentRegion, typeof(Region))
        };

        // Return gear switch move
        yield return new MoveData<SearchState, int>(state with { Gear = gearSwitch }, SWITCH_DELAY);

        // Get adjacent regions we could move to
        foreach (Vector2<int> adjacent in state.Position.AsAdjacentEnumerable())
        {
            // Get adjacent region type
            if (!map.TryGetPosition(adjacent, out Region targetRegion)) continue;

            // Return valid moves to adjacent regions
            switch (targetRegion)
            {
                case Region.ROCKY:
                    if (state.Gear is not Gear.NONE) yield return new MoveData<SearchState, int>(state with { Position = adjacent });
                    continue;

                case Region.WET:
                    if (state.Gear is not Gear.TORCH) yield return new MoveData<SearchState, int>(state with { Position = adjacent });
                    continue;

                case Region.NARROW:
                    if (state.Gear is not Gear.CLIMBING) yield return new MoveData<SearchState, int>(state with { Position = adjacent });
                    continue;

                default:
                    throw new InvalidEnumArgumentException(nameof(targetRegion), (int)targetRegion, typeof(Region));
            }
        }
    }

    /// <inheritdoc />
    protected override (int, Vector2<int>) Convert(string[] rawInput)
    {
        int depth = int.Parse(rawInput[0].AsSpan(7));
        Vector2<int> target = Vector2<int>.Parse(rawInput[1].AsSpan(8));
        return (depth, target);
    }
}
