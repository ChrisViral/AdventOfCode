using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils.Extensions.Types;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Integer three component vector
/// </summary>
[PublicAPI]
public readonly struct Vector4<T> : IVector<Vector4<T>, T>, IDivisionOperators<Vector4<T>, T, Vector4<T>>, IMultiplyOperators<Vector4<T>, T, Vector4<T>>,
                                    IModulusOperators<Vector4<T>, T, Vector4<T>>
    where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
{
    /// <summary>
    /// Internal data buffer
    /// </summary>
    [InlineArray(4)]
    internal struct Data
    {
        private T element;
    }

    /// <summary>If this is an integer vector type</summary>
    private static readonly bool IsInteger = typeof(T).IsGenericImplementationOf(typeof(IBinaryInteger<>));
    /// <summary>Small comparison value for floating point numbers</summary>
    private static readonly T Epsilon = T.CreateChecked(1E-5);
    /// <summary>Parse number style</summary>
    /// ReSharper disable once StaticMemberInGenericType
    private static readonly NumberStyles Style = IsInteger ? NumberStyles.Integer : NumberStyles.Float | NumberStyles.AllowThousands;
    /// <summary>Up vector</summary>
    public static readonly Vector4<T> Up        = new(T.Zero, T.One,  T.Zero, T.Zero);
    /// <summary>Down vector</summary>
    public static readonly Vector4<T> Down      = new(T.Zero, -T.One, T.Zero, T.Zero);
    /// <summary>Left vector</summary>
    public static readonly Vector4<T> Left      = new(-T.One, T.Zero, T.Zero, T.Zero);
    /// <summary>Right vector</summary>
    public static readonly Vector4<T> Right     = new(T.One,  T.Zero, T.Zero, T.Zero);
    /// <summary>Forward vector</summary>
    public static readonly Vector4<T> Forwards  = new(T.Zero, T.Zero, T.One,  T.Zero);
    /// <summary>Backward vector</summary>
    public static readonly Vector4<T> Backwards = new(T.Zero, T.Zero, -T.One, T.Zero);
    /// <summary>Inward vector</summary>
    public static readonly Vector4<T> Inwards   = new(T.Zero, T.Zero, T.Zero, T.One);
    /// <summary>Outkward vector</summary>
    public static readonly Vector4<T> Outwards  = new(T.Zero, T.Zero, T.Zero, -T.One);

    /// <inheritdoc />
    public static int Dimension => 4;

    /// <summary>
    /// Zero vector
    /// </summary>
    public static Vector4<T> Zero { get; }      = new(T.Zero, T.Zero, T.Zero, T.Zero);

    /// <summary>
    /// One vector
    /// </summary>
    public static  Vector4<T> One { get; }       = new(T.One,  T.One,  T.One, T.One);

    /// <summary>
    /// Minimum vector value
    /// </summary>
    public static Vector4<T> MinValue  { get; } = new(T.MinValue, T.MinValue, T.MinValue, T.MaxValue);

    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector4<T> MaxValue  { get; } = new(T.MaxValue, T.MaxValue, T.MaxValue, T.MaxValue);

    /// <summary>
    /// Components array
    /// </summary>
    internal readonly Data data;

    /// <summary>
    /// X component of the Vector
    /// </summary>
    public T X
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => this.data[0];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => this.data[0] = value;
    }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public T Y
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => this.data[1];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => this.data[1] = value;
    }

    /// <summary>
    /// Z component of the Vector
    /// </summary>
    public T Z
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => this.data[2];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => this.data[2] = value;
    }

    /// <summary>
    /// W component of the Vector
    /// </summary>
    public T W
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get  => this.data[3];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        init => this.data[3] = value;
    }

    /// <inheritdoc />
    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.data[index];
    }

    /// <inheritdoc />
    public T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.data[index];
    }

    /// <inheritdoc />
    public double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetLength<double>();
    }

    /// <summary>
    /// Creates a new <see cref="Vector4{T}"/> with the specified components
    /// </summary>
    /// <param name="x">X component</param>
    /// <param name="y">Y component</param>
    /// <param name="z">Z component</param>
    /// <param name="w">W component</param>
    public Vector4(T x, T y, T z, T w)
    {
        this.data[0] = x;
        this.data[1] = y;
        this.data[2] = z;
        this.data[3] = w;
    }

    /// <summary>
    /// Creates a new <see cref="Vector4{T}"/> from a given three component tuple
    /// </summary>
    /// <param name="tuple">Tuple to create the Vector from</param>
    /// ReSharper disable once UseDeconstructionOnParameter
    /// ReSharper disable once MemberCanBePrivate.Global
    public Vector4((T x, T y, T z, T w) tuple) : this(tuple.x, tuple.y, tuple.z, tuple.w) { }

    /// <summary>
    /// Vector constructor from a components span
    /// </summary>
    /// <param name="components">Span of components</param>
    /// <exception cref="ArgumentException">If the length of <paramref name="components"/> is greater than <see cref="Dimension"/></exception>
    public Vector4(ReadOnlySpan<T> components)
    {
        if (components.Length > Dimension) throw new ArgumentException("Components span cannot be larger than vector dimensions", nameof(components));

        switch (components.Length)
        {
            case 1:
                this.data[0] = components[0];
                break;

            case 2:
                this.data[0] = components[0];
                this.data[1] = components[1];
                break;

            case 3:
                this.data[0] = components[0];
                this.data[1] = components[1];
                this.data[2] = components[2];
                break;

            case 4:
                this.data[0] = components[0];
                this.data[1] = components[1];
                this.data[2] = components[2];
                this.data[3] = components[3];
                break;

            default:
                throw new UnreachableException("Invalid component dimensions");
        }
    }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector4(Vector4<T> copy)
    {
        this.data = copy.data;
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? other) => other is Vector4<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector4<T> other) => IsInteger ? this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W
                                                : Approximately(this.X, other.X) && Approximately(this.Y, other.Y) && Approximately(this.Z, other.Z) && Approximately(this.W, other.W);

    /// <inheritdoc cref="object.GetHashCode"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Z, this.W);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    /// ReSharper disable once TailRecursiveCall - not tail recursive
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? other) => other is Vector4<T> vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Vector4<T> other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => $"({this.X}, {this.Y}, {this.Z}, {this.W})";

    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? provider)
    {
        return $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)}, {this.Z.ToString(format, provider)}, {this.W.ToString(format, provider)})";
    }

    /// <inheritdoc />
    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        if (destination.Length < 12) return false;

        destination[0] = '(';
        destination = destination[1..];
        if (!this.X.TryFormat(destination, out int xWritten, format, provider)) return false;

        destination = destination[xWritten..];
        if (destination.Length < 10) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Y.TryFormat(destination, out int yWritten, format, provider)) return false;

        destination = destination[yWritten..];
        if (destination.Length < 7) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Z.TryFormat(destination, out int zWritten, format, provider)) return false;

        destination = destination[zWritten..];
        if (destination.Length < 4) return false;

        destination[0] = ',';
        destination[1] = ' ';
        destination = destination[2..];
        if (!this.Z.TryFormat(destination, out int wWritten, format, provider)) return false;

        destination = destination[wWritten..];
        if (destination.Length < 1) return false;

        destination[0] = ')';
        charsWritten = xWritten + yWritten + zWritten + 8;
        return true;
    }

    /// <summary>
    /// Deconstructs this vector into a tuple
    /// </summary>
    /// <param name="x">X parameter</param>
    /// <param name="y">Y parameter</param>
    /// <param name="z">Z parameter</param>
    /// <param name="w">W parameter</param>
    public void Deconstruct(out T x, out T y, out T z, out T w)
    {
        x = this.X;
        y = this.Y;
        z = this.Z;
        w = this.W;
    }

    /// <summary>
    /// Gets the length of this vector in the target floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point result type</typeparam>
    /// <returns>The length of the vector, in the specified floating point type</returns>
    private TResult GetLength<TResult>() where TResult : unmanaged, IBinaryFloatingPointIeee754<TResult>, IMinMaxValue<TResult>
    {
        Vector4<TResult> resultVector = Vector4<TResult>.CreateChecked(this);
        return TResult.Sqrt((resultVector.X * resultVector.X) + (resultVector.Y * resultVector.Y) + (resultVector.Z * resultVector.Z) + (resultVector.W * resultVector.W));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Distance(Vector4<T> a, Vector4<T> b) => (a - b).Length;

    /// <inheritdoc cref="Distance" />
    /// <typeparam name="TResult">Result value type</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Distance<TResult>(Vector4<T> a, Vector4<T> b) where TResult : unmanaged, IBinaryFloatingPointIeee754<TResult>, IMinMaxValue<TResult>
    {
        return (a - b).GetLength<TResult>();
    }

    /// <summary>
    /// Gives the absolute value of a given vector
    /// </summary>
    /// <param name="vector">Vector to get the absolute value of</param>
    /// <returns>The <paramref name="vector"/> where all it's elements are positive</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Abs(Vector4<T> vector) => new(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z), T.Abs(vector.W));

    /// <summary>
    /// Does component-wise multiplication on the vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The multiplied vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> ComponentMultiply(Vector4<T> a, Vector4<T> b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);

    /// <inheritdoc cref="ComponentMultiply"/>
    /// <typeparam name="TResult">Result value type</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<TResult> ComponentMultiply<TResult>(Vector4<T> a, Vector4<T> b) where TResult : unmanaged, IBinaryNumber<TResult>, IMinMaxValue<TResult>
    {
        return Vector4<TResult>.ComponentMultiply(Vector4<TResult>.CreateChecked(a), Vector4<TResult>.CreateChecked(b));
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot(Vector4<T> a, Vector4<T> b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

    /// <inheritdoc cref="Dot"/>
    /// <typeparam name="TResult">Result value type</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TResult Dot<TResult>(Vector4<T> a, Vector4<T> b) where TResult : unmanaged, IBinaryNumber<TResult>, IMinMaxValue<TResult>
    {
        return Vector4<TResult>.Dot(Vector4<TResult>.CreateChecked(a), Vector4<TResult>.CreateChecked(b));
    }

    /// <summary>
    /// Gets the minimum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component minimum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Min(Vector4<T> a, Vector4<T> b) => new(T.Min(a.X, b.X), T.Min(a.Y, b.Y), T.Min(a.Z, b.Z), T.Min(a.W, b.W));

    /// <summary>
    /// Gets the maximum value vector for the passed values
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The per-component maximum vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> Max(Vector4<T> a, Vector4<T> b) => new(T.Max(a.X, b.X), T.Max(a.Y, b.Y), T.Max(a.Z, b.Z), T.Max(a.W, b.W));

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> MinMagnitude(Vector4<T> a, Vector4<T> b) => a.Length < b.Length ? a : b;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> MaxMagnitude(Vector4<T> a, Vector4<T> b) => a.Length > b.Length ? a : b;

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
    public static Vector4<T> Parse(string value, string separator = ",", NumberStyles? style = null, IFormatProvider? provider = null)
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
    public static Vector4<T> Parse(ReadOnlySpan<char> value, string separator = ",", NumberStyles? style = null, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace()) throw new ArgumentException("Value cannot be null or empty", nameof(value));
        if (string.IsNullOrEmpty(separator)) throw new ArgumentNullException(nameof(separator), "Separator cannot be null or empty");

        Span<Range> ranges = stackalloc Range[4];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 4) throw new FormatException("String to parse not properly formatted");

        style ??= Style;
        T x = T.Parse(value[ranges[0]], style.Value, provider);
        T y = T.Parse(value[ranges[1]], style.Value, provider);
        return new Vector4<T>(x, y,
                              written >= 3 ? T.Parse(value[ranges[2]], style.Value, provider) : T.Zero,
                              written is 4 ? T.Parse(value[ranges[3]], style.Value, provider) : T.Zero);
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
    public static bool TryParse(string? value, out Vector4<T> result, string separator = ",", NumberStyles? style = null, IFormatProvider? provider = null)
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
    public static bool TryParse(ReadOnlySpan<char> value, out Vector4<T> result, string separator = ",", NumberStyles? style = null, IFormatProvider? provider = null)
    {
        if (value.IsEmpty || value.IsWhiteSpace() || string.IsNullOrEmpty(separator))
        {
            result = Zero;
            return false;
        }

        style ??= Style;
        Span<Range> ranges = stackalloc Range[4];
        int written = value.Split(ranges, separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (written is < 2 or > 4 || !T.TryParse(value[ranges[0]], style.Value, provider, out T x) || !T.TryParse(value[ranges[1]], style.Value, provider, out T y))
        {
            result = Zero;
            return false;
        }

        result = new Vector4<T>(x, y,
                                written >= 3 && T.TryParse(value[ranges[2]], style.Value, provider, out T z) ? z : T.Zero,
                                written is 4 && T.TryParse(value[ranges[3]], style.Value, provider, out T w) ? w : T.Zero);
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
    public static Vector4<T> CreateChecked<TSource>(Vector4<TSource> vector) where TSource : unmanaged, IBinaryNumber<TSource>, IMinMaxValue<TSource>
    {
        return new Vector4<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y), T.CreateChecked(vector.Z), T.CreateChecked(vector.W));
    }

    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2, T3, T4}"/> to <see cref="Vector4{T}"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4<T>((T x, T y, T z, T w) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector4{T}"/> to <see cref="ValueTuple{T1, T2, T3, T4}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator (T x, T y, T z, T w)(Vector4<T> vector) => (vector.X, vector.Y, vector.Z, vector.W);

    /// <summary>
    /// Casts from <see cref="Vector2{T}"/> to <see cref="Vector4{T}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4<T>(Vector2<T> vector) => new(vector.X, vector.Y, T.Zero, T.Zero);

    /// <summary>
    /// Casts from <see cref="Vector3{T}"/> to <see cref="Vector4{T}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Vector4<T>(Vector3<T> vector) => new(vector.X, vector.Y, vector.Z, T.Zero);

    /// <summary>
    /// Equality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are equal, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector4<T> a, Vector4<T> b) => a.Equals(b);

    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector4<T> a, Vector4<T> b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Vector4<T> a, Vector4<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Vector4<T> a, Vector4<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Vector4<T> a, Vector4<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Vector4<T> a, Vector4<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator -(Vector4<T> a) => new(-a.X, -a.Y, -a.Z, -a.W);

    /// <summary>
    /// Plus operation on a vector
    /// </summary>
    /// <param name="a">Vector to apply the plus to</param>
    /// <returns>The vector where all the components had the plus operator applied to</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator +(Vector4<T> a) => new(+a.X, +a.Y, +a.Z, +a.W);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator +(Vector4<T> a, Vector4<T> b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator -(Vector4<T> a, Vector4<T> b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

    /// <summary>
    /// Increment operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component incremented by one</returns>
    public static Vector4<T> operator ++(Vector4<T> a) => new(a.X + T.One, a.Y + T.One, a.Z + T.One, a.W + T.One);

    /// <summary>
    /// Decrement operation on vector
    /// </summary>
    /// <param name="a">Vector</param>
    /// <returns>The vector with each component decremented by one</returns>
    public static Vector4<T> operator --(Vector4<T> a) => new(a.X - T.One, a.Y - T.One, a.Z - T.One, a.W - T.One);

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator *(Vector4<T> a, T b) => new(a.X * b, a.Y * b, a.Z * b, a.W * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator /(Vector4<T> a, T b) => new(a.X / b, a.Y / b, a.Z / b, a.W / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator %(Vector4<T> a, T b) => new(a.X % b, a.Y % b, a.Z % b, a.W % b);

    /// <summary>
    /// Per component modulo operator
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Vector containing which values to modulo the components by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4<T> operator %(Vector4<T> a, Vector4<T> b) => new(a.X % b.X, a.Y % b.Y, a.Z % b.Z, a.W % b.W);

    /// <inheritdoc />
    static int INumberBase<Vector4<T>>.Radix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => T.Radix;
    }

    /// <inheritdoc />
    static Vector4<T> IAdditiveIdentity<Vector4<T>, Vector4<T>>.AdditiveIdentity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Zero;
    }

    /// <inheritdoc />
    static Vector4<T> IMultiplicativeIdentity<Vector4<T>, Vector4<T>>.MultiplicativeIdentity
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
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromChecked(this.data[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    bool IVector.TryGetComponentSaturating<TResult>(int index, out TResult result)
    {
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromSaturating(this.data[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    bool IVector.TryGetComponentTruncating<TResult>(int index, out TResult result)
    {
        if (index >= 0 && index < Dimension) return TResult.TryConvertFromTruncating(this.data[index], out result);

        result = default;
        return false;
    }

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out y) && T.TryConvertFromChecked(components[2], out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 4:
                if (T.TryConvertFromChecked(components[0], out x) && T.TryConvertFromChecked(components[1], out y) && T.TryConvertFromChecked(components[2], out z) && T.TryConvertFromChecked(components[3], out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
                    return true;
                }
                result = null;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out y) && T.TryConvertFromSaturating(components[2], out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 4:
                if (T.TryConvertFromSaturating(components[0], out x) && T.TryConvertFromSaturating(components[1], out y) && T.TryConvertFromSaturating(components[2], out z) && T.TryConvertFromSaturating(components[3], out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
                    return true;
                }
                result = null;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 2:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 3:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out y) && T.TryConvertFromTruncating(components[2], out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = null;
                return false;

            case 4:
                if (T.TryConvertFromTruncating(components[0], out x) && T.TryConvertFromTruncating(components[1], out y) && T.TryConvertFromTruncating(components[2], out z) && T.TryConvertFromTruncating(components[3], out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
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
    static bool INumberBase<Vector4<T>>.IsCanonical(Vector4<T> value) => T.IsCanonical(value.X) && T.IsCanonical(value.Y) && T.IsCanonical(value.Z) && T.IsCanonical(value.Z);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsComplexNumber(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsEvenInteger(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsFinite(Vector4<T> value) => T.IsFinite(value.X) && T.IsFinite(value.Y) && T.IsFinite(value.Z) && T.IsFinite(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsImaginaryNumber(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsInfinity(Vector4<T> value) => T.IsInfinity(value.X) || T.IsInfinity(value.Y) || T.IsInfinity(value.Z) || T.IsInfinity(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsInteger(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsNaN(Vector4<T> value) => T.IsNaN(value.X) || T.IsNaN(value.Y) || T.IsNaN(value.Z) || T.IsNaN(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsNegative(Vector4<T> value) => T.IsNegative(value.X) || T.IsNegative(value.Y) || T.IsNegative(value.Z) || T.IsNegative(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsNegativeInfinity(Vector4<T> value) => T.IsNegativeInfinity(value.X) || T.IsNegativeInfinity(value.Y) || T.IsNegativeInfinity(value.Z) || T.IsNegativeInfinity(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsNormal(Vector4<T> value) => T.IsNormal(value.X) && T.IsNormal(value.Y) && T.IsNormal(value.Z) && T.IsNormal(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsOddInteger(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsPositive(Vector4<T> value) => T.IsPositive(value.X) && T.IsPositive(value.Y) && T.IsPositive(value.Z) && T.IsPositive(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsPositiveInfinity(Vector4<T> value) => T.IsPositiveInfinity(value.X) || T.IsPositiveInfinity(value.Y) || T.IsPositiveInfinity(value.Z) || T.IsPositiveInfinity(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsRealNumber(Vector4<T> value) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsSubnormal(Vector4<T> value) => T.IsSubnormal(value.X) || T.IsSubnormal(value.Y) || T.IsSubnormal(value.Z) || T.IsSubnormal(value.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.IsZero(Vector4<T> value) => value == Zero;

    /// <inheritdoc />
    static Vector4<T> INumberBase<Vector4<T>>.MaxMagnitudeNumber(Vector4<T> a, Vector4<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y) || T.IsNaN(a.Z) || T.IsNaN(a.W)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y) || T.IsNaN(b.Z) || T.IsNaN(a.W)) return a;
        return MaxMagnitude(a, b);
    }

    /// <inheritdoc />
    static Vector4<T> INumberBase<Vector4<T>>.MinMagnitudeNumber(Vector4<T> a, Vector4<T> b)
    {
        if (T.IsNaN(a.X) || T.IsNaN(a.Y) || T.IsNaN(a.Z) || T.IsNaN(a.W)) return b;
        if (T.IsNaN(b.X) || T.IsNaN(b.Y) || T.IsNaN(b.Z) || T.IsNaN(a.W)) return a;
        return MinMagnitude(a, b);
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    static bool INumberBase<Vector4<T>>.TryConvertFromChecked<TOther>(TOther value, out Vector4<T> result)
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 3:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out y) && vector.TryGetComponentChecked(0, out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 4:
                if (vector.TryGetComponentChecked(0, out x) && vector.TryGetComponentChecked(0, out y) && vector.TryGetComponentChecked(0, out z) && vector.TryGetComponentChecked(0, out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
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
    static bool INumberBase<Vector4<T>>.TryConvertFromSaturating<TOther>(TOther value, out Vector4<T> result)
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentSaturating(0, out x) && vector.TryGetComponentSaturating(0, out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 3:
                if (vector.TryGetComponentSaturating(0, out x) && vector.TryGetComponentSaturating(0, out y) && vector.TryGetComponentSaturating(0, out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 4:
                if (vector.TryGetComponentSaturating(0, out x) && vector.TryGetComponentSaturating(0, out y) && vector.TryGetComponentSaturating(0, out z) && vector.TryGetComponentSaturating(0, out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
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
    static bool INumberBase<Vector4<T>>.TryConvertFromTruncating<TOther>(TOther value, out Vector4<T> result)
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
                    result = new Vector4<T>(x, T.Zero, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 2:
                if (vector.TryGetComponentTruncating(0, out x) && vector.TryGetComponentTruncating(0, out T y))
                {
                    result = new Vector4<T>(x, y, T.Zero, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 3:
                if (vector.TryGetComponentTruncating(0, out x) && vector.TryGetComponentTruncating(0, out y) && vector.TryGetComponentTruncating(0, out T z))
                {
                    result = new Vector4<T>(x, y, z, T.Zero);
                    return true;
                }
                result = default;
                return false;

            case 4:
                if (vector.TryGetComponentTruncating(0, out x) && vector.TryGetComponentTruncating(0, out y) && vector.TryGetComponentTruncating(0, out z) && vector.TryGetComponentTruncating(0, out T w))
                {
                    result = new Vector4<T>(x, y, z, w);
                    return true;
                }
                result = default;
                return false;

            default:
                throw new UnreachableException("Invalid vector dimensions");
        }
    }

    /// <inheritdoc />
    static bool INumberBase<Vector4<T>>.TryConvertToChecked<TOther>(Vector4<T> value, [MaybeNullWhen(false)] out TOther result)
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

        if (vector.TryMakeFromComponentsChecked(value.AsSpan(), out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    static bool INumberBase<Vector4<T>>.TryConvertToSaturating<TOther>(Vector4<T> value, [MaybeNullWhen(false)] out TOther result)
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

        if (vector.TryMakeFromComponentsSaturating(value.AsSpan(), out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    static bool INumberBase<Vector4<T>>.TryConvertToTruncating<TOther>(Vector4<T> value, [MaybeNullWhen(false)] out TOther result)
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

        if (vector.TryMakeFromComponentsTruncating(value.AsSpan(), out IVector? created) && created is TOther other)
        {
            result = other;
            return true;
        }

        result = default;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> IParsable<Vector4<T>>.Parse(string s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> INumberBase<Vector4<T>>.Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> INumberBase<Vector4<T>>.Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse(s, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> ISpanParsable<Vector4<T>>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Vector4<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool INumberBase<Vector4<T>>.TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out Vector4<T> result) => TryParse(s, out result, style: style, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IParsable<Vector4<T>>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Vector4<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool ISpanParsable<Vector4<T>>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Vector4<T> result) => TryParse(s, out result, provider: provider);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> IMultiplyOperators<Vector4<T>, Vector4<T>, Vector4<T>>.operator *(Vector4<T> left, Vector4<T> right) => new(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Vector4<T> IDivisionOperators<Vector4<T>, Vector4<T>, Vector4<T>>.operator /(Vector4<T> left, Vector4<T> right) => new(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
}
