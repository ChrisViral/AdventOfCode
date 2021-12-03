using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

namespace AdventOfCode.Grids.Vectors;

/// <summary>
/// Integer two component vector
/// </summary>
public readonly struct Vector2<T> : IComparable, IComparable<Vector2<T>>, IEquatable<Vector2<T>>, IFormattable where T : IBinaryNumber<T>
{
    #region Constants
    private static readonly Regex directionMatch = new(@"^\s*(U|N|D|S|L|W|R|E)(\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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
    private static readonly bool IsInteger = typeof(T).IsAssignableTo(typeof(IBinaryInteger<>));
    #endregion

    #region Propeties
    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T X { get; }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public T Y { get; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    public double Length { get; }
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
        this.Length = GetLength<double>(x, y);
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
        this.Length = copy.Length;
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? other) => other is Vector2<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(in Vector2<T> other) => this.X == other.X && this.Y == other.Y;

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    /// ReSharper disable once TailRecursiveCall - not tail recursive
    public int CompareTo(object? other) => other is Vector2<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
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
    /// <param name="directions">Direction to move in</param>
    /// <returns>The new, moved vector</returns>
    public Vector2<T> Move(Directions directions) => this + directions;

    /// <summary>
    /// Gets all the adjacent Vector2 to this one
    /// </summary>
    /// <returns>Adjacent vectors</returns>
    public IEnumerable<Vector2<T>> Adjacent(bool includeDiagonals = false)
    {
        if (!IsInteger)
        {
            yield break;
        }

        if (includeDiagonals)
        {
            for (T x = this.X - T.One; x <= this.X + T.One; x++)
            {
                for (T y = this.Y - T.One; y <= this.Y + T.One; y++)
                {
                    if (x == this.X && y == this.Y) continue;

                    yield return new(x, y);
                }
            }
        }
        else
        {
            yield return this + Up;
            yield return this + Left;
            yield return this + Right;
            yield return this + Down;
        }
    }

    /// <summary>
    /// Creates an irreducible version of this vector
    /// </summary>
    /// <returns>The fully reduced version of this vector</returns>
    public Vector2<T> Reduced()
    {
        if (!IsInteger)
        {
            return this;
        }

        T a = T.Abs(this.X);
        T b = T.Abs(this.Y);
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

        T gcd = a | b;
        return this / gcd;
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    bool IEquatable<Vector2<T>>.Equals(Vector2<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    int IComparable<Vector2<T>>.CompareTo(Vector2<T> other) => CompareTo(other);
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
        if (IsInteger)
        {
            return Rotate(vector, (double)angle);
        }

        if (angle % 90 is not 0) throw new InvalidOperationException($"Can only rotate integer vectors by 90 degrees, got {angle} instead");
        angle = ((angle % 360) + 360) % 360;
        return angle switch
        {
            90  => new(-vector.Y, vector.X),
            180 => -vector,
            270 => new(vector.Y, -vector.X),
            _   => vector
        };
    }

    /// <summary>
    /// Rotates a vector by a specified angle, must be a multiple of 90 degrees
    /// </summary>
    /// <param name="vector">Vector to rotate</param>
    /// <param name="angle">Angle to rotate by</param>
    /// <returns>The rotated vector</returns>
    /// <exception cref="InvalidOperationException">If the angle is not a multiple of 90 degrees</exception>
    public static Vector2<T> Rotate(in Vector2<T> vector, double angle)
    {
        if (IsInteger)
        {
            return Rotate(vector, (int)Math.Round(angle));
        }



        if (angle % 90 is not 0) throw new InvalidOperationException($"Can only rotate integer vectors by 90 degrees, got {angle} instead");
        angle = ((angle % 360) + 360) % 360;
        return angle switch
        {
            90  => new(-vector.Y, vector.X),
            180 => -vector,
            270 => new(vector.Y, -vector.X),
            _   => vector
        };
    }

    /// <summary>
    /// Parses the Vector2 from a direction and distance
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>The parsed Vector2</returns>
    /// <exception cref="FormatException">If the direction format or string format is invalid</exception>
    /// <exception cref="OverflowException">If the number parse causes an overflow</exception>
    public static Vector2<T> ParseFromDirection(string value)
    {
        GroupCollection groups = directionMatch.Match(value).Groups;
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
        return direction * T.Parse(groups[2].Value, NumberStyles.Number, null);
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
        Match match = directionMatch.Match(value);
        if (!match.Success) return false;

        GroupCollection groups = match.Groups;
        if (groups.Count is not 3) return false;
        if (!T.TryParse(groups[2].Value, NumberStyles.Number, null, out T distance)) return false;
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
    /// <returns>An enumerable of all the vectors in the given range</returns>
    public static IEnumerable<Vector2<T>> Enumerate(T maxX, T maxY)
    {
        if (!IsInteger) yield break;

        for (T y = T.Zero; y < maxY; y++)
        {
            for (T x = T.Zero; x < maxX; x++)
            {
                yield return new(x, y);
            }
        }
    }

    private static TResult GetLength<TResult>(T x, T y) where TResult : IBinaryFloatingPoint<TResult>
    {
        return TResult.Sqrt(TResult.Create((x * x) + (y * y)));
    }

    private Vector2<TResult> Convert<TResult>() where TResult : IBinaryFloatingPoint<TResult>
    {
        return new(TResult.Create(this.X), TResult.Create(this.Y));
    }
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
    public static bool operator ==(in Vector2<T> a, in Vector2<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    public static bool operator !=(in Vector2<T> a, in Vector2<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    public static bool operator <(in Vector2<T> a, in Vector2<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    public static bool operator >(in Vector2<T> a, in Vector2<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    public static bool operator <=(in Vector2<T> a, in Vector2<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    public static bool operator >=(in Vector2<T> a, in Vector2<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    public static Vector2<T> operator -(in Vector2<T> a) => new(-a.X, -a.Y);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    public static Vector2<T> operator +(in Vector2<T> a, in Vector2<T> b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    public static Vector2<T> operator -(in Vector2<T> a, in Vector2<T> b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Add operation between a vector and a direction
    /// </summary>
    /// <param name="a">Vector</param>
    /// <param name="b">Direction</param>
    /// <returns>The result of the movement of the vector in the given direction</returns>
    public static Vector2<T> operator +(in Vector2<T> a, Directions b) => a + b.ToVector<T>();

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2<T> operator *(in Vector2<T> a, T b) => new(a.X * b, a.Y * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2<T> operator /(in Vector2<T> a, T b) => new(a.X / b, a.Y / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector2<T> operator %(in Vector2<T> a, T b) => new(a.X % b, a.Y % b);
    #endregion
}