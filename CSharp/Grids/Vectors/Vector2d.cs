using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Grids.Vectors;

/// <summary>
/// Integer two component vector
/// </summary>
public readonly struct Vector2d : IComparable, IComparable<Vector2d>, IEquatable<Vector2d>, IFormattable
{
    #region Constants
    private const double TOLERANCE = 1E-5;
    private static readonly Regex directionMatch = new(@"^\s*(U|D|L|R)(\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    /// <summary>Zero vector</summary>
    public static readonly Vector2d Zero  = new(0d, 0d);
    /// <summary>One vector</summary>
    public static readonly Vector2d One   = new(1d, 1d);
    /// <summary>Up vector</summary>
    public static readonly Vector2d Up    = new(0d, -1d);
    /// <summary>Down vector</summary>
    public static readonly Vector2d Down  = new(0d, 1d);
    /// <summary>Left vector</summary>
    public static readonly Vector2d Left  = new(-1d, 0d);
    /// <summary>Right vector</summary>
    public static readonly Vector2d Right = new(1d, 0d);
    #endregion
        
    #region Propeties
    /// <summary>
    /// X component of the Vector
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Y component of the Vector
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Length of the Vector
    /// </summary>
    public double Length { get; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Vector2d"/> with the specified components
    /// </summary>
    /// <param name="x">X component</param>
    /// <param name="y">Y component</param>
    public Vector2d(double x, double y)
    {
        this.X = x;
        this.Y = y;
        this.Length = Math.Sqrt(x * x + y * y);
    }
        
    /// <summary>
    /// Creates a new <see cref="Vector2d"/> from a given two component tuple
    /// </summary>
    /// <param name="tuple">Tuple to create the Vector from</param>
    public Vector2d((double x, double y) tuple) : this(tuple.x, tuple.y) { }

    /// <summary>
    /// Vector copy constructor
    /// </summary>
    /// <param name="copy">Vector to copy</param>
    public Vector2d(in Vector2d copy)
    {
        this.X = copy.X;
        this.Y = copy.Y;
        this.Length = copy.Length;
    }
    #endregion
        
    #region Methods
    /// <inheritdoc cref="object.Equals(object)"/>
    public override bool Equals(object? other) => other is Vector2 vector && Equals(vector);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(in Vector2d other) => Math.Abs(this.X - other.X) < TOLERANCE && Math.Abs(this.Y - other.Y) < TOLERANCE;

    /// <inheritdoc cref="object.GetHashCode"/>
    public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    /// ReSharper disable once TailRecursiveCall - not tail recursive
    public int CompareTo(object? other) => other is Vector2d vector ? CompareTo(vector) : 0;

    /// <inheritdoc cref="IComparable{T}.CompareTo"/>
    public int CompareTo(in Vector2d other) => this.Length.CompareTo(other.Length);

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"({this.X}, {this.Y})";
        
    /// <summary>
    /// Deconstructs this vector into a tuple
    /// </summary>
    /// <param name="x">X parameter</param>
    /// <param name="y">Y parameter</param>
    public void Deconstruct(out double x, out double y)
    {
        x = this.X;
        y = this.Y;
    }
        
    /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
    public string ToString(string? format, IFormatProvider? provider) => $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)})";

    /// <summary>
    /// Creates a new vector resulting in the moving of this vector in the specified direction
    /// </summary>
    /// <param name="directions">Direction to move in</param>
    /// <returns>The new, moved vector</returns>
    public Vector2d Move(Directions directions) => this + directions.ToVector();

    /// <inheritdoc cref="IEquatable{T}"/>
    bool IEquatable<Vector2d>.Equals(Vector2d other) => Equals(other);
        
    /// <inheritdoc cref="IComparable{T}"/>
    int IComparable<Vector2d>.CompareTo(Vector2d other) => CompareTo(other);
    #endregion
        
    #region Static methods
    /// <summary>
    /// Calculates the distance between two vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The distance between both vectors</returns>
    public static double Distance(in Vector2d a, in Vector2d b) => (a - b).Length;

    /// <summary>
    /// Calculates the signed angle, in degrees, between two vectors. The result is in the range [-180, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle Angle(in Vector2d a, in Vector2d b) => Vectors.Angle.FromRadians(Math.Atan2(a.X * b.Y - a.Y * b.X, a.X * b.X + a.Y * b.Y));

    /// <summary>
    /// Calculates the absolute angle, in degrees, between two vectors. The result is in the range [0, 180]
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The angle between both vectors</returns>
    public static Angle AbsoluteAngle(in Vector2d a, in Vector2d b) => Vectors.Angle.FromRadians(Math.Acos(Dot(a, b) / (a.Length * b.Length)));

    /// <summary>
    /// Calculates the dot product of both vectors
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>The dot product of both vectors</returns>
    public static double Dot(in Vector2d a, in Vector2d b) => a.X * b.X + a.Y * b.Y;
        
    /// <summary>
    /// Parses the Vector2 from a direction and distance
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <returns>The parsed Vector2</returns>
    /// <exception cref="FormatException">If the direction format or string format is invalid</exception>
    /// <exception cref="OverflowException">If the number parse causes an overflow</exception>
    public static Vector2d ParseFromDirection(string value)
    {
        GroupCollection groups = directionMatch.Match(value).Groups;
        //Parse direction first
        Vector2d direction = groups[1].Value switch
        {
            "U" => Up,
            "D" => Down,
            "L" => Left,
            "R" => Right,
            _   => throw new FormatException($"Direction value ({groups[1].Value}) cannot be parsed into a direction")
        };
        //Return with correct length
        return direction * int.Parse(groups[2].Value);
    }

    /// <summary>
    /// Tries to parses the Vector2 from a direction and distance
    /// </summary>
    /// <param name="value">String to parse</param>
    /// <param name="direction">Result out parameter</param>
    /// <returns>True if the vector was successfully parsed, false otherwise</returns>
    public static bool TryParseFromDirection(string value, out Vector2d direction)
    {
        //Check if it matches at all
        direction = Zero;
        Match match = directionMatch.Match(value);
        if (!match.Success) return false;

            
        GroupCollection groups = match.Groups;
        if (groups.Count is not 3) return false;
        if (!int.TryParse(groups[2].Value, out int distance)) return false;
        Vector2d dir;
        switch (groups[1].Value)
        {
            case "U":
                dir = Up;
                break;
            case "D":
                dir = Down;
                break;
            case "L":
                dir = Left;
                break;
            case "R":
                dir = Right;
                break;
                
            default:
                return false;
        }

        direction = dir * distance;
        return true;

    }
    #endregion

    #region Operators
    /// <summary>
    /// Cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="Vector2d"/>
    /// </summary>
    /// <param name="tuple">Tuple to cast from</param>
    public static implicit operator Vector2d((double x, double y) tuple) => new(tuple);

    /// <summary>
    /// Casts from <see cref="Vector2d"/> to <see cref="ValueTuple{T1, T2}"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    public static implicit operator (double x, double y)(in Vector2d vector) => (vector.X, vector.Y);

    /// <summary>
    /// Casts from <see cref="Vector2"/> to <see cref="Vector2d"/>
    /// </summary>
    /// <param name="vector">Vector to cast from</param>
    public static implicit operator Vector2d(Vector2 vector) => new(vector.X, vector.Y);

    /// <summary>
    /// Equality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are equal, false otherwise</returns>
    public static bool operator ==(in Vector2d a, in Vector2d b) => a.Equals(b);
        
    /// <summary>
    /// Inequality between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if both vectors are unequal, false otherwise</returns>
    public static bool operator !=(in Vector2d a, in Vector2d b) => !a.Equals(b);

    /// <summary>
    /// Less-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
    public static bool operator <(in Vector2d a, in Vector2d b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
    public static bool operator >(in Vector2d a, in Vector2d b) => a.CompareTo(b) > 0;
        
    /// <summary>
    /// Less-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
    public static bool operator <=(in Vector2d a, in Vector2d b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than-or-equals between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
    public static bool operator >=(in Vector2d a, in Vector2d b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Negate operation on a vector
    /// </summary>
    /// <param name="a">Vector to negate</param>
    /// <returns>The vector with all it's components negated</returns>
    public static Vector2d operator -(in Vector2d a) => new(-a.X, -a.Y);

    /// <summary>
    /// Add operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise addition on both Vectors</returns>
    public static Vector2d operator +(in Vector2d a, in Vector2d b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtract operation between two vectors
    /// </summary>
    /// <param name="a">First Vector</param>
    /// <param name="b">Second Vector</param>
    /// <returns>The result of the component-wise subtraction on both Vectors</returns>
    public static Vector2d operator -(in Vector2d a, in Vector2d b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Scalar integer multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2d operator *(in Vector2d a, int b) => new(a.X * b, a.Y * b);

    /// <summary>
    /// Scalar integer division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2d operator /(in Vector2d a, int b) => new(a.X / b, a.Y / b);
        
    /// <summary>
    /// Scalar floating point multiplication on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to multiply by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2d operator *(in Vector2d a, double b) => new(a.X * b, a.Y * b);

    /// <summary>
    /// Scalar floating point division on a Vector
    /// </summary>
    /// <param name="a">Vector to scale</param>
    /// <param name="b">Scalar to divide by</param>
    /// <returns>The scaled vector</returns>
    public static Vector2d operator /(in Vector2d a, double b) => new(a.X / b, a.Y / b);

    /// <summary>
    /// Modulo scalar operation on a Vector
    /// </summary>
    /// <param name="a">Vector to use the Modulo onto</param>
    /// <param name="b">Scalar to modulo by</param>
    /// <returns>The vector with the results of the modulo operation component wise</returns>
    public static Vector2d operator %(in Vector2d a, int b) => new(a.X % b, a.Y % b);
    #endregion
}