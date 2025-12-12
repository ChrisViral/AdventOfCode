using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions.Types;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Integer two component vector
/// </summary>
[PublicAPI]
public readonly partial struct Vector2<T> : IVector<Vector2<T>, T>, IDivisionOperators<Vector2<T>, T, Vector2<T>>, IMultiplyOperators<Vector2<T>, T, Vector2<T>>,
                                            IModulusOperators<Vector2<T>, T, Vector2<T>>, ICrossProductOperator<Vector2<T>, T, T>
    where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = !IsInteger ? T.CreateChecked(1E-5) : T.Zero;
    /// <summary>Up vector</summary>
    public static readonly Vector2<T> Up    = new(T.Zero, -T.One);
    /// <summary>Down vector</summary>
    public static readonly Vector2<T> Down  = new(T.Zero, T.One);
    /// <summary>Left vector</summary>
    public static readonly Vector2<T> Left  = new(-T.One, T.Zero);
    /// <summary>Right vector</summary>
    public static readonly Vector2<T> Right = new(T.One, T.Zero);

    /// <summary>
    /// Zero vector
    /// </summary>
    public static Vector2<T> Zero { get; } = new(T.Zero, T.Zero);

    /// <summary>
    /// One vector
    /// </summary>
    public static Vector2<T> One { get; } = new(T.One, T.One);

    /// <inheritdoc />
    public static int Dimension => 2;

    /// <summary>
    /// Minimum vector value
    /// </summary>
    public static Vector2<T> MinValue { get; } = new(T.MinValue, T.MinValue);

    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector2<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue);

    /// <summary>
    /// Regex direction match
    /// </summary>
    [GeneratedRegex(@"^\s*(U|N|D|S|L|W|R|E)\s*(\d+)\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex DirectionMatch { get; }

    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T X { get; init; }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public T Y { get; init; }

    /// <inheritdoc />
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index switch
        {
            0 => this.X,
            1 => this.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Vector dimension out of range")
        };
    }

    /// <inheritdoc />
    public T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index.GetOffset(Dimension) switch
        {
            0 => this.X,
            1 => this.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Vector dimension out of range")
        };
    }

    /// <inheritdoc />
    public double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetLength<double>();
    }

    /// <inheritdoc />
    public T ManhattanLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.Abs(this.X) + T.Abs(this.Y);
    }

    /// <summary>
    /// Vector swizzling to YX
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector2<T> YX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.Y, this.X);
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
    /// Vector constructor from a components span
    /// </summary>
    /// <param name="components">Span of components</param>
    /// <exception cref="ArgumentException">If the length of <paramref name="components"/> is greater than <see cref="Dimension"/></exception>
    public Vector2(ReadOnlySpan<T> components)
    {
        if (components.Length > Dimension) throw new ArgumentException("Components span cannot be larger than vector dimensions", nameof(components));

        switch (components.Length)
        {
            case 1:
                this.X = components[0];
                break;

            case 2:
                this.X = components[0];
                this.Y = components[1];
                break;

            default:
                throw new UnreachableException("Invalid component dimensions");
        }
    }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector2(Vector2<T> copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? other) => other is Vector2<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector2<T> other) => IsInteger
                                                ? this.X == other.X && this.Y == other.Y
                                                : Approximately(this.X, other.X) && Approximately(this.Y, other.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? other) => other is Vector2<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Vector2<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"({this.X}, {this.Y})";

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? provider) => $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)})";

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        if (destination.Length < 6) return false;

        destination[0] = '(';
        destination = destination[1..];
        if (!this.X.TryFormat(destination, out int xWritten, format, provider)) return false;

        destination = destination[xWritten..];
        if (destination.Length < 4) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Y.TryFormat(destination, out int yWritten, format, provider)) return false;

        destination = destination[yWritten..];
        if (destination.Length < 1) return false;

        destination[0] = ')';
        charsWritten = xWritten + yWritten + 4;
        return true;
    }

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
    public TResult GetLength<TResult>() where TResult : unmanaged, IBinaryFloatingPointIeee754<TResult>, IMinMaxValue<TResult>
    {
        Vector2<TResult> resultVector = Vector2<TResult>.CreateChecked(this);
        return TResult.Sqrt((resultVector.X * resultVector.X) + (resultVector.Y * resultVector.Y));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<Vector2<T>>.Equals(Vector2<T> other) => Equals(other);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int IComparable<Vector2<T>>.CompareTo(Vector2<T> other) => CompareTo(other);

    /// <summary>
    /// Converts the vector to the target type
    /// </summary>
    /// <typeparam name="TSource">Source number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> CreateChecked<TSource>(Vector2<TSource> value)
        where TSource : unmanaged, IBinaryNumber<TSource>, IMinMaxValue<TSource>
    {
        return new Vector2<T>(T.CreateChecked(value.X), T.CreateChecked(value.Y));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Distance(Vector2<T> a, Vector2<T> b) => (a - b).Length;

    /// <inheritdoc />
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

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Cross(Vector2<T> a, Vector2<T> b) => (a.X * b.Y) - (a.Y * b.X);

    /// <inheritdoc />
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

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> MinMagnitude(Vector2<T> a, Vector2<T> b) => a.Length < b.Length ? a : b;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> MaxMagnitude(Vector2<T> a, Vector2<T> b) => a.Length > b.Length ? a : b;

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
        if (!T.TryParse(groups[2].ValueSpan, NumberStyles.Number, null, out T distance)) return false;
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
    /// <param name="style">Number style</param>
    /// <param name="provider">Format provider</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two values present after the split</exception>
    public static Vector2<T> Parse(string value, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value), "Value cannot be null or empty");
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        return Parse(value.AsSpan(), separator, style, provider);
    }

    /// <summary>
    /// Parses the two component vector using the given value and number separator
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <param name="style">Number style</param>
    /// <param name="provider">Format provider</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is empty</exception>
    /// <exception cref="ArgumentNullException">If <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two values present after the split</exception>
    public static Vector2<T> Parse(ReadOnlySpan<char> value, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Value cannot be empty", nameof(value));
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        Span<Range> ranges = stackalloc Range[2];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is not 2) throw new FormatException("String to parse not properly formatted");

        T x = T.Parse(value[ranges[0]], style, provider);
        T y = T.Parse(value[ranges[1]], style, provider);
        return new Vector2<T>(x, y);
    }

    /// <summary>
    /// Tries to parse the two component vector using the given value and returns the success
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="result">Resulting vector, if any</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <param name="style">Number style</param>
    /// <param name="provider">Format provider</param>
    /// <returns><see langword="true"/> if the parse succeeded, otherwise <see langword="false"/></returns>
    public static bool TryParse(string? value, out Vector2<T> result, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            result = Zero;
            return false;
        }

        return TryParse(value.AsSpan(), out result, separator, style, provider);
    }

    /// <summary>
    /// Tries to parse the two component vector using the given value and returns the success
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="result">Resulting vector, if any</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <param name="style">Number style</param>
    /// <param name="provider">Format provider</param>
    /// <returns><see langword="true"/> if the parse succeeded, otherwise <see langword="false"/></returns>
    public static bool TryParse(ReadOnlySpan<char> value, out Vector2<T> result, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace() || string.IsNullOrEmpty(separator))
        {
            result = Zero;
            return false;
        }

        Span<Range> ranges = stackalloc Range[2];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is not 2
         || !T.TryParse(value[ranges[0]], style, provider, out T x)
         || !T.TryParse(value[ranges[1]], style, provider, out T y))
        {
            result = Zero;
            return false;
        }

        result = new Vector2<T>(x, y);
        return true;
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
    /// Increment operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component incremented by one</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator ++(Vector2<T> a) => new(a.X + T.One, a.Y + T.One);

    /// <summary>
    /// Decrement operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component decremented by one</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator --(Vector2<T> a) => new(a.X - T.One, a.Y - T.One);

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

    /// <inheritdoc />
    static int INumberBase<Vector2<T>>.Radix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.Radix;
    }

    /// <inheritdoc />
    static Vector2<T> IAdditiveIdentity<Vector2<T>, Vector2<T>>.AdditiveIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Zero;
    }

    /// <inheritdoc />
    static Vector2<T> IMultiplicativeIdentity<Vector2<T>, Vector2<T>>.MultiplicativeIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => One;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int IVector.GetDimension() => Dimension;

    /// <inheritdoc />
    bool IVector.TryGetComponentChecked<TResult>(int index, out TResult result)
    {
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromChecked(this[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    bool IVector.TryGetComponentSaturating<TResult>(int index, out TResult result)
    {
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromSaturating(this[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    bool IVector.TryGetComponentTruncating<TResult>(int index, out TResult result)
    {
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromTruncating(this[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    bool IVector.TryMakeFromComponentsChecked<TComponent>(ReadOnlySpan<TComponent> components, out IVector? result)
    {
        if (components.Length > Dimension)
        {
            result = null;
            return false;
        }

        switch (components.Length)
        {
            case 1:
                if (T.TryConvertFromChecked(components[0], out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = null;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    bool IVector.TryMakeFromComponentsSaturating<TComponent>(ReadOnlySpan<TComponent> components, out IVector? result)
    {
        if (components.Length > Dimension)
        {
            result = null;
            return false;
        }

        switch (components.Length)
        {
            case 1:
                if (T.TryConvertFromSaturating(components[0], out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = null;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    bool IVector.TryMakeFromComponentsTruncating<TComponent>(ReadOnlySpan<TComponent> components, out IVector? result)
    {
        if (components.Length > Dimension)
        {
            result = null;
            return false;
        }

        switch (components.Length)
        {
            case 1:
                if (T.TryConvertFromTruncating(components[0], out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = null;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsCanonical(Vector2<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsComplexNumber(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsEvenInteger(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsFinite(Vector2<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsImaginaryNumber(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsInfinity(Vector2<T> value) => T.IsInfinity(value.X) || T.IsInfinity(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsInteger(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsNaN(Vector2<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsNegative(Vector2<T> value) => T.IsNegative(value.X) || T.IsNegative(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsNegativeInfinity(Vector2<T> value) => T.IsNegativeInfinity(value.X) || T.IsNegativeInfinity(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsNormal(Vector2<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsOddInteger(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsPositive(Vector2<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsPositiveInfinity(Vector2<T> value) => T.IsPositiveInfinity(value.X) || T.IsPositiveInfinity(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsRealNumber(Vector2<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsSubnormal(Vector2<T> value) => T.IsSubnormal(value.X) || T.IsSubnormal(value.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.IsZero(Vector2<T> value) => value == Zero;

    /// <inheritdoc />
    static Vector2<T> INumberBase<Vector2<T>>.MaxMagnitudeNumber(Vector2<T> a, Vector2<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y)) return a;
        return MaxMagnitude(a, b);
    }

    /// <inheritdoc />
    static Vector2<T> INumberBase<Vector2<T>>.MinMagnitudeNumber(Vector2<T> a, Vector2<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y)) return a;
        return MinMagnitude(a, b);
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertFromChecked<TOther>(TOther value, out Vector2<T> result)
    {
        if (value is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension > Dimension)
        {
            result = default;
            return false;
        }

        switch (dimension)
        {
            case 1:
                if (vector.TryGetComponentChecked(0, out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertFromSaturating<TOther>(TOther value, out Vector2<T> result)
    {
        if (value is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension > Dimension)
        {
            result = default;
            return false;
        }

        switch (dimension)
        {
            case 1:
                if (vector.TryGetComponentSaturating(0, out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentSaturating(0, out x) && vector.TryGetComponentSaturating(0, out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertFromTruncating<TOther>(TOther value, out Vector2<T> result)
    {
        if (value is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension > Dimension)
        {
            result = default;
            return false;
        }

        switch (dimension)
        {
            case 1:
                if (vector.TryGetComponentTruncating(0, out T x))
                {
                    result = new Vector2<T>(x, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentTruncating(0, out x) && vector.TryGetComponentTruncating(0, out T y))
                {
                    result = new Vector2<T>(x, y);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertToChecked<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result)
    {
        result = default;
        if (result is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension < Dimension)
        {
            result = default;
            return false;
        }

        if (vector.TryMakeFromComponentsChecked([value.X, value.Y], out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertToSaturating<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result)
    {
        result = default;
        if (result is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension < Dimension)
        {
            result = default;
            return false;
        }

        if (vector.TryMakeFromComponentsSaturating([value.X, value.Y], out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    static bool INumberBase<Vector2<T>>.TryConvertToTruncating<TOther>(Vector2<T> value, [MaybeNullWhen(false)] out TOther result)
    {
        result = default;
        if (result is not IVector vector)
        {
            result = default;
            return false;
        }

        int dimension = vector.GetDimension();
        if (dimension < Dimension)
        {
            result = default;
            return false;
        }

        if (vector.TryMakeFromComponentsTruncating([value.X, value.Y], out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> INumberBase<Vector2<T>>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> INumberBase<Vector2<T>>.Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> IParsable<Vector2<T>>.Parse(string s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> ISpanParsable<Vector2<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector2<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector2<T>>.TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Vector2<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<Vector2<T>>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Vector2<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<Vector2<T>>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector2<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> IMultiplyOperators<Vector2<T>, Vector2<T>, Vector2<T>>.operator *(Vector2<T> left, Vector2<T> right) => new(left.X * right.X, left.Y * right.Y);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector2<T> IDivisionOperators<Vector2<T>, Vector2<T>, Vector2<T>>.operator /(Vector2<T> left, Vector2<T> right) => new(left.X / right.X, left.Y / right.Y);
}
