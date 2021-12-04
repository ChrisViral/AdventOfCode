using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using AdventOfCode.Grids.Vectors;

namespace AdventOfCode.Grids;

/// <summary>
/// Cardinal Directions
/// </summary>
public enum Directions
{
    NONE  = 0,
    UP    = 1,
    DOWN  = 2,
    LEFT  = 3,
    RIGHT = 4,
    NORTH = UP,
    SOUTH = DOWN,
    EAST  = LEFT,
    WEST  = RIGHT
}

/// <summary>
/// Directions extension methods
/// </summary>
public static class DirectionsUtils
{
    #region Static properties
    /// <summary>
    /// All possible directions
    /// </summary>
    public static ReadOnlyCollection<Directions> AllDirections { get; } = new(new[] { Directions.UP, Directions.DOWN, Directions.LEFT, Directions.RIGHT });
    #endregion

    #region Extension methods
    /// <summary>
    /// Gets a Vector2 from a given Directions
    /// </summary>
    /// <param name="directions">Direction to get the vector from</param>
    /// <returns>The resulting vector</returns>
    public static Vector2<T> ToVector<T>(this Directions directions) where T : IBinaryNumber<T>, IMinMaxValue<T>
    {
        return directions switch
        {
            Directions.UP    => Vector2<T>.Up,
            Directions.DOWN  => Vector2<T>.Down,
            Directions.LEFT  => Vector2<T>.Left,
            Directions.RIGHT => Vector2<T>.Right,
            _                => Vector2<T>.Zero,
        };
    }

    /// <summary>
    /// Inverts the direction
    /// </summary>
    /// <param name="directions">Direction to invert</param>
    /// <returns>Reverse direction from the current one</returns>
    public static Directions Invert(this Directions directions)
    {
        return directions switch
        {
            Directions.UP    => Directions.DOWN,
            Directions.DOWN  => Directions.UP,
            Directions.LEFT  => Directions.RIGHT,
            Directions.RIGHT => Directions.LEFT,
            _                => directions
        };
    }

    /// <summary>
    /// Turns the direction towards the left
    /// </summary>
    /// <param name="directions">Direction to turn</param>
    /// <returns>The new direction after turning to the left</returns>
    public static Directions TurnLeft(this Directions directions)
    {
        return directions switch
        {
            Directions.NONE  => Directions.NONE,
            Directions.UP    => Directions.LEFT,
            Directions.LEFT  => Directions.DOWN,
            Directions.DOWN  => Directions.RIGHT,
            Directions.RIGHT => Directions.UP,
            _                => throw new InvalidEnumArgumentException(nameof(directions), (int)directions, typeof(Directions))
        };
    }

    /// <summary>
    /// Turns the direction towards the right
    /// </summary>
    /// <param name="directions">Direction to turn</param>
    /// <returns>The new direction after turning to the right</returns>
    public static Directions TurnRight(this Directions directions)
    {
        return directions switch
        {
            Directions.NONE  => Directions.NONE,
            Directions.UP    => Directions.RIGHT,
            Directions.RIGHT => Directions.DOWN,
            Directions.DOWN  => Directions.LEFT,
            Directions.LEFT  => Directions.UP,
            _                => throw new InvalidEnumArgumentException(nameof(directions), (int)directions, typeof(Directions))
        };
    }
    #endregion
}