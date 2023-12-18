using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;

namespace AdventOfCode.Vectors;

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
    EAST  = RIGHT,
    WEST  = LEFT
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
    public static ReadOnlyCollection<Directions> AllDirections { get; } = new(new[]
    {
        Directions.UP,
        Directions.DOWN,
        Directions.LEFT,
        Directions.RIGHT
    });
    #endregion

    #region Static Methods
    /// <summary>
    /// Parses the given string into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed direction</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> is null</exception>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
    /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
    public static Directions Parse(string value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "Parse value is null");

        return Parse(value.AsSpan());
    }

    /// <summary>
    /// Parses the given char into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed direction</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
    /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
    public static Directions Parse(char value) => Parse(new ReadOnlySpan<char>(ref value));

    /// <summary>
    /// Parses the given char span into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed direction</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
    /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
    public static Directions Parse(in ReadOnlySpan<char> value)
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Parse value cannot be empty", nameof(value));

        ReadOnlySpan<char> trimmed = value.Trim();
        Span<char> lowered = stackalloc char[trimmed.Length];
        trimmed.ToLowerInvariant(lowered);
        return lowered switch
        {
            "u" or "up"    or "n" or "north" => Directions.UP,
            "d" or "down"  or "s" or "south" => Directions.DOWN,
            "l" or "left"  or "e" or "east"  => Directions.LEFT,
            "r" or "right" or "w" or "west"  => Directions.RIGHT,
            _                                => throw new FormatException("Direction could not properly be parsed from input")
        };
    }

    /// <summary>
    /// Tries to parse the given string into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(string value, out Directions direction) => TryParse(value.AsSpan(), out direction);

    /// <summary>
    /// Tries to parse the given char into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(char value, out Directions direction) => TryParse(new ReadOnlySpan<char>(ref value), out direction);

    /// <summary>
    /// Tries to parse the given char span into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(in ReadOnlySpan<char> value, out Directions direction)
    {
        if (value.IsEmpty || value.IsWhiteSpace())
        {
            direction = Directions.NONE;
            return false;
        }

        ReadOnlySpan<char> trimmed = value.Trim();
        Span<char> lowered = stackalloc char[trimmed.Length];
        trimmed.ToLowerInvariant(lowered);
        switch (lowered)
        {
            case "u" or "up" or "n" or "north":
                direction = Directions.UP;
                return true;

            case "d" or "down" or "s" or "south":
                direction = Directions.DOWN;
                return true;

            case "l" or "left" or "e" or "east":
                direction = Directions.LEFT;
                return true;

            case "r" or "right" or "w" or "west":
                direction = Directions.RIGHT;
                return true;

            default:
                direction = Directions.NONE;
                return false;
        }
    }
    #endregion

    #region Extension methods
    /// <summary>
    /// Gets a Vector2 from a given Directions
    /// </summary>
    /// <param name="directions">Direction to get the vector from</param>
    /// <returns>The resulting vector</returns>
    public static Vector2<T> ToVector<T>(this Directions directions) where T : IBinaryNumber<T>, IMinMaxValue<T> => directions switch
    {
        Directions.UP    => Vector2<T>.Up,
        Directions.DOWN  => Vector2<T>.Down,
        Directions.LEFT  => Vector2<T>.Left,
        Directions.RIGHT => Vector2<T>.Right,
        _                => Vector2<T>.Zero
    };

    /// <summary>
    /// Gets a Vector2 from a given Directions
    /// </summary>
    /// <param name="directions">Direction to get the vector from</param>
    /// <param name="length">DThe length of the direction vector</param>
    /// <returns>The resulting vector</returns>
    public static Vector2<T> ToVector<T>(this Directions directions, T length) where T : IBinaryNumber<T>, IMinMaxValue<T> => directions switch
    {
        Directions.UP    => new(T.Zero, -length),
        Directions.DOWN  => new(T.Zero,  length),
        Directions.LEFT  => new(-length, T.Zero),
        Directions.RIGHT => new(length,  T.Zero),
        _                => Vector2<T>.Zero
    };

    /// <summary>
    /// Inverts the direction
    /// </summary>
    /// <param name="directions">Direction to invert</param>
    /// <returns>Reverse direction from the current one</returns>
    public static Directions Invert(this Directions directions) => directions switch
    {
        Directions.UP    => Directions.DOWN,
        Directions.DOWN  => Directions.UP,
        Directions.LEFT  => Directions.RIGHT,
        Directions.RIGHT => Directions.LEFT,
        _                => directions
    };

    /// <summary>
    /// Turns the direction towards the left
    /// </summary>
    /// <param name="directions">Direction to turn</param>
    /// <returns>The new direction after turning to the left</returns>
    public static Directions TurnLeft(this Directions directions) => directions switch
    {
        Directions.NONE  => Directions.NONE,
        Directions.UP    => Directions.LEFT,
        Directions.LEFT  => Directions.DOWN,
        Directions.DOWN  => Directions.RIGHT,
        Directions.RIGHT => Directions.UP,
        _                => throw new InvalidEnumArgumentException(nameof(directions), (int)directions, typeof(Directions))
    };

    /// <summary>
    /// Turns the direction towards the right
    /// </summary>
    /// <param name="directions">Direction to turn</param>
    /// <returns>The new direction after turning to the right</returns>
    public static Directions TurnRight(this Directions directions) => directions switch
    {
        Directions.NONE  => Directions.NONE,
        Directions.UP    => Directions.RIGHT,
        Directions.RIGHT => Directions.DOWN,
        Directions.DOWN  => Directions.LEFT,
        Directions.LEFT  => Directions.UP,
        _                => throw new InvalidEnumArgumentException(nameof(directions), (int)directions, typeof(Directions))
    };
    #endregion
}