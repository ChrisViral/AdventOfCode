using System.ComponentModel;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 24
/// </summary>
public sealed partial class Day24 : Solver<Day24.Neighbour[][]>
{
    /// <summary>
    /// Hax grid neighbours
    /// </summary>
    public enum Neighbour
    {
        EAST,
        WEST,
        NORTH_EAST,
        NORTH_WEST,
        SOUTH_EAST,
        SOUTH_WEST
    }

    /// <summary>
    /// Neighbour pattern
    /// </summary>
    [GeneratedRegex("[ns]?[ew]")]
    private static partial Regex Matcher { get; }

    /// <summary>
    /// Total iterations of part 2
    /// </summary>
    private const int ITERATIONS = 100;
    /// <summary>
    /// String to Neighbour conversion
    /// </summary>
    private static readonly Dictionary<string, Neighbour> ToNeighbour = new(6)
    {
        ["e"]  = Neighbour.EAST,
        ["w"]  = Neighbour.WEST,
        ["ne"] = Neighbour.NORTH_EAST,
        ["nw"] = Neighbour.NORTH_WEST,
        ["se"] = Neighbour.SOUTH_EAST,
        ["sw"] = Neighbour.SOUTH_WEST
    };

    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Neighbour"/>[][] fails</exception>
    public Day24(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Get all the flipped tiles
        HashSet<Vector2<int>> flipped = [];
        foreach (Neighbour[] path in this.Data)
        {
            //Start at zero, and move into each direction
            Vector2<int> pos = Vector2<int>.Zero;
            foreach (Neighbour direction in path)
            {
                pos += direction switch
                {
                    Neighbour.EAST       => Vector2<int>.Left,
                    Neighbour.WEST       => Vector2<int>.Right,
                    Neighbour.NORTH_EAST => Vector2<int>.Left + Vector2<int>.Up,
                    Neighbour.NORTH_WEST => Vector2<int>.Up,
                    Neighbour.SOUTH_EAST => Vector2<int>.Down,
                    Neighbour.SOUTH_WEST => Vector2<int>.Right + Direction.DOWN,
                    _                    => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Neighbour))
                };
            }

            //Add to flipped
            if (!flipped.Add(pos))
            {
                //If already in, remove
                flipped.Remove(pos);
            }
        }
        AoCUtils.LogPart1(flipped.Count);

        //Setup new stated and updated tiles
        HashSet<Vector2<int>> newState = [];
        HashSet<Vector2<int>> updated = [];
        foreach (int _ in ..ITERATIONS)
        {
            //Get all the updated tiles
            updated.UnionWith(flipped);
            updated.UnionWith(flipped.SelectMany(Neighbours));
            //Get new status for all updated
            foreach (Vector2<int> tile in updated)
            {
                //Get surrounding flipped tiles
                int surrounding = Neighbours(tile).Count(flipped.Contains);
                //If flipped
                if (flipped.Contains(tile))
                {
                    //Check if there is one or two active neighbour
                    if (surrounding is 1 or 2)
                    {
                        //If so stay flipped
                        newState.Add(tile);
                    }
                }
                else if (surrounding is 2)
                {
                    //Else flip if has two neighbours
                    newState.Add(tile);
                }
            }

            //Swap and clear
            (flipped, newState) = (newState, flipped);
            newState.Clear();
            updated.Clear();
        }
        AoCUtils.LogPart2(flipped.Count);
    }

    /// <summary>
    /// Gets all the neighbouring positions in the hex grid for a given position
    /// </summary>
    /// <param name="position">Position to get the neighbours of</param>
    /// <returns>All siz neighbours of the given position in an enumerable</returns>
    private static IEnumerable<Vector2<int>> Neighbours(Vector2<int> position)
    {
        yield return position + Vector2<int>.Left;                 //East
        yield return position + Vector2<int>.Right;                //West
        yield return position + Vector2<int>.Left + Vector2<int>.Up;    //NorthEast
        yield return position + Vector2<int>.Up;                   //NorthWest
        yield return position + Vector2<int>.Down;                 //SouthEast
        yield return position + Vector2<int>.Right + Vector2<int>.Down; //SouthWest
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Neighbour[][] Convert(string[] rawInput)
    {
        Neighbour[][] directions = new Neighbour[rawInput.Length][];
        foreach (int i in ..directions.Length)
        {
            directions[i] = Matcher.Matches(rawInput[i]).Select(m => ToNeighbour[m.Value]).ToArray();
        }

        return directions;
    }
}
