using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Extensions.Types;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Integer three component vector
/// </summary>
[PublicAPI]
public readonly struct Vector3<T> : IVector<Vector3<T>, T>, IDivisionOperators<Vector3<T>, T, Vector3<T>>, IMultiplyOperators<Vector3<T>, T, Vector3<T>>,
                                    IModulusOperators<Vector3<T>, T, Vector3<T>>, ICrossProductOperator<Vector3<T>, T, Vector3<T>>
    where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = T.CreateChecked(1E-5);
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

    /// <inheritdoc />
    public static int Dimension => 3;

    /// <summary>
    /// Zero vector
    /// </summary>
    public static Vector3<T> Zero { get; }      = new(T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// One vector
    /// </summary>
    public static  Vector3<T> One { get; }       = new(T.One,  T.One,  T.One);

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

    /// <inheritdoc />
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => index switch
        {
            0 => this.X,
            1 => this.Y,
            2 => this.Z,
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
            2 => this.Z,
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
        get => T.Abs(this.X) + T.Abs(this.Y) + T.Abs(this.Z);
    }

    /// <summary>
    /// Vector swizzling to XZY
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector3<T> XZY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.X, this.Z, this.Y);
    }

    /// <summary>
    /// Vector swizzling to XZY
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector3<T> YXZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.Y, this.X, this.Z);
    }

    /// <summary>
    /// Vector swizzling to XZY
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector3<T> YZX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.Y, this.Z, this.X);
    }

    /// <summary>
    /// Vector swizzling to XZY
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector3<T> ZXY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.Z, this.X, this.Y);
    }

    /// <summary>
    /// Vector swizzling to XZY
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public Vector3<T> ZYX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(this.Z, this.Y, this.X);
    }

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
    /// Vector constructor from a components span
    /// </summary>
    /// <param name="components">Span of components</param>
    /// <exception cref="ArgumentException">If the length of <paramref name="components"/> is greater than <see cref="Dimension"/></exception>
    public Vector3(ReadOnlySpan<T> components)
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

            case 3:
                this.X = components[0];
                this.Y = components[1];
                this.Z = components[2];
                break;

            default:
                throw new UnreachableException("Invalid component dimensions");
        }
    }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector3(Vector3<T> copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
        this.Z = copy.Z;
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? other) => other is Vector3<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector3<T> other) => IsInteger ? this.X == other.X && this.Y == other.Y && this.Z == other.Z
                                                         : Approximately(this.X, other.X) && Approximately(this.Y, other.Y) && Approximately(this.Z, other.Z);

    /// <inheritdoc cref="object.GetHashCode"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Z);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    /// ReSharper disable once TailRecursiveCall - not tail recursive
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? other) => other is Vector3<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Vector3<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"({this.X}, {this.Y}, {this.Z})";

    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? provider)
    {
        return $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)}, {this.Z.ToString(format, provider)})";
    }

    /// <inheritdoc />
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        if (destination.Length < 9) return false;

        destination[0] = '(';
        destination = destination[1..];
        if (!this.X.TryFormat(destination, out int xWritten, format, provider)) return false;

        destination = destination[xWritten..];
        if (destination.Length < 7) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Y.TryFormat(destination, out int yWritten, format, provider)) return false;

        destination = destination[yWritten..];
        if (destination.Length < 4) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Z.TryFormat(destination, out int zWritten, format, provider)) return false;

        destination = destination[zWritten..];
        if (destination.Length < 1) return false;

        destination[0] = ')';
        charsWritten = xWritten + yWritten + zWritten + 6;
        return true;
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
    /// Gets the length of this vector in the target floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point result type</typeparam>
    /// <returns>The length of the vector, in the specified floating point type</returns>
    private TResult GetLength<TResult>() where TResult : unmanaged, IBinaryFloatingPointIeee754<TResult>, IMinMaxValue<TResult>
    {
        Vector3<TResult> resultVector = Vector3<TResult>.CreateChecked(this);
        return TResult.Sqrt((resultVector.X * resultVector.X) + (resultVector.Y * resultVector.Y)+ (resultVector.Z * resultVector.Z));
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IEquatable<Vector3<T>>.Equals(Vector3<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int IComparable<Vector3<T>>.CompareTo(Vector3<T> other) => CompareTo(other);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Distance(Vector3<T> a, Vector3<T> b) => (a - b).Length;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ManhattanDistance(Vector3<T> a, Vector3<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y) + T.Abs(a.Z - b.Z);

    /// <summary>
    /// Gives the absolute value of a given vector
    /// </summary>
    /// <param name="vector">Vector to get the absolute value of</param>
    /// <returns>The <paramref name="vector"/> where all it's elements are positive</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Abs(Vector3<T> vector) => new(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z));

    /// <summary>
    /// Does component-wise multiplication on the vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The multiplied vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> ComponentMultiply(Vector3<T> a, Vector3<T> b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Cross(Vector3<T> a, Vector3<T> b) => new(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot(Vector3<T> a, Vector3<T> b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    /// <summary>
    /// Gets the minimum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component minimum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Min(Vector3<T> a, Vector3<T> b) => new(T.Min(a.X, b.X), T.Min(a.Y, b.Y), T.Min(a.Z, b.Z));

    /// <summary>
    /// Gets the maximum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component maximum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> Max(Vector3<T> a, Vector3<T> b) => new(T.Max(a.X, b.X), T.Max(a.Y, b.Y), T.Max(a.Z, b.Z));

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> MinMagnitude(Vector3<T> a, Vector3<T> b) => a.Length < b.Length ? a : b;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> MaxMagnitude(Vector3<T> a, Vector3<T> b) => a.Length > b.Length ? a : b;

    /// <summary>
    /// Parses the two component vector using the given value and number separator
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <param name="separator">Number separator, defaults to ","</param>
    /// <param name="style">Number style</param>
    /// <param name="provider">Format provider</param>
    /// <returns>The parsed vector</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two or three values present after the split</exception>
    public static Vector3<T> Parse(string value, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
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
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> or <paramref name="separator"/> is null or empty</exception>
    /// <exception cref="FormatException">If there isn't exactly two or three values present after the split</exception>
    public static Vector3<T> Parse(ReadOnlySpan<char> value, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Value cannot be null or empty", nameof(value));
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        Span<Range> ranges = stackalloc Range[3];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 3) throw new FormatException("String to parse not properly formatted");

        T x = T.Parse(value[ranges[0]], style, provider);
        T y = T.Parse(value[ranges[1]], style, provider);
        return new Vector3<T>(x, y, written is 3 ? T.Parse(value[ranges[2]], style, provider) : T.Zero);
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
    public static bool TryParse(string? value, out Vector3<T> result, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(separator))
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
    public static bool TryParse(ReadOnlySpan<char> value, out Vector3<T> result, string separator = ",", NumberStyles style = default, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace() || string.IsNullOrEmpty(separator))
        {
            result = Zero;
            return false;
        }

        Span<Range> ranges = stackalloc Range[3];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 3 || !T.TryParse(value[ranges[0]], style, provider, out T x) || !T.TryParse(value[ranges[1]], style, provider, out T y))
        {
            result = Zero;
            return false;
        }

        result = new Vector3<T>(x, y, written is 3 && T.TryParse(value[ranges[2]], style, provider, out T z) ? z : T.Zero);
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
    /// Converts the vector to the target type
    /// </summary>
    /// <typeparam name="TSource">Source number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> CreateChecked<TSource>(Vector3<TSource> vector) where TSource : unmanaged, IBinaryNumber<TSource>, IMinMaxValue<TSource>
    {
        return new Vector3<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y), T.CreateChecked(vector.Z));
    }

    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2, T3}"/> to <see cref="Vector3{T}"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3<T>((T x, T y, T z) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector3{T}"/> to <see cref="ValueTuple{T1, T2, T3}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator (T x, T y, T z)(Vector3<T> vector) => (vector.X, vector.Y, vector.Z);

    /// <summary>
    /// Casts from <see cref="Vector3{T}"/> to <see cref="Vector3{T}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector3<T>(Vector2<T> vector) => new(vector.X, vector.Y, T.Zero);

    /// <summary>
    /// Equality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are equal, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector3<T> a, Vector3<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector3<T> a, Vector3<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Vector3<T> a, Vector3<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator -(Vector3<T> a) => new(-a.X, -a.Y, -a.Z);

    /// <summary>
    /// Plus operation on a vector
    /// </summary>
    /// <param name="a">Vector to apply the plus to</param>
    /// <returns>The vector where all the components had the plus operator applied to</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator +(Vector3<T> a) => new(+a.X, +a.Y, +a.Z);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator +(Vector3<T> a, Vector3<T> b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator -(Vector3<T> a, Vector3<T> b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    /// <summary>
    /// Increment operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component incremented by one</returns>
    public static Vector3<T> operator ++(Vector3<T> a) => new(a.X + T.One, a.Y + T.One, a.Z + T.One);

    /// <summary>
    /// Decrement operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component decremented by one</returns>
    public static Vector3<T> operator --(Vector3<T> a) => new(a.X - T.One, a.Y - T.One, a.Z - T.One);

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator *(Vector3<T> a, T b) => new(a.X * b, a.Y * b, a.Z * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator /(Vector3<T> a, T b) => new(a.X / b, a.Y / b, a.Z / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator %(Vector3<T> a, T b) => new(a.X % b, a.Y % b, a.Z % b);

    /// <summary>
    /// Per component modulo operator
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Vector containing which values to modulo the components by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3<T> operator %(Vector3<T> a, Vector3<T> b) => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z);

    /// <inheritdoc />
    static int INumberBase<Vector3<T>>.Radix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.Radix;
    }

    /// <inheritdoc />
    static Vector3<T> IAdditiveIdentity<Vector3<T>, Vector3<T>>.AdditiveIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Zero;
    }

    /// <inheritdoc />
    static Vector3<T> IMultiplicativeIdentity<Vector3<T>, Vector3<T>>.MultiplicativeIdentity
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
                    result = new Vector3<T>(x, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out T y))
                {
                    result = new Vector3<T>(x, y, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out y) && T.TryConvertFromChecked(components[2], out T z))
                {
                    result = new Vector3<T>(x, y, z);
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
                    result = new Vector3<T>(x, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out T y))
                {
                    result = new Vector3<T>(x, y, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out y) && T.TryConvertFromSaturating(components[2], out T z))
                {
                    result = new Vector3<T>(x, y, z);
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
                    result = new Vector3<T>(x, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out T y))
                {
                    result = new Vector3<T>(x, y, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out y) && T.TryConvertFromTruncating(components[2], out T z))
                {
                    result = new Vector3<T>(x, y, z);
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
    static bool INumberBase<Vector3<T>>.IsCanonical(Vector3<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsComplexNumber(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsEvenInteger(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsFinite(Vector3<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsImaginaryNumber(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsInfinity(Vector3<T> value) => T.IsInfinity(value.X) || T.IsInfinity(value.Y) || T.IsInfinity(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsInteger(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsNaN(Vector3<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsNegative(Vector3<T> value) => T.IsNegative(value.X) || T.IsNegative(value.Y) || T.IsNegative(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsNegativeInfinity(Vector3<T> value) => T.IsNegativeInfinity(value.X) || T.IsNegativeInfinity(value.Y) || T.IsNegativeInfinity(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsNormal(Vector3<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsOddInteger(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsPositive(Vector3<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsPositiveInfinity(Vector3<T> value) => T.IsPositiveInfinity(value.X) || T.IsPositiveInfinity(value.Y) || T.IsPositiveInfinity(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsRealNumber(Vector3<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsSubnormal(Vector3<T> value) => T.IsSubnormal(value.X) || T.IsSubnormal(value.Y) || T.IsSubnormal(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.IsZero(Vector3<T> value) => value == Zero;

    /// <inheritdoc />
    static Vector3<T> INumberBase<Vector3<T>>.MaxMagnitudeNumber(Vector3<T> a, Vector3<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y) || T.IsNaN(a.Z)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y) || T.IsNaN(b.Z)) return a;
        return MaxMagnitude(a, b);
    }

    /// <inheritdoc />
    static Vector3<T> INumberBase<Vector3<T>>.MinMagnitudeNumber(Vector3<T> a, Vector3<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y) || T.IsNaN(a.Z)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y) || T.IsNaN(b.Z)) return a;
        return MinMagnitude(a, b);
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    static bool INumberBase<Vector3<T>>.TryConvertFromChecked<TOther>(TOther value, out Vector3<T> result)
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
                    result = new Vector3<T>(x, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out T y))
                {
                    result = new Vector3<T>(x, y, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 3:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out y) && vector.TryGetComponentChecked(0, out T z))
                {
                    result = new Vector3<T>(x, y, z);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    static bool INumberBase<Vector3<T>>.TryConvertFromSaturating<TOther>(TOther value, out Vector3<T> result)
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

            case 3:
                if (vector.TryGetComponentSaturating(0, out x) && vector.TryGetComponentSaturating(0, out y) && vector.TryGetComponentSaturating(0, out T z))
                {
                    result = new Vector3<T>(x, y, z);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    static bool INumberBase<Vector3<T>>.TryConvertFromTruncating<TOther>(TOther value, out Vector3<T> result)
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

            case 3:
                if (vector.TryGetComponentTruncating(0, out x) && vector.TryGetComponentTruncating(0, out y) && vector.TryGetComponentTruncating(0, out T z))
                {
                    result = new Vector3<T>(x, y, z);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    static bool INumberBase<Vector3<T>>.TryConvertToChecked<TOther>(Vector3<T> value, [MaybeNullWhen(false)] out TOther result)
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
    static bool INumberBase<Vector3<T>>.TryConvertToSaturating<TOther>(Vector3<T> value, [MaybeNullWhen(false)] out TOther result)
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
    static bool INumberBase<Vector3<T>>.TryConvertToTruncating<TOther>(Vector3<T> value, [MaybeNullWhen(false)] out TOther result)
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
    static Vector3<T> IParsable<Vector3<T>>.Parse(string s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3<T> INumberBase<Vector3<T>>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3<T> INumberBase<Vector3<T>>.Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3<T> ISpanParsable<Vector3<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector3<T>>.TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Vector3<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<Vector3<T>>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Vector3<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<Vector3<T>>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector3<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3<T> IMultiplyOperators<Vector3<T>, Vector3<T>, Vector3<T>>.operator *(Vector3<T> left, Vector3<T> right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector3<T> IDivisionOperators<Vector3<T>, Vector3<T>, Vector3<T>>.operator /(Vector3<T> left, Vector3<T> right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
}
