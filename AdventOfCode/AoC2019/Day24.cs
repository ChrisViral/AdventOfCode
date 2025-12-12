using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Collections;
using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Maths.Vectors.BitVectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using CommunityToolkit.HighPerformance;
using MemoryExtensions = System.MemoryExtensions;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 24
/// </summary>
public sealed class Day24 : GridSolver<bool>
{
    private const int LOOPS = 200;

    private readonly Grid<bool> emptyTemplate;
    private readonly Vector2<int> center;

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="System.InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day24(string input) : base(input)
    {
        this.emptyTemplate = new Grid<bool>(this.Grid);
        this.emptyTemplate.Clear();
        this.center = this.Grid.Dimensions / 2;
    }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Copy grid and setup states map
        DelayedGrid<bool> current = new(this.Grid);
        HashSet<BitVector32> states = new(100);

        // Setup initial state and loop until a match is found
        BitVector32 latest = GridToBitVector(current);
        while(states.Add(latest))
        {
            // Update map, apply changes, then get state
            UpdateBugs(current);
            current.Apply();
            latest = GridToBitVector(current);
        }
        AoCUtils.LogPart1(latest.Data);

        // Create levels map
        Dictionary<int, DelayedGrid<bool>> levels = new(100)
        {
            [-1] = new DelayedGrid<bool>(this.emptyTemplate),
            [0]  = new DelayedGrid<bool>(this.Grid),
            [1]  = new DelayedGrid<bool>(this.emptyTemplate),
        };

        // Create levels update queue
        Queue<int> levelsToUpdate = new(100);
        foreach (int _ in ..LOOPS)
        {
            // Enqueue all current levels and update until nothing is left
            levels.Keys.ForEach(levelsToUpdate.Enqueue);
            while (levelsToUpdate.TryDequeue(out int level))
            {
                UpdateLevel(level, levels, levelsToUpdate);
            }
            // Apply each level after being done updating
            levels.Values.ForEach(map => map.Apply());
        }

        // Sum
        int bugs = levels.Values.Sum(map => map.AsSpan2D().Count(true));
        AoCUtils.LogPart2(bugs);
    }

    private static void UpdateBugs(DelayedGrid<bool> current)
    {
        foreach (Vector2<int> position in current.Dimensions.Enumerate())
        {
            bool hasBug = current[position];
            int surrounding = position.AsAdjacentEnumerable()
                                      .Where(current.WithinGrid)
                                      .Count(p => current[p]);
            if (hasBug)
            {
                current[position] = surrounding is 1;
            }
            else
            {
                current[position] = surrounding is 1 or 2;
            }
        }
    }

    private static BitVector32 GridToBitVector(DelayedGrid<bool> grid)
    {
        ReadOnlySpan2D<bool> data = grid.AsSpan2D();
        if (data.TryGetSpan(out ReadOnlySpan<bool> bitArray))
        {
            return BitVector32.FromBitArray(bitArray);
        }

        int i = 0;
        BitVector32 result = new();
        foreach (bool value in data)
        {
            result[i++] = value;
        }
        return result;
    }

    // ReSharper disable once CognitiveComplexity
    private void UpdateLevel(int level, Dictionary<int, DelayedGrid<bool>> levels, Queue<int> levelsToUpdate)
    {
        // Enumerate through the level
        DelayedGrid<bool> levelMap = levels[level];
        foreach (Vector2<int> position in levelMap.Dimensions.Enumerate())
        {
            // Ignore central square
            if (position == this.center) continue;

            bool hasBug = levelMap[position];
            int surrounding = 0;
            // Check all four surrounding directions
            foreach (Direction checkDirection in Direction.CardinalDirections)
            {
                Vector2<int> adjacent = position + checkDirection;
                if (adjacent == this.center)
                {
                    // In the center, check one level up
                    if (TryGetLevel(hasBug, level + 1, levels, levelsToUpdate, out DelayedGrid<bool>? insideMap))
                    {
                        surrounding += CountSurroundingInside(checkDirection, insideMap);
                    }
                }
                else if (!levelMap.WithinGrid(adjacent))
                {
                    // On the outside, check one level down
                    if (TryGetLevel(hasBug, level - 1, levels, levelsToUpdate, out DelayedGrid<bool>? outsideMap))
                    {
                        surrounding += outsideMap[this.center + checkDirection] ? 1 : 0;
                    }
                }
                else
                {
                    // Everywhere else, normal check
                    surrounding += levelMap[adjacent] ? 1 : 0;
                }
            }

            // Set new state
            if (hasBug)
            {
                levelMap[position] = surrounding is 1;
            }
            else
            {
                levelMap[position] = surrounding is 1 or 2;
            }
        }
    }

    private static int CountSurroundingInside(Direction checkDirection, DelayedGrid<bool> levelMap) => checkDirection switch
    {
        Direction.UP    => MemoryExtensions.Count(levelMap.GetRow(^1), true),
        Direction.DOWN  => MemoryExtensions.Count(levelMap.GetRow(0), true),
        Direction.LEFT  => levelMap.GetColumn(^1).Count(true),
        Direction.RIGHT => levelMap.GetColumn(0).Count(true),
        Direction.NONE  => throw new InvalidOperationException("None is not a valid check direction"),
        _               => throw new InvalidEnumArgumentException(nameof(checkDirection), (int)checkDirection, typeof(Direction))
    };

    private bool TryGetLevel(bool hasBug, int level, Dictionary<int, DelayedGrid<bool>> levels, Queue<int> levelsToUpdate, [NotNullWhen(true)] out DelayedGrid<bool>? levelMap)
    {
        if (!levels.TryGetValue(level, out levelMap))
        {
            // If there is no bug on the tile, we don't need to generate the new map yet
            if (!hasBug) return false;

            // Else, create new map and enqueue it
            levelMap = new DelayedGrid<bool>(this.emptyTemplate);
            levels.Add(level, levelMap);
            levelsToUpdate.Enqueue(level);
        }
        return true;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();

    /// <inheritdoc />
    protected override string StringConversion(bool value) => value ? "#" : ".";
}
