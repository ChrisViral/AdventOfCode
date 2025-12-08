using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.Extensions.Types;
using JetBrains.Annotations;
using static AdventOfCode.Vectors.WrongNumericalTypeException;

namespace AdventOfCode.Vectors;

/// <summary>
/// Integer three component vector
/// </summary>
[PublicAPI]
public readonly partial struct Vector3<T> : IAdditionOperators<Vector3<T>, Vector3<T>, Vector3<T>>, ISubtractionOperators<Vector3<T>, Vector3<T>, Vector3<T>>,
                                            IUnaryNegationOperators<Vector3<T>, Vector3<T>>, IUnaryPlusOperators<Vector3<T>, Vector3<T>>,
                                            IComparisonOperators<Vector3<T>, Vector3<T>, bool>, IMinMaxValue<Vector3<T>>, IFormattable,
                                            IDivisionOperators<Vector3<T>, T, Vector3<T>>, IMultiplyOperators<Vector3<T>, T, Vector3<T>>,
                                            IModulusOperators<Vector3<T>, T, Vector3<T>>, IModulusOperators<Vector3<T>, Vector3<T>, Vector3<T>>,
                                            IComparable<Vector3<T>>, IEquatable<Vector3<T>>
    where T : IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = T.CreateChecked(1E-5);
    /// <summary>Zero vector</summary>
    public static readonly Vector3<T> Zero      = new(T.Zero, T.Zero, T.Zero);
    /// <summary>One vector</summary>
    public static readonly Vector3<T> One       = new(T.One,  T.One,  T.One);
    /// <summary>Up vector</summary>
    public static readonly Vector3<T> Up        = new(T.Zero, T.One,  T.Zero);
    /// <summary>Down vector</summary>
    public static readonly Vector3<T> Down      = new(T.Zero, -T.One, T.Zero);
    /// <summary>Left vector</summary>
    public static readonly Vector3<T> Left      = new(-T.One, T.Zero, T.Zero);
    /// <summary>Right vector</summary>
    public static readonly Vector3<T> Right     = new(T.One,  T.Zero, T.Zero);
    /// <summary>Forward vector</summary>
    public static readonly Vector3<T> Forwards  = new(T.Zero, T.Zero, T.One);
    /// <summary>Backward vector</summary>
    public static readonly Vector3<T> Backwards = new(T.Zero, T.Zero, -T.One);
    /// <summary>
    /// Minimum vector value
    /// </summary>
    public static Vector3<T> MinValue  { get; } = new(T.MinValue, T.MinValue, T.MinValue);
    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector3<T> MaxValue  { get; } = new(T.MaxValue, T.MaxValue, T.MaxValue);

    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T X { get; init; }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public T Y { get; init; }

    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T Z { get; init; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    /// ReSharper disable once MemberCanBePrivate.Global
    public double Length => GetLength<double>();

    /// <summary>
    /// Absolute length of all three vector components summed
    /// </summary>
    public T ManhattanLength => T.Abs(this.X) + T.Abs(this.Y)+ T.Abs(this.Z);

    /// <summary>
    /// Creates an irreducible version of this vector<br/>
    /// NOTE: If this is an floating point vector, an exception will be thrown, use <see cref="Normalized"/> instead
    /// </summary>
    /// <returns>The fully reduced version of this vector</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    public Vector3<T> Reduced => IsInteger ? this / GCD(GCD(this.X, this.Y), this.Z) : throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));

    /// <summary>
    /// Creates a normalized version of this vector<br/>
    /// NOTE: If this is an integer vector, an exception will be thrown, use <see cref="Reduced"/> instead
    /// </summary>
    /// <returns>The vector normalized</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not a floating type</exception>
    public Vector3<T> Normalized => !IsInteger ? this / T.CreateChecked(this.Length) : throw new WrongNumericalTypeException(NumericalType.FLOATING, typeof(T));

    /// <summary>
    /// Creates a new <see cref="Vector3{T}"/> with the specified components
    /// </summary>
    /// <param name="x">X component</param>
    /// <param name="y">Y component</param>
    /// <param name="z">Z component</param>
    public Vector3(T x, T y, T z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    /// <summary>
    /// Creates a new <see cref="Vector3{T}"/> from a given three component tuple
    /// </summary>
    /// <param name="tuple">Tuple to create the Vector from</param>
    /// ReSharper disable once UseDeconstructionOnParameter
    /// ReSharper disable once MemberCanBePrivate.Global
    public Vector3((T x, T y, T z) tuple) : this(tuple.x, tuple.y, tuple.z) { }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector3(in Vector3<T> copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
        this.Z = copy.Z;
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? other) => other is Vector3<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    public bool Equals(in Vector3<T> other) => IsInteger ? this.X == other.X && this.Y == other.Y && this.Z == other.Z
                                                         : Approximately(this.X, other.X) && Approximately(this.Y, other.Y) && Approximately(this.Z, other.Z);

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Z);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    /// ReSharper disable once TailRecursiveCall - not tail recursive
    public int CompareTo(object? other) => other is Vector3<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    public int CompareTo(in Vector3<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"({this.X}, {this.Y}, {this.Z})";

    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
    public string ToString(string? format, IFormatProvider? provider)
    {
        return $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)}, {this.Z.ToString(format, provider)})";
    }

    /// <summary>
    /// Deconstructs this vector into a tuple
    /// </summary>
    /// <param name="x">X parameter</param>
    /// <param name="y">Y parameter</param>
    /// <param name="z">Z parameter</param>
    public void Deconstruct(out T x, out T y, out T z)
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
    }

    /// <summary>
    /// Lists out the 27 vectors adjacent to this one
    /// </summary>
    /// <returns>An enumerable of the adjacent vectors</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    /// ReSharper disable once CognitiveComplexity
    public IEnumerable<Vector3<T>> Adjacent(bool includeDiagonals = true)
    {
        if (!IsInteger) throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));

        if (includeDiagonals)
        {
            for (T x = -T.One; x <= T.One; x++)
            {
                for (T y = -T.One; y <= T.One; y++)
                {
                    for (T z = -T.One; z <= T.One; z++)
                    {
                        Vector3<T> v = new(x, y, z);
                        if (v == Zero) continue;

                        yield return this + v;
                    }
                }
            }
        }
        else
        {
            yield return this + Left;
            yield return this + Right;
            yield return this + Up;
            yield return this + Down;
            yield return this + Backwards;
            yield return this + Forwards;
        }
    }

    /// <summary>
    /// Scales the components of the vector by the specified factors
    /// </summary>
    /// <param name="scaleX">X component scale</param>
    /// <param name="scaleY">Y component scale</param>
    /// <param name="scaleZ">Z component scale</param>
    /// <returns>The scaled vector</returns>
    public Vector3<T> Scale(T scaleX, T scaleY, T scaleZ) => new(this.X * scaleX, this.Y * scaleY, this.Z * scaleZ);

    /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
    /// </summary>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <see cref="X"/>, <see cref="Y"/>, or <see cref="Z"/> are smaller or equal to zero</exception>
    public SpaceEnumerator Enumerate()
    {
        if (this.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.X), this.X, "X boundary value must be greater than zero");
        if (this.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.Y), this.Y, "Y boundary value must be greater than zero");
        if (this.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.Z), this.Z, "Z boundary value must be greater than zero");

        return new SpaceEnumerator(this.X, this.Y, this.Y);
    }

    /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
    /// </summary>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <see cref="X"/>, <see cref="Y"/>, or <see cref="Z"/> are smaller or equal to zero</exception>
    public SpaceEnumerable AsEnumerable()
    {
        if (this.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.X), this.X, "X boundary value must be greater than zero");
        if (this.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.Y), this.Y, "Y boundary value must be greater than zero");
        if (this.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(this.Z), this.Z, "Z boundary value must be greater than zero");

        return new SpaceEnumerable(this.X, this.Y, this.Z);
    }

    /// <summary>
    /// Gets the length of this vector in the target floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point result type</typeparam>
    /// <returns>The length of the vector, in the specified floating point type</returns>
    private TResult GetLength<TResult>() where TResult : IBinaryFloatingPointIeee754<TResult>
    {
        if (!IsInteger)
        {
            return TResult.Sqrt(TResult.CreateChecked((this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z)));
        }

        long longX = long.CreateChecked(this.X);
        long longY = long.CreateChecked(this.Y);
        long longZ = long.CreateChecked(this.Z);
        return TResult.Sqrt(TResult.CreateChecked((longX * longX) + (longY * longY) + (longZ * longZ)));
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    bool IEquatable<Vector3<T>>.Equals(Vector3<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    int IComparable<Vector3<T>>.CompareTo(Vector3<T> other) => CompareTo(other);

    /// <summary>
    /// Calculates the distance between two vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The distance between both vectors</returns>
    public static double Distance(in Vector3<T> a, in Vector3<T> b) => (a - b).Length;

    /// <summary>
    /// The Manhattan distance between both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>Tge straight line distance between both vectors</returns>
    public static T ManhattanDistance(in Vector3<T> a, in Vector3<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y) + T.Abs(a.Z - b.Z);

    /// <summary>
    /// Calculates the dot product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The dot product of both vectors</returns>
    public static T Dot(in Vector3<T> a, in Vector3<T> b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>
    /// Gives the absolute value of a given vector
    /// </summary>
    /// <param name="vector">Vector to get the absolute value of</param>
    /// <returns>Absolute value of the vector</returns>
    public static Vector3<T> Abs(in Vector3<T> vector) => new(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z));

    /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
    /// </summary>
    /// <param name="maxX">Max value for the x component, exclusive</param>
    /// <param name="maxY">Max value for the y component, exclusive</param>
    /// <param name="maxZ">Max value for the z component, exclusive</param>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
    public static SpaceEnumerator EnumerateOver(T maxX, T maxY, T maxZ)
    {
        if (!IsInteger) throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));
        if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
        if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
        if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");

        return new SpaceEnumerator(maxX, maxY, maxZ);
    }

    /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
    /// </summary>
    /// <param name="maxX">Max value for the x component, exclusive</param>
    /// <param name="maxY">Max value for the y component, exclusive</param>
    /// <param name="maxZ">Max value for the z component, exclusive</param>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="WrongNumericalTypeException">If <typeparamref name="T"/> is not an integer type</exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
    public static SpaceEnumerable MakeEnumerable(T maxX, T maxY, T maxZ)
    {
        if (!IsInteger) throw new WrongNumericalTypeException(NumericalType.INTEGER, typeof(T));
        if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
        if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
        if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");

        return new SpaceEnumerable(maxX, maxY, maxZ);
    }

    /// <summary>
    /// Gets the length of this vector in the target floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point result type</typeparam>
    /// <param name="x">X Parameter</param>
    /// <param name="y">Y parameter</param>
    /// <param name="z">Z parameter</param>
    /// <returns>The length of the vector, in the specified floating point type</returns>
    private static TResult GetLength<TResult>(T x, T y, T z) where TResult : IBinaryFloatingPointIeee754<TResult>
    {
        return TResult.Sqrt(TResult.CreateChecked((x * x) + (y * y) + (z * z)));
    }

    /// <summary>
    /// Parses the two component vector using the given value and number separator
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two or three values present after the split</exception>
    public static Vector3<T> Parse(string value, string separator = ",")
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
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two or three values present after the split</exception>
    public static Vector3<T> Parse(ReadOnlySpan<char> value, string separator = ",")
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Value cannot be null or empty", nameof(value));
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        Span<Range> ranges = stackalloc Range[3];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 3) throw new FormatException("String to parse not properly formatted");

        T x = T.Parse(value[ranges[0]], null);
        T y = T.Parse(value[ranges[1]], null);
        return new Vector3<T>(x, y, written is 3 ? T.Parse(value[ranges[2]], null) : T.Zero);
    }

    /// <summary>
    /// Tries to parse the two component vector using the given value and returns the success
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="result">Resulting vector, if any</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <returns><see langword="true"/> if the parse succeeded, otherwise <see langword="false"/></returns>
    public static bool TryParse(string? value, out Vector3<T> result, string separator = ",")
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(separator))
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
    public static bool TryParse(ReadOnlySpan<char> value, out Vector3<T> result, string separator = ",")
    {
        if (value.IsEmpty || value.IsWhiteSpace() || string.IsNullOrEmpty(separator))
        {
            result = Zero;
            return false;
        }

        Span<Range> ranges = stackalloc Range[3];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 3 || !T.TryParse(value[ranges[0]], null, out T? x) || !T.TryParse(value[ranges[1]], null, out T? y))
        {
            result = Zero;
            return false;
        }

        result = new Vector3<T>(x, y, written is 3 && T.TryParse(value[ranges[2]], null, out T? z) ? z : T.Zero);
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

    /// <summary>
    /// Converts the vector to the target type
    /// </summary>
    /// <typeparam name="TResult">Number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    public Vector3<TResult> Convert<TResult>() where TResult : IBinaryNumber<TResult>, IMinMaxValue<TResult>
    {
        return new Vector3<TResult>(TResult.CreateChecked(this.X), TResult.CreateChecked(this.Y), TResult.CreateChecked(this.Z));
    }

    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2, T3}"/> to <see cref="Vector3{T}"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    public static implicit operator Vector3<T>((T x, T y, T z) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector3{T}"/> to <see cref="ValueTuple{T1, T2, T3}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    public static implicit operator (T x, T y, T z)(Vector3<T> vector) => (vector.X, vector.Y, vector.Z);

    /// <summary>
    /// Casts from <see cref="Vector3{T}"/> to <see cref="Vector3{T}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    public static implicit operator Vector3<T>(Vector2<T> vector) => new(vector.X, vector.Y, T.Zero);

    /// <summary>
    /// Equality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are equal, false otherwise</returns>
    public static bool operator ==(Vector3<T> a, Vector3<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    public static bool operator !=(Vector3<T> a, Vector3<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    public static bool operator <(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    public static bool operator >(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    public static bool operator <=(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    public static bool operator >=(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    public static Vector3<T> operator -(Vector3<T> a) => new(-a.X, -a.Y, -a.Z);

    /// <summary>
    /// Plus operation on a vector
    /// </summary>
    /// <param name="a">Vector to apply the plus to</param>
    /// <returns>The vector where all the components had the plus operator applied to</returns>
    public static Vector3<T> operator +(Vector3<T> a) => new(+a.X, +a.Y, +a.Z);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    public static Vector3<T> operator *(Vector3<T> a, T b) => new(a.X * b, a.Y * b, a.Z * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    public static Vector3<T> operator /(Vector3<T> a, T b) => new(a.X / b, a.Y / b, a.Z / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector3<T> operator %(Vector3<T> a, T b) => new(a.X % b, a.Y % b, a.Z % b);

    /// <summary>
    /// Per component modulo operator
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Vector containing which values to modulo the components by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector3<T> operator %(Vector3<T> a, Vector3<T> b) => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z);
}
