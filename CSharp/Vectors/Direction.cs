using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;

namespace AdventOfCode.Vectors;

/// <summary>
/// Cardinal Directions
/// </summary>
public enum Direction
{
    NONE  = 0b0000,
    UP    = 0b0101,
    DOWN  = 0b0110,
    LEFT  = 0b1001,
    RIGHT = 0b1010,
    NORTH = UP,
    SOUTH = DOWN,
    EAST  = RIGHT,
    WEST  = LEFT
}

/// <summary>
/// Direction extension methods
/// </summary>
public static class DirectionsUtils
{
    /// <summary> Vertical direction mask </summary>
    public const int VERTICAL_MASK = 0b0100;
    /// <summary> Horizontal direction mask </summary>
    public const int HORIZONTAL_MASK = 0b1000;

    #region Static properties
    /// <summary>
    /// All possible directions
    /// </summary>
    public static ReadOnlyCollection<Direction> AllDirections { get; } = new(
    [
        Direction.UP,
        Direction.DOWN,
        Direction.LEFT,
        Direction.RIGHT
    ]);
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
    public static Direction Parse(string value)
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
    public static Direction Parse(char value) => Parse(new ReadOnlySpan<char>(ref value));

    /// <summary>
    /// Parses the given char span into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <returns>The parsed direction</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
    /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
    public static Direction Parse(in ReadOnlySpan<char> value)
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Parse value cannot be empty", nameof(value));

        ReadOnlySpan<char> trimmed = value.Trim();
        Span<char> lowered = stackalloc char[trimmed.Length];
        trimmed.ToLowerInvariant(lowered);
        return lowered switch
        {
            "u" or "up"    or "n" or "north" => Direction.UP,
            "d" or "down"  or "s" or "south" => Direction.DOWN,
            "l" or "left"  or "e" or "east"  => Direction.LEFT,
            "r" or "right" or "w" or "west"  => Direction.RIGHT,
            _                                => throw new FormatException("Direction could not properly be parsed from input")
        };
    }

    /// <summary>
    /// Tries to parse the given string into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(string value, out Direction direction) => TryParse(value.AsSpan(), out direction);

    /// <summary>
    /// Tries to parse the given char into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(char value, out Direction direction) => TryParse(new ReadOnlySpan<char>(ref value), out direction);

    /// <summary>
    /// Tries to parse the given char span into a direction
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="direction">The parsed direction output</param>
    /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
    public static bool TryParse(in ReadOnlySpan<char> value, out Direction direction)
    {
        if (value.IsEmpty || value.IsWhiteSpace())
        {
            direction = Direction.NONE;
            return false;
        }

        ReadOnlySpan<char> trimmed = value.Trim();
        Span<char> lowered = stackalloc char[trimmed.Length];
        trimmed.ToLowerInvariant(lowered);
        switch (lowered)
        {
            case "u" or "up" or "n" or "north":
                direction = Direction.UP;
                return true;

            case "d" or "down" or "s" or "south":
                direction = Direction.DOWN;
                return true;

            case "l" or "left" or "e" or "east":
                direction = Direction.LEFT;
                return true;

            case "r" or "right" or "w" or "west":
                direction = Direction.RIGHT;
                return true;

            default:
                direction = Direction.NONE;
                return false;
        }
    }
    #endregion

    #region Extension methods
    /// <summary>
    /// Gets a Vector2 from a given Direction
    /// </summary>
    /// <param name="direction">Direction to get the vector from</param>
    /// <returns>The resulting vector</returns>
    public static Vector2<T> ToVector<T>(this Direction direction) where T : IBinaryNumber<T>, IMinMaxValue<T> => direction switch
    {
        Direction.UP    => Vector2<T>.Up,
        Direction.DOWN  => Vector2<T>.Down,
        Direction.LEFT  => Vector2<T>.Left,
        Direction.RIGHT => Vector2<T>.Right,
        _                => Vector2<T>.Zero
    };

    /// <summary>
    /// Gets a Vector2 from a given Direction
    /// </summary>
    /// <param name="direction">Direction to get the vector from</param>
    /// <param name="length">DThe length of the direction vector</param>
    /// <returns>The resulting vector</returns>
    public static Vector2<T> ToVector<T>(this Direction direction, T length) where T : IBinaryNumber<T>, IMinMaxValue<T> => direction switch
    {
        Direction.UP    => new(T.Zero, -length),
        Direction.DOWN  => new(T.Zero,  length),
        Direction.LEFT  => new(-length, T.Zero),
        Direction.RIGHT => new(length,  T.Zero),
        _                => Vector2<T>.Zero
    };

    /// <summary>
    /// Inverts the direction
    /// </summary>
    /// <param name="direction">Direction to invert</param>
    /// <returns>Reverse direction from the current one</returns>
    public static Direction Invert(this Direction direction) => direction switch
    {
        Direction.UP    => Direction.DOWN,
        Direction.DOWN  => Direction.UP,
        Direction.LEFT  => Direction.RIGHT,
        Direction.RIGHT => Direction.LEFT,
        _                => direction
    };

    /// <summary>
    /// Turns the direction towards the left
    /// </summary>
    /// <param name="direction">Direction to turn</param>
    /// <returns>The new direction after turning to the left</returns>
    public static Direction TurnLeft(this Direction direction) => direction switch
    {
        Direction.NONE  => Direction.NONE,
        Direction.UP    => Direction.LEFT,
        Direction.LEFT  => Direction.DOWN,
        Direction.DOWN  => Direction.RIGHT,
        Direction.RIGHT => Direction.UP,
        _                => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
    };

    /// <summary>
    /// Turns the direction towards the right
    /// </summary>
    /// <param name="direction">Direction to turn</param>
    /// <returns>The new direction after turning to the right</returns>
    public static Direction TurnRight(this Direction direction) => direction switch
    {
        Direction.NONE  => Direction.NONE,
        Direction.UP    => Direction.RIGHT,
        Direction.RIGHT => Direction.DOWN,
        Direction.DOWN  => Direction.LEFT,
        Direction.LEFT  => Direction.UP,
        _                => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
    };

    /// <summary>
    /// Checks if the given direction is vertical or not
    /// </summary>
    /// <param name="direction">Direction to check</param>
    /// <returns><see langword="true"/> if the direction is vertical, else <see langword="false"/></returns>
    public static bool IsVertical(this Direction direction) => ((int)direction & VERTICAL_MASK) is not 0;

    /// <summary>
    /// Checks if the given direction is horizontal or not
    /// </summary>
    /// <param name="direction">Direction to check</param>
    /// <returns><see langword="true"/> if the direction is horizontal, else <see langword="false"/></returns>
    public static bool IsHorizontal(this Direction direction) => ((int)direction & HORIZONTAL_MASK) is not 0;
    #endregion
}