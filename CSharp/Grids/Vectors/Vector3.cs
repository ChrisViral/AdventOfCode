using System;
using System.Collections.Generic;
using Vector2 = AdventOfCode.Grids.Vectors.Vector2<int>;

namespace AdventOfCode.Grids.Vectors;

/// <summary>
/// Integer three component vector
/// </summary>
public readonly struct Vector3<T> : IAdditionOperators<Vector3<T>, Vector3<T>, Vector3<T>>, ISubtractionOperators<Vector3<T>, Vector3<T>, Vector3<T>>,
                                    IUnaryNegationOperators<Vector3<T>, Vector3<T>>, IUnaryPlusOperators<Vector3<T>, Vector3<T>>,
                                    IComparisonOperators<Vector3<T>, Vector3<T>>, IMinMaxValue<Vector3<T>>, IFormattable,
                                    IDivisionOperators<Vector3<T>, T, Vector3<T>>, IMultiplyOperators<Vector3<T>, T, Vector3<T>>, IModulusOperators<Vector3<T>, T, Vector3<T>>
                                    where T : IBinaryNumber<T>, IMinMaxValue<T>
{
    #region Constants
    private static readonly bool isInteger = typeof(T).IsAssignableTo(typeof(IBinaryInteger<>));
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
    public static Vector3<T> MinValue { get; } = new(T.MinValue, T.MinValue, T.MinValue);
    /// <summary>
    /// Maximum vector value
    /// </summary>
    public static Vector3<T> MaxValue { get; } = new(T.MaxValue, T.MaxValue, T.MaxValue);
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
    /// X component of the Vector
    /// </summary>
    public T Z { get; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    /// ReSharper disable once MemberCanBePrivate.Global
    public double Length { get; }

    /// <summary>
    /// Creates an irreducible version of this vector<br/>
    /// NOTE: If this is an floating point vector, the normalized version is returned instead.
    /// </summary>
    /// <returns>The fully reduced version of this vector</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public Vector3<T> Reduced
    {
        get
        {
            if (!isInteger)
            {
                return Normalized;
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

            b |= a;
            T c = T.Abs(this.Z);
            while (b != T.Zero && c != T.Zero)
            {
                if (b > c)
                {
                    b %= c;
                }
                else
                {
                    c %= b;
                }
            }

            T gcd = b | c;

            return this / gcd;
        }
    }

    /// <summary>
    /// Creates a normalized version of this vector<br/>
    /// NOTE: If this is an integer vector, the reduced version is returned instead.
    /// </summary>
    /// <returns>The vector normalized</returns>
    /// ReSharper disable once MemberCanBePrivate.Global
    public Vector3<T> Normalized => isInteger ? this.Reduced : this / T.Create(this.Length);
    #endregion

    #region Constructors
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
        this.Length = GetLength<double>(x, y, z);
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
        this.Length = copy.Length;
    }
    #endregion

    #region Methods
    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? other) => other is Vector3<T> vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    /// ReSharper disable once MemberCanBePrivate.Global
    public bool Equals(in Vector3<T> other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;

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
    public string ToString(string? format, IFormatProvider? provider) => $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)}, {this.Z.ToString(format, provider)})";

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
    public IEnumerable<Vector3<T>> Adjacent()
    {
        if (!isInteger)
        {
            yield break;
        }

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

    /// <summary>
    /// Converts a vector to the target type
    /// </summary>
    /// <typeparam name="TResult">Number type</typeparam>
    /// <returns>The vector converted to the specified type</returns>
    /// ReSharper disable once UnusedMember.Local
    private Vector3<TResult> Convert<TResult>() where TResult : IBinaryNumber<TResult>, IMinMaxValue<TResult> => new(TResult.Create(this.X), TResult.Create(this.Y), TResult.Create(this.Z));

    /// <inheritdoc cref="IEquatable{T}"/>
    bool IEquatable<Vector3<T>>.Equals(Vector3<T> other) => Equals(other);

    /// <inheritdoc cref="IComparable{T}"/>
    int IComparable<Vector3<T>>.CompareTo(Vector3<T> other) => CompareTo(other);
    #endregion

    #region Static methods
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
    /// Gets the length of this vector in the target floating point type
    /// </summary>
    /// <typeparam name="TResult">Floating point result type</typeparam>
    /// <param name="x">X Parameter</param>
    /// <param name="y">Y parameter</param>
    /// <param name="z">Z parameter</param>
    /// <returns>The length of the vector, in the specified floating point type</returns>
    private static TResult GetLength<TResult>(T x, T y, T z) where TResult : IBinaryFloatingPoint<TResult> => TResult.Sqrt(TResult.Create((x * x) + (y * y) + (z * z)));
    #endregion

    #region Operators
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
    #endregion
}