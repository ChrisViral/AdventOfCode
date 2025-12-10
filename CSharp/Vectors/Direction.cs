using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

/// <summary>
/// Cardinal Directions
/// </summary>
[PublicAPI]
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
[PublicAPI]
public static class DirectionsUtils
{
    /// <summary> Vertical direction mask </summary>
    public const int VERTICAL_MASK = 0b0100;
    /// <summary> Horizontal direction mask </summary>
    public const int HORIZONTAL_MASK = 0b1000;

    /// <summary>
    /// All possible directions
    /// </summary>
    public static ImmutableArray<Direction> CardinalDirections { get; } =
    [
        Direction.UP,
        Direction.DOWN,
        Direction.LEFT,
        Direction.RIGHT
    ];

    extension(Direction direction)
    {
        /// <summary>
        /// Gets a Vector2 from a given Direction
        /// </summary>
        /// <returns>The resulting vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2<T> ToVector<T>() where T : IBinaryNumber<T>, IMinMaxValue<T> => direction switch
        {
            Direction.UP    => Vector2<T>.Up,
            Direction.DOWN  => Vector2<T>.Down,
            Direction.LEFT  => Vector2<T>.Left,
            Direction.RIGHT => Vector2<T>.Right,
            _               => Vector2<T>.Zero
        };

        /// <summary>
        /// Gets a Vector2 from a given Direction
        /// </summary>
        /// <param name="length">DThe length of the direction vector</param>
        /// <returns>The resulting vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2<T> ToVector<T>(T length) where T : IBinaryNumber<T>, IMinMaxValue<T> => direction switch
        {
            Direction.UP    => new Vector2<T>(T.Zero, -length),
            Direction.DOWN  => new Vector2<T>(T.Zero,  length),
            Direction.LEFT  => new Vector2<T>(-length, T.Zero),
            Direction.RIGHT => new Vector2<T>(length,  T.Zero),
            _               => Vector2<T>.Zero
        };

        /// <summary>
        /// Inverts the direction
        /// </summary>
        /// <returns>Reverse direction from the current one</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction Invert() => direction switch
        {
            Direction.UP    => Direction.DOWN,
            Direction.DOWN  => Direction.UP,
            Direction.LEFT  => Direction.RIGHT,
            Direction.RIGHT => Direction.LEFT,
            _               => direction
        };

        /// <summary>
        /// Turns the direction towards the left
        /// </summary>
        /// <returns>The new direction after turning to the left</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction TurnLeft() => direction switch
        {
            Direction.NONE  => Direction.NONE,
            Direction.UP    => Direction.LEFT,
            Direction.LEFT  => Direction.DOWN,
            Direction.DOWN  => Direction.RIGHT,
            Direction.RIGHT => Direction.UP,
            _               => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
        };

        /// <summary>
        /// Turns the direction towards the right
        /// </summary>
        /// <returns>The new direction after turning to the right</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction TurnRight() => direction switch
        {
            Direction.NONE  => Direction.NONE,
            Direction.UP    => Direction.RIGHT,
            Direction.RIGHT => Direction.DOWN,
            Direction.DOWN  => Direction.LEFT,
            Direction.LEFT  => Direction.UP,
            _               => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
        };

        /// <summary>
        /// Turns the direction vector in te specified direction
        /// </summary>
        /// <param name="turn">Direction to turn into</param>
        /// <returns>The new direction after turning by the specified direction</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Direction TurnBy(Direction turn) => turn switch
        {
            Direction.NONE  => direction,
            Direction.UP    => direction,
            Direction.RIGHT => direction.TurnRight(),
            Direction.DOWN  => direction.Invert(),
            Direction.LEFT  => direction.TurnLeft(),
            _               => throw new InvalidEnumArgumentException(nameof(direction), (int)direction, typeof(Direction))
        };

        /// <summary>
        /// Checks if the given direction is vertical or not
        /// </summary>
        /// <returns><see langword="true"/> if the direction is vertical, else <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsVertical() => ((int)direction & VERTICAL_MASK) is not 0;

        /// <summary>
        /// Checks if the given direction is horizontal or not
        /// </summary>
        /// <returns><see langword="true"/> if the direction is horizontal, else <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsHorizontal() => ((int)direction & HORIZONTAL_MASK) is not 0;
    }

    extension(Direction)
    {
        /// <summary>
        /// Parses the given char into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed direction</returns>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
        /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction Parse(char value) => char.ToLowerInvariant(value) switch
        {
            'u' or 'n' or '^' => Direction.UP,
            'd' or 's' or 'v' => Direction.DOWN,
            'l' or 'e' or '<' => Direction.LEFT,
            'r' or 'w' or '>' => Direction.RIGHT,
            _                 => throw new FormatException("Direction could not properly be parsed from input")
        };

        /// <summary>
        /// Parses the given string into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed direction</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
        /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction Parse(string value) => value is not null
                                                           ? Parse(value.AsSpan())
                                                           : throw new ArgumentNullException(nameof(value), "Parse value is null");

        /// <summary>
        /// Parses the given char span into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>The parsed direction</returns>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is empty or whitespace</exception>
        /// <exception cref="FormatException">If <paramref name="value"/> is not a valid Direction string</exception>
        public static Direction Parse(ReadOnlySpan<char> value)
        {
            if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Parse value cannot be empty", nameof(value));

            ReadOnlySpan<char> trimmed = value.Trim();
            Span<char> lowered = stackalloc char[trimmed.Length];
            trimmed.ToLowerInvariant(lowered);
            return lowered switch
            {
                "u" or "up"    or "n" or "north" or "^" => Direction.UP,
                "d" or "down"  or "s" or "south" or "v" => Direction.DOWN,
                "l" or "left"  or "e" or "east"  or "<" => Direction.LEFT,
                "r" or "right" or "w" or "west"  or ">" => Direction.RIGHT,
                _                                       => throw new FormatException("Direction could not properly be parsed from input")
            };
        }

        /// <summary>
        /// Tries to parse the given char into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="direction">The parsed direction output</param>
        /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
        public static bool TryParse(char value, out Direction direction)
        {
            if (value is char.MinValue || char.IsWhiteSpace(value))
            {
                direction = Direction.NONE;
                return false;
            }

            switch (char.ToLowerInvariant(value))
            {
                case 'u' or 'n' or '^':
                    direction = Direction.UP;
                    return true;

                case 'd' or 's' or 'v':
                    direction = Direction.DOWN;
                    return true;

                case 'l' or 'e' or '<':
                    direction = Direction.LEFT;
                    return true;

                case 'r' or 'w' or '>':
                    direction = Direction.RIGHT;
                    return true;

                default:
                    direction = Direction.NONE;
                    return false;
            }
        }

        /// <summary>
        /// Tries to parse the given string into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="direction">The parsed direction output</param>
        /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(string value, out Direction direction) => TryParse(value.AsSpan(), out direction);

        /// <summary>
        /// Tries to parse the given char span into a direction
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <param name="direction">The parsed direction output</param>
        /// <returns><see langword="true"/> if the value was successfully parsed, otherwise <see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Direction direction)
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
                case "u" or "up" or "n" or "north" or "^":
                    direction = Direction.UP;
                    return true;

                case "d" or "down" or "s" or "south" or "v":
                    direction = Direction.DOWN;
                    return true;

                case "l" or "left" or "e" or "east" or ">":
                    direction = Direction.LEFT;
                    return true;

                case "r" or "right" or "w" or "west" or "<":
                    direction = Direction.RIGHT;
                    return true;

                default:
                    direction = Direction.NONE;
                    return false;
            }
        }
    }
}
