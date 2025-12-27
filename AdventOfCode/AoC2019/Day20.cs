using System.Collections.Frozen;
using AdventOfCode.Collections;
using AdventOfCode.Collections.Search;
using AdventOfCode.Utils.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enums;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 20
/// </summary>
public sealed class Day20 : Solver<Day20.MapData>
{
    public enum Element
    {
        NONE     = 0,
        EMPTY    = Day20.EMPTY,
        WALL     = '#',
        TELEPORT = '@',
        ENTRANCE = 'X'
    }

    private const char EMPTY = '.';

    /// <summary>
    /// Position structure also indicating depth level
    /// </summary>
    /// <param name="Position">Position in maze</param>
    /// <param name="Depth">Depth level</param>
    private readonly record struct LayeredPosition(Vector2<int> Position, int Depth);

    /// <summary>
    /// Map data
    /// </summary>
    /// <param name="Grid">Map grid</param>
    /// <param name="Teleporters">Teleporter mapping</param>
    /// <param name="Start">Start point</param>
    /// <param name="End">End point</param>
    public sealed record MapData(Grid<Element> Grid, FrozenDictionary<Vector2<int>, Vector2<int>> Teleporters, Vector2<int> Start, Vector2<int> End);

    /// <summary>
    /// Teleporter structure
    /// </summary>
    /// <param name="Label">Teleporter label</param>
    /// <param name="From">First teleporter location</param>
    /// <param name="To">Second teleporter location</param>
    private record struct Teleporter(string Label, Vector2<int> From, Vector2<int> To);

    /// <summary>
    /// Creates a new <see cref="Day20"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day20(string input) : base(input, options: StringSplitOptions.RemoveEmptyEntries) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int? path = SearchUtils.GetPathLength(this.Data.Start, this.Data.End, null, GetNeighbours, MinSearchComparer<int>.Comparer);
        AoCUtils.LogPart1(path!.Value);

        path = SearchUtils.GetPathLength(new LayeredPosition(this.Data.Start, 0), new LayeredPosition(this.Data.End, 0),
                                         null, LayeredNeighbours, MinSearchComparer<int>.Comparer);
        AoCUtils.LogPart2(path!.Value);
    }

    // ReSharper disable once CognitiveComplexity
    private IEnumerable<MoveData<LayeredPosition, int>> LayeredNeighbours(LayeredPosition current)
    {
        foreach (Vector2<int> adjacent in current.Position.AsAdjacentEnumerable())
        {
            if (this.Data.Grid.TryGetPosition(adjacent, out Element value))
            {
                switch (value)
                {
                    case Element.NONE:
                        // If in empty middle, check teleporters and go one layer deeper
                        if (this.Data.Teleporters.TryGetValue(current.Position, out Vector2<int> teleported))
                        {
                            yield return new MoveData<LayeredPosition, int>(new LayeredPosition(teleported, current.Depth + 1), 1);
                        }
                        break;

                    case not Element.WALL:
                        // Normal movement
                        yield return new MoveData<LayeredPosition, int>(current with { Position = adjacent }, 1);
                        break;
                }
            }
            // If on outside, check teleporters if not in outermost level, and go one layer up
            else if (current.Depth is not 0 && this.Data.Teleporters.TryGetValue(current.Position, out Vector2<int> teleported))
            {
                yield return new MoveData<LayeredPosition, int>(new LayeredPosition(teleported, current.Depth - 1), 1);
            }
        }
    }

    private IEnumerable<MoveData<Vector2<int>, int>> GetNeighbours(Vector2<int> current)
    {
        foreach (Vector2<int> adjacent in current.AsAdjacentEnumerable())
        {
            if (this.Data.Grid.TryGetPosition(adjacent, out Element value) && value is not Element.NONE)
            {
                // If within grid, not in empty middle, and not wall, normal movement
                if (value is not Element.WALL)
                {
                    yield return new MoveData<Vector2<int>, int>(adjacent, 1);
                }
            }
            // If outside grid or in empty middle, check teleproters
            else if (this.Data.Teleporters.TryGetValue(current, out Vector2<int> teleported))
            {
                yield return new MoveData<Vector2<int>, int>(teleported, 1);
            }
        }
    }

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
    protected override MapData Convert(string[] rawInput)
    {
        // Get size for proper grid
        int width  = rawInput[0].Length - 4;
        int height = rawInput.Length - 4;
        Grid<Element> grid = new(width, height, ElementToString);

        // Parse raws properly
        ReadOnlySpan<string> rawInputs = rawInput.AsSpan(2, height);
        foreach (int y in ..height)
        {
            Span<Element> row = grid[y];
            ReadOnlySpan<char> line = rawInputs[y].AsSpan(2, width);
            foreach (int x in ..width)
            {
                char c = line[x];
                row[x] = !char.IsLetter(c) && !char.IsWhiteSpace(c) ? (Element)c : Element.NONE;
            }
        }

        // Parse teleporters
        Vector2<int> offset = new(2, 2);
        Dictionary<string, Teleporter> teleporters = [];
        Grid<char> rawGrid = new(width + 4, height + 4, rawInput, line => line.ToCharArray());
        foreach (int y in 1..^(height + 2))
        {
            foreach (int x in 1..^(width + 2))
            {
                if (!ParseTeleporter(rawGrid, new Vector2<int>(x, y), out string label, out Vector2<int> teleportPosition)) continue;

                if (!teleporters.TryGetValue(label, out Teleporter teleporter))
                {
                    teleporter = new Teleporter(label, teleportPosition - offset, Vector2<int>.Zero);
                    teleporters.Add(label, teleporter);
                }
                else
                {
                    teleporters[label] = teleporter with { To = teleportPosition - offset };
                }
            }
        }

        // Setup teleporters
        Vector2<int> start = Vector2<int>.Zero, end = Vector2<int>.Zero;
        Dictionary<Vector2<int>, Vector2<int>> teleportMap = new(teleporters.Count * 2);
        foreach (Teleporter teleporter in teleporters.Values)
        {
            switch (teleporter.Label)
            {
                case "AA":
                    start = teleporter.From;
                    grid[teleporter.From] = Element.ENTRANCE;
                    break;

                case "ZZ":
                    end = teleporter.From;
                    grid[teleporter.From] = Element.ENTRANCE;
                    break;

                default:
                    teleportMap.Add(teleporter.From, teleporter.To);
                    teleportMap.Add(teleporter.To, teleporter.From);
                    grid[teleporter.From] = Element.TELEPORT;
                    grid[teleporter.To]   = Element.TELEPORT;
                    break;
            }
        }

        return new MapData(grid, teleportMap.ToFrozenDictionary(), start, end);
    }

    private static bool ParseTeleporter(Grid<char> rawGrid, Vector2<int> position, out string label, out Vector2<int> teleportPosition)
    {
        char value = rawGrid[position];
        if (!char.IsLetter(value))
        {
            label = string.Empty;
            teleportPosition = Vector2<int>.Zero;
            return false;
        }

        // Teleporter on a right edge
        Vector2<int> edgePosition = position + Vector2<int>.Left;
        if (rawGrid[edgePosition] is EMPTY)
        {
            label = new string([value, rawGrid[position + Vector2<int>.Right]]);
            teleportPosition = edgePosition;
            return true;
        }

        // Teleporter on a left edge
        edgePosition = position + Vector2<int>.Right;
        if (rawGrid[edgePosition] is EMPTY)
        {
            label = new string([rawGrid[position + Vector2<int>.Left], value]);
            teleportPosition = edgePosition;
            return true;
        }

        // Teleporter on a bottom edge
        edgePosition = position + Vector2<int>.Up;
        if (rawGrid[edgePosition] is EMPTY)
        {
            label = new string([value, rawGrid[position + Vector2<int>.Down]]);
            teleportPosition = edgePosition;
            return true;
        }

        // Teleporter on a top edge
        edgePosition = position + Vector2<int>.Down;
        if (rawGrid[edgePosition] is EMPTY)
        {
            label = new string([rawGrid[position + Vector2<int>.Up], value]);
            teleportPosition = edgePosition;
            return true;
        }

        // None found, invalid position
        label = string.Empty;
        teleportPosition = Vector2<int>.Zero;
        return false;
    }

    private static string ElementToString(Element element) => element switch
    {
        Element.NONE     => " ",
        Element.EMPTY    => "░",
        Element.WALL     => "▓",
        Element.TELEPORT => "@",
        Element.ENTRANCE => "X",
        _                => throw element.Invalid()
    };
}
