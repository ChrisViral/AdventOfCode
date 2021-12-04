using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Grids;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using Vector2 = AdventOfCode.Grids.Vectors.Vector2<int>;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 24
/// </summary>
public class Day24 : Solver<Day24.Neighbour[][]>
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

    #region Constants
    /// <summary>
    /// Neighbour pattern
    /// </summary>
    private const string PATTERN = "[ns]?[ew]";
    /// <summary>
    /// Total iterations of part 2
    /// </summary>
    private const int ITERATIONS = 100;
    /// <summary>
    /// String to Neighbour conversion
    /// </summary>
    private static readonly Dictionary<string, Neighbour> toNeighbour = new(6)
    {
        ["e"]  = Neighbour.EAST,
        ["w"]  = Neighbour.WEST,
        ["ne"] = Neighbour.NORTH_EAST,
        ["nw"] = Neighbour.NORTH_WEST,
        ["se"] = Neighbour.SOUTH_EAST,
        ["sw"] = Neighbour.SOUTH_WEST
    };
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day24"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Neighbour"/>[][] fails</exception>
    public Day24(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        //Get all the flipped tiles
        HashSet<Vector2> flipped = new();
        foreach (Neighbour[] path in this.Data)
        {
            //Start at zero, and move into each direction
            Vector2 pos = Vector2.Zero;
            foreach (Neighbour direction in path)
            {
                pos += direction switch
                {
                    Neighbour.EAST       => Vector2.Left,
                    Neighbour.WEST       => Vector2.Right,
                    Neighbour.NORTH_EAST => Vector2.Left + Vector2.Up,
                    Neighbour.NORTH_WEST => Vector2.Up,
                    Neighbour.SOUTH_EAST => Vector2.Down,
                    Neighbour.SOUTH_WEST => Vector2.Right + Directions.DOWN,
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
        HashSet<Vector2> newState = new();
        HashSet<Vector2> updated = new();
        foreach (int _ in ..ITERATIONS)
        {
            //Get all the updated tiles
            updated.UnionWith(flipped);
            updated.UnionWith(flipped.SelectMany(Neighbours));
            //Get new status for all updated
            foreach (Vector2 tile in updated)
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
    private static IEnumerable<Vector2> Neighbours(Vector2 position)
    {
        yield return position + Vector2.Left;                 //East
        yield return position + Vector2.Right;                //West
        yield return position + Vector2.Left + Vector2.Up;    //NorthEast
        yield return position + Vector2.Up;                   //NorthWest
        yield return position + Vector2.Down;                 //SouthEast
        yield return position + Vector2.Right + Vector2.Down; //SouthWest
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Neighbour[][] Convert(string[] rawInput)
    {
        Regex match = new(PATTERN, RegexOptions.Compiled);
        Neighbour[][] directions = new Neighbour[rawInput.Length][];
        foreach (int i in ..directions.Length)
        {
            directions[i] = match.Matches(rawInput[i]).Select(m => toNeighbour[m.Value]).ToArray();
        }

        return directions;
    }
    #endregion
}