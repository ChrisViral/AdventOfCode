using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Extensions.Types;
using JetBrains.Annotations;
using NumericalType = AdventOfCode.Vectors.WrongNumericalTypeException.NumericalType;

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
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    public struct VectorSpaceEnumerator(T maxX, T maxY) : IEnumerable<Vector2<T>>, IEnumerator<Vector2<T>>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;

        private T x = maxX - T.One;
        private T y = -T.One;

        /// <inheritdoc />
        object IEnumerator.Current => this.Current;

        /// <inheritdoc />
        public Vector2<T> Current => new(this.x, this.y);

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (++this.x == this.maxX)
            {
                this.x = T.Zero;
                this.y++;
            }

            return this.y < this.maxY;
        }

        /// <inheritdoc />
        public void Dispose() { }

        /// <inheritdoc />
        public void Reset() => throw new NotSupportedException();

        /// <inheritdoc />
        public IEnumerator<Vector2<T>> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this;
    }

    #region Constants
    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = T.CreateChecked(1E-5);
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
    [GeneratedRegex(@"^\s*(U|N|D|S|L|W|R|E)\s*(\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex DirectionMatch { get; }
    /// <summary>
    /// Minimum vector value
    /// </summary>
    public static Vector2<T> MinValue { get; } = new(T.MinValue, T.MinValue);
    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector2<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue);
    #endregion

    #region Propeties
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
    public double Length => GetLength<double>();

    /// <summary>
    /// Creates an irreducible version of this vector<br/>
    /// NOTE: If this is an floating point vector, an exception will be thrown, use <see cref="Normalized"/> instead
    /// </summary>
    /// <returns>The fully reduced version of this vector</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    public Vector2<T> Reduced => IsInteger ? this / GCD(this.X, this.Y) : throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));

    /// <summary>
    /// Creates a normalized version of this vector<br/>
    /// NOTE: If this is an integer vector, an exception will be thrown, use <see cref="Reduced"/> instead
    /// </summary>
    /// <returns>The vector normalized</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not a floating type</exception>
    public Vector2<T> Normalized => !IsInteger ? this / T.CreateChecked(this.Length) : throw new WrongNumericalTypeException(NumericalType.FLOATING, typeof(T));
    #endregion

    #region Constructors
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
    public Vector2(in Vector2<T> copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? other) => other is Vector2<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    public bool Equals(in Vector2<T> other) => IsInteger ? this.X == other.X && this.Y == other.Y
                                                         : Approximately(this.X, other.X) && Approximately(this.Y, other.Y);

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    public int CompareTo(object? other) => other is Vector2<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    public int CompareTo(in Vector2<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"({this.X}, {this.Y})";

    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
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
    /// Creates a new vector resulting in the moving of this vector in the specified direction
    /// </summary>
    /// <param name="direction">Direction to move in</param>
    /// <returns>The new, moved vector</returns>
    public Vector2<T> Move(Direction direction) => this + direction;

    /// <summary>
    /// Gets all the adjacent Vector2 to this one
    /// </summary>
    /// <returns>Adjacent vectors</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    public IEnumerable<Vector2<T>> Adjacent(bool includeDiagonals = false, bool includeSelf = false)
    {
        if (!IsInteger) throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));

        if (includeDiagonals)
        {
            for (T y = this.Y - T.One; y <= this.Y + T.One; y++)
            {
                for (T x = this.X - T.One; x <= this.X + T.One; x++)
                {
                    if (!includeSelf && x == this.X && y == this.Y) continue;

                    yield return new(x, y);
                }
            }
        }
        else
        {
            if (includeSelf) yield return this;
            yield return this + Up;
            yield return this + Left;
            yield return this + Right;
            yield return this + Down;
        }
    }

    /// <summary>
    /// Scales the components of the vector by the specified factors
    /// </summary>
    /// <param name="scaleX">X component scale</param>
    /// <param name="scaleY">Y component scale</param>
    /// <returns>The scaled vector</returns>
    public Vector2<T> Scale(T scaleX, T scaleY) => new(this.X * scaleX, this.Y * scaleY);

    /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
    /// </summary>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <see cref="X"/> or <see cref="Y"/> are smaller or equal to zero</exception>
    public VectorSpaceEnumerator EnumerateOver()
    {
        if (this.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.X), this.X, "X boundary value must be greater than zero");
        if (this.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.Y), this.Y, "Y boundary value must be greater than zero");

        return new(this.X, this.Y);
    }

    /// <summary>
    /// Gets the length of this vector in the specified floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point type to get the length in</typeparam>
    /// <returns>The length of the vector</returns>
    public TResult GetLength<TResult>() where TResult : IBinaryFloatingPointIeee754<TResult>
    {
        return TResult.Sqrt(TResult.CreateChecked((this.X * this.X) + (this.Y * this.Y)));
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    bool IEquatable<Vector2<T>>.Equals(Vector2<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    int IComparable<Vector2<T>>.CompareTo(Vector2<T> other) => CompareTo(other);

    /// <summary>
    /// Converts the vector to the target type
    /// </summary>
    /// <typeparam name="TResult">Number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    public Vector2<TResult> Convert<TResult>() where TResult : IBinaryNumber<TResult>, IMinMaxValue<TResult>
    {
        return new(TResult.CreateChecked(this.X), TResult.CreateChecked(this.Y));
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Calculates the distance between two vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The distance between both vectors</returns>
    public static double Distance(in Vector2<T> a, in Vector2<T> b) => (a - b).Length;

    /// <summary>
    /// The Manhattan distance between both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Tge straight line distance between both vectors</returns>
    public static T ManhattanDistance(in Vector2<T> a, in Vector2<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y);

    /// <summary>
    /// Calculates the signed angle, in degrees, between two vectors. The result is in the range [-180, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle Angle(in Vector2<T> a, in Vector2<T> b)
    {
        (double aX, double aY) = a.Convert<double>();
        (double bX, double bY) = b.Convert<double>();
        return Vectors.Angle.FromRadians(Math.Atan2(aX * bY - aY * bX, aX * bX + aY * bY));
    }

    /// <summary>
    /// Calculates the absolute angle, in degrees, between two vectors. The result is in the range [0, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle AbsoluteAngle(in Vector2<T> a, in Vector2<T> b)
    {
        double dot = Vector2<double>.Dot(a.Convert<double>(), b.Convert<double>());
        return Vectors.Angle.FromRadians(Math.Acos(dot / (a.Length * b.Length)));
    }

    /// <summary>
    /// Calculates the dot product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The dot product of both vectors</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public static T Dot(in Vector2<T> a, in Vector2<T> b) => a.X * b.X + a.Y * b.Y;

    /// <summary>
    /// Rotates a vector by a specified angle, must be a multiple of 90 degrees
    /// </summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Angle to rotate by</param>
    /// <returns>The rotated vector</returns>
    /// <exception cref="InvalidOperationException">If the angle is not a multiple of 90 degrees</exception>
    public static Vector2<T> Rotate(in Vector2<T> vector, int angle)
    {
        if (!IsInteger) return Rotate(vector, (double)angle);

        if (!angle.IsMultiple(90)) throw new InvalidOperationException($"Can only rotate integer vectors by 90 degrees, got {angle} instead");

        angle = angle.Mod(360);
        return angle switch
        {
            90  => new(-vector.Y, vector.X),
            180 => -vector,
            270 => new(vector.Y, -vector.X),
            _   => vector
        };
    }

    /// <summary>
    /// Rotates a vector by a specified angle
    /// </summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Angle to rotate by</param>
    /// <returns>The rotated vector</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not a floating type</exception>
    public static Vector2<T> Rotate(in Vector2<T> vector, double angle)
    {
        if (IsInteger) throw new WrongNumericalTypeException(NumericalType.FLOATING, typeof(T));

        (double x, double y) = vector.Convert<double>();
        double radians = angle * Vectors.Angle.DEG2RAD;
        Vector2<double> result = new(x * Math.Cos(radians) - y * Math.Sin(radians),
                                     x * Math.Sin(radians) + y * Math.Cos(radians));
        return result.Convert<T>();
    }

    /// <summary>
    /// Rotates a vector by a specified angle
    /// </summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Angle to rotate by</param>
    /// <returns>The rotated vector</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not a floating type</exception>
    public static Vector2<T> Rotate(in Vector2<T> vector, Angle angle) => Rotate(vector, angle.Radians);

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
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
    /// </summary>
    /// <param name="maxX">Max value for the x component, exclusive</param>
    /// <param name="maxY">Max value for the y component, exclusive</param>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/> or <paramref name="maxY"/> are smaller or equal to zero</exception>
    public static VectorSpaceEnumerator Enumerate(T maxX, T maxY)
    {
        if (!IsInteger) throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));
        if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
        if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");

        return new(maxX, maxY);
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
        return new(x, y);
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

        return TryParse(value.AsSpan(),  out result, separator);
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

        result = new(x, y);
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
    private static bool Approximately(T a, T b) => T.Abs(a - b) <= Epsilon;
    #endregion

    #region Operators
    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="Vector2"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    public static implicit operator Vector2<T>((T x, T y) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector2"/> to <see cref="ValueTuple{T1, T2}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    public static implicit operator (T x, T y)(in Vector2<T> vector) => (vector.X, vector.Y);

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
    public static bool operator !=(Vector2<T> a, Vector2<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    public static bool operator <(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    public static bool operator >(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    public static bool operator <=(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    public static bool operator >=(Vector2<T> a, Vector2<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    public static Vector2<T> operator -(Vector2<T> a) => new(-a.X, -a.Y);

    /// <summary>
    /// Plus operation on a vector
    /// </summary>
    /// <param name="a">Vector to apply the plus to</param>
    /// <returns>The vector where all the components had the plus operator applied to</returns>
    public static Vector2<T> operator +(Vector2<T> a) => new(+a.X, +a.Y);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    public static Vector2<T> operator +(Vector2<T> a, Vector2<T> b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    public static Vector2<T> operator -(Vector2<T> a, Vector2<T> b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Add operation between a vector and a direction
    /// </summary>
    /// <param name="a">Vector</param>
    /// <param name="b">Direction</param>
    /// <returns>The result of the movement of the vector in the given direction</returns>
    public static Vector2<T> operator +(Vector2<T> a, Direction b) => a + b.ToVector<T>();

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2<T> operator *(Vector2<T> a, T b) => new(a.X * b, a.Y * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2<T> operator /(Vector2<T> a, T b) => new(a.X / b, a.Y / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector2<T> operator %(Vector2<T> a, T b) => new(a.X % b, a.Y % b);

    /// <summary>
    /// Per component modulo operator
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Vector containing which values to modulo the components by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector2<T> operator %(Vector2<T> a, Vector2<T> b) => new(a.X % b.X, a.Y % b.Y);
    #endregion
}