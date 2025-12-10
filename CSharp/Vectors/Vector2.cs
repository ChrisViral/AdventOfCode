using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Types;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

/// <summary>
/// Integer two component vector
/// </summary>
[PublicAPI]
public readonly partial struct Vector2<T> : IAdditionOperators<Vector2<T>, Vector2<T>, Vector2<T>>, ISubtractionOperators<Vector2<T>, Vector2<T>, Vector2<T>>,
                                            IUnaryNegationOperators<Vector2<T>, Vector2<T>>, IUnaryPlusOperators<Vector2<T>, Vector2<T>>,
                                            IComparisonOperators<Vector2<T>, Vector2<T>, bool>, IMinMaxValue<Vector2<T>>,
                                            IDivisionOperators<Vector2<T>, T, Vector2<T>>, IMultiplyOperators<Vector2<T>, T, Vector2<T>>,
                                            IModulusOperators<Vector2<T>, T, Vector2<T>>, IModulusOperators<Vector2<T>, Vector2<T>, Vector2<T>>,
                                            IComparable<Vector2<T>>, IEquatable<Vector2<T>>, IFormattable
    where T : IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = !IsInteger ? T.CreateChecked(1E-5) : T.Zero;
    /// <summary>Zero vector</summary>
    public static readonly Vector2<T> Zero  = new(T.Zero, T.Zero);
    /// <summary>One vector</summary>
    public static readonly Vector2<T> One   = new(T.One, T.One);
    /// <summary>Up vector</summary>
    public static readonly Vector2<T> Up    = new(T.Zero, -T.One);
    /// <summary>Down vector</summary>
    public static readonly Vector2<T> Down  = new(T.Zero, T.One);
    /// <summary>Left vector</summary>
    public static readonly Vector2<T> Left  = new(-T.One, T.Zero);
    /// <summary>Right vector</summary>
    public static readonly Vector2<T> Right = new(T.One, T.Zero);

    /// <summary>
    /// Regex direction match
    /// </summary>
    [GeneratedRegex(@"^\s*(U|N|D|S|L|W|R|E)\s*(\d+)\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex DirectionMatch { get; }
    /// <summary>
    /// Minimum vector value
    /// </summary>
    public static Vector2<T> MinValue { get; } = new(T.MinValue, T.MinValue);
    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector2<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue);

    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T X { get; init; }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public T Y { get; init; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    public double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetLength<double>();
    }

    /// <summary>
    /// Absolute length of both vector components summed
    /// </summary>
    public T ManhattanLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.Abs(this.X) + T.Abs(this.Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2{T}"/> with the specified components
    /// </summary>
    /// <param name="x">X component</param>
    /// <param name="y">Y component</param>
    public Vector2(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2{T}"/> from a given two component tuple
    /// </summary>
    /// <param name="tuple">Tuple to create the Vector from</param>
    public Vector2((T x, T y) tuple) : this(tuple.x, tuple.y) { }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector2(Vector2<T> copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? other) => other is Vector2<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector2<T> other) => IsInteger
                                                ? this.X == other.X && this.Y == other.Y
                                                : Approximately(this.X, other.X) && Approximately(this.Y, other.Y);

    /// <inheritdoc cref="object.GetHashCode"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? other) => other is Vector2<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Vector2<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"({this.X}, {this.Y})";

    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? provider) => $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)})";

    /// <summary>
    /// Deconstructs this vector into a tuple
    /// </summary>
    /// <param name="x">X parameter</param>
    /// <param name="y">Y parameter</param>
    public void Deconstruct(out T x, out T y)
    {
        x = this.X;
        y = this.Y;
    }

    /// <summary>
    /// Gets the length of this vector in the specified floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point type to get the length in</typeparam>
    /// <returns>The length of the vector</returns>
    public TResult GetLength<TResult>() where TResult : IBinaryFloatingPointIeee754<TResult>, IMinMaxValue<TResult>
    {
        Vector2<TResult> resultVector = Vector2<TResult>.CreateChecked(this);
        return TResult.Sqrt((resultVector.X * resultVector.X) + (resultVector.Y * resultVector.Y));
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<Vector2<T>>.Equals(Vector2<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int IComparable<Vector2<T>>.CompareTo(Vector2<T> other) => CompareTo(other);

    /// <summary>
    /// Converts the vector to the target type
    /// </summary>
    /// <typeparam name="TSource">Source number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> CreateChecked<TSource>(Vector2<TSource> value)
        where TSource : IBinaryNumber<TSource>, IMinMaxValue<TSource>
    {
        return new Vector2<T>(T.CreateChecked(value.X), T.CreateChecked(value.Y));
    }

    /// <summary>
    /// Calculates the distance between two vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The distance between both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Distance(Vector2<T> a, Vector2<T> b) => (a - b).Length;

    /// <summary>
    /// The Manhattan distance between both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Tge straight line distance between both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ManhattanDistance(Vector2<T> a, Vector2<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y);

    /// <summary>
    /// Calculates the signed angle, in degrees, between two vectors. The result is in the range [-180, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle Angle(Vector2<T> a, Vector2<T> b)
    {
        (double aX, double aY) = Vector2<double>.CreateChecked(a);
        (double bX, double bY) = Vector2<double>.CreateChecked(b);
        return Vectors.Angle.FromRadians(Math.Atan2(aX * bY - aY * bX, aX * bX + aY * bY));
    }

    /// <summary>
    /// Calculates the absolute angle, in degrees, between two vectors. The result is in the range [0, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle AbsoluteAngle(Vector2<T> a, Vector2<T> b)
    {
        double dot = Vector2<double>.Dot(Vector2<double>.CreateChecked(a), Vector2<double>.CreateChecked(b));
        return Vectors.Angle.FromRadians(Math.Acos(dot / (a.Length * b.Length)));
    }

    /// <summary>
    /// Gives the absolute value of a given vector
    /// </summary>
    /// <param name="vector">Vector to get the absolute value of</param>
    /// <returns>The <paramref name="vector"/> where all it's elements are positive</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Abs(Vector2<T> vector) => new(T.Abs(vector.X), T.Abs(vector.Y));

    /// <summary>
    /// Does component-wise multiplication on the vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The multiplied vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> ComponentMultiply(Vector2<T> a, Vector2<T> b) => new(a.X * b.X, a.Y * b.Y);

    /// <summary>
    /// Calculates the cross product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The cross product of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cross(Vector2<T> a, Vector2<T> b) => (a.X * b.Y) - (a.Y * b.X);

    /// <summary>
    /// Calculates the dot product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The dot product of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot(Vector2<T> a, Vector2<T> b) => a.X * b.X + a.Y * b.Y;

    /// <summary>
    /// Gets the minimum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component minimum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Min(Vector2<T> a, Vector2<T> b) => new(T.Min(a.X, b.X), T.Min(a.Y, b.Y));

    /// <summary>
    /// Gets the maximum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component maximum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> Max(Vector2<T> a, Vector2<T> b) => new(T.Max(a.X, b.X), T.Max(a.Y, b.Y));

    /// <summary>
    /// Parses the Vector2 from a direction and distance
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>The parsed Vector2</returns>
    /// <exception cref="FormatException">If the direction format or string format is invalid</exception>
    /// <exception cref="OverflowException">If the number parse causes an overflow</exception>
    public static Vector2<T> ParseFromDirection(string value)
    {
        GroupCollection groups = DirectionMatch.Match(value).Groups;
        //Parse direction first
        Vector2<T> direction = groups[1].Value switch
        {
            "U" => Up,
            "N" => Up,
            "D" => Down,
            "S" => Down,
            "L" => Left,
            "W" => Left,
            "R" => Right,
            "E" => Right,
            _   => throw new FormatException($"Direction value ({groups[1].Value}) cannot be parsed into a direction")
        };

        //Return with correct length
        return direction * T.Parse(groups[2].ValueSpan, NumberStyles.Number, null);
    }

    /// <summary>
    /// Tries to parses the Vector2 from a direction and distance
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <param name="direction">Result out parameter</param>
    /// <returns>True if the vector was successfully parsed, false otherwise</returns>
    public static bool TryParseFromDirection(string value, out Vector2<T> direction)
    {
        //Check if it matches at all
        direction = Zero;
        Match match = DirectionMatch.Match(value);
        if (!match.Success) return false;

        GroupCollection groups = match.Groups;
        if (groups.Count is not 3) return false;
        if (!T.TryParse(groups[2].ValueSpan, NumberStyles.Number, null, out T? distance)) return false;
        Vector2<T> dir;
        switch (groups[1].Value)
        {
            case "U":
            case "N":
                dir = Up;
                break;
            case "D":
            case "S":
                dir = Down;
                break;
            case "L":
            case "W":
                dir = Left;
                break;
            case "R":
            case "E":
                dir = Right;
                break;

            default:
                return false;
        }

        direction = dir * distance;
        return true;
    }

    /// <summary>
    /// Enumerates all the points in a square at a certain distance from the point given
    /// </summary>
    /// <param name="from">Point to enumerate around</param>
    /// <param name="distance">Distance from the point to start at</param>
    /// <returns>An enumerable of all the points at the given distance</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the distance is less than zero</exception>
    public static IEnumerable<Vector2<T>> EnumerateAtDistance(Vector2<T> from, T distance)
    {
        if (distance <= T.Zero) throw new ArgumentOutOfRangeException(nameof(distance), "Distance must be greater than zero");

        Vector2<T> position = new(from.X - distance, from.Y);
        Vector2<T> direction = Up + Right;
        do
        {
            yield return position;
            position += direction;
        }
        while (position.X != from.X);

        direction = Down + Right;
        do
        {
            yield return position;
            position += direction;
        }
        while (position.Y != from.Y);

        direction = Down + Left;
        do
        {
            yield return position;
            position += direction;
        }
        while (position.X != from.X);

        direction = Up + Left;
        do
        {
            yield return position;
            position += direction;
        }
        while (position.Y != from.Y);
    }

    /// <summary>
    /// Parses the two component vector using the given value and number separator
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two values present after the split</exception>
    public static Vector2<T> Parse(string value, string separator = ",")
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value), "Value cannot be null or empty");
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        return Parse(value.AsSpan(), separator);
    }

    /// <summary>
    /// Parses the two component vector using the given value and number separator
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two values present after the split</exception>
    public static Vector2<T> Parse(ReadOnlySpan<char> value, string separator = ",")
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Value cannot be empty", nameof(value));
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        Span<Range> ranges = stackalloc Range[2];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is not 2) throw new FormatException("String to parse not properly formatted");

        T x = T.Parse(value[ranges[0]], null);
        T y = T.Parse(value[ranges[1]], null);
        return new Vector2<T>(x, y);
    }

    /// <summary>
    /// Tries to parse the two component vector using the given value and returns the success
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="result">Resulting vector, if any</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns><see langword="true"/> if the parse succeeded, otherwise <see langword="false"/></returns>
    public static bool TryParse(string? value, out Vector2<T> result, string separator = ",")
    {
        if (string.IsNullOrEmpty(value))
        {
            result = Zero;
            return false;
        }

        return TryParse(value.AsSpan(), out result, separator);
    }

    /// <summary>
    /// Tries to parse the two component vector using the given value and returns the success
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="result">Resulting vector, if any</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns><see langword="true"/> if the parse succeeded, otherwise <see langword="false"/></returns>
    public static bool TryParse(ReadOnlySpan<char> value, out Vector2<T> result, string separator = ",")
    {
        if (value.IsEmpty || value.IsWhiteSpace() || string.IsNullOrEmpty(separator))
        {
            result = Zero;
            return false;
        }

        Span<Range> ranges = stackalloc Range[2];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is not 2
         || !T.TryParse(value[ranges[0]], null, out T? x)
         || !T.TryParse(value[ranges[1]], null, out T? y))
        {
            result = Zero;
            return false;
        }

        result = new Vector2<T>(x, y);
        return true;
    }

    /// <summary>
    /// Greatest Common Divisor function
    /// </summary>
    /// <param name="a">First number</param>
    /// <param name="b">Second number</param>
    /// <returns>Gets the GCD of a and b</returns>
    private static T GCD(T a, T b)
    {
        a = T.Abs(a);
        b = T.Abs(b);
        while (a != T.Zero && b != T.Zero)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a | b;
    }

    /// <summary>
    /// Compares two numbers to see if they are nearly identical
    /// </summary>
    /// <param name="a">First number to test</param>
    /// <param name="b">Second number to test</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> and <paramref name="b"/> are approximately equal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Approximately(T a, T b) => T.Abs(a - b) <= Epsilon;

    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="Vector2"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector2<T>((T x, T y) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector2{T}"/> to <see cref="ValueTuple{T1, T2}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator (T x, T y)(Vector2<T> vector) => (vector.X, vector.Y);

    /// <summary>
    /// Equality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are equal, false otherwise</returns>
    public static bool operator ==(Vector2<T> a, Vector2<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector2<T> a, Vector2<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator -(Vector2<T> a) => new(-a.X, -a.Y);

    /// <summary>
    /// Plus operation on a vector
    /// </summary>
    /// <param name="a">Vector to apply the plus to</param>
    /// <returns>The vector where all the components had the plus operator applied to</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator +(Vector2<T> a) => new(+a.X, +a.Y);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Add operation between a vector and a direction
    /// </summary>
    /// <param name="a">Vector</param>
    /// <param name="b">Direction</param>
    /// <returns>The result of the movement of the vector in the given direction</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator +(Vector2<T> a, Direction b) => a + b.ToVector<T>();

    /// <summary>
    /// Subtract operation between a vector and a direction
    /// </summary>
    /// <param name="a">Vector</param>
    /// <param name="b">Direction</param>
    /// <returns>The result of the movement of the vector in the reversed given direction</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator -(Vector2<T> a, Direction b) => a - b.ToVector<T>();

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(Vector2<T> a, T b) => new(a.X * b, a.Y * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator /(Vector2<T> a, T b) => new(a.X / b, a.Y / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator %(Vector2<T> a, T b) => new(a.X % b, a.Y % b);

    /// <summary>
    /// Per component modulo operator
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Vector containing which values to modulo the components by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator %(Vector2<T> a, Vector2<T> b) => new(a.X % b.X, a.Y % b.Y);
}
