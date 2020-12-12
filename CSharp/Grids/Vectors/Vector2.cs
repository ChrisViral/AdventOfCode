using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdventOfCode.Utils;

namespace AdventOfCode.Grids.Vectors
{
    /// <summary>
    /// Integer two component vector
    /// </summary>
    public readonly struct Vector2 : IComparable, IComparable<Vector2>, IEquatable<Vector2>, IFormattable
    {
        #region Constants
        private static readonly Regex directionMatch = new(@"^\s*(U|D|L|R)(\d+)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>Zero vector</summary>
        public static readonly Vector2 Zero  = new(0, 0);
        /// <summary>One vector</summary>
        public static readonly Vector2 One   = new(1, 1);
        /// <summary>Up vector</summary>
        public static readonly Vector2 Up    = new(0, -1);
        /// <summary>Down vector</summary>
        public static readonly Vector2 Down  = new(0, 1);
        /// <summary>Left vector</summary>
        public static readonly Vector2 Left  = new(-1, 0);
        /// <summary>Right vector</summary>
        public static readonly Vector2 Right = new(1, 0);
        #endregion
        
        #region Propeties
        /// <summary>
        /// X component of the Vector
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y component of the Vector
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Length of the Vector
        /// </summary>
        public double Length { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Vector2"/> with the specified components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Vector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Length = Math.Sqrt(x * x + y * y);
        }
        
        /// <summary>
        /// Creates a new <see cref="Vector2"/> from a given two component tuple
        /// </summary>
        /// <param name="tuple">Tuple to create the Vector from</param>
        public Vector2((int x, int y) tuple) : this(tuple.x, tuple.y) { }

        /// <summary>
        /// Vector copy constructor
        /// </summary>
        /// <param name="copy">Vector to copy</param>
        public Vector2(in Vector2 copy)
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
        public bool Equals(in Vector2 other) => this.X == other.X && this.Y == other.Y;

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

        /// <inheritdoc cref="IComparable.CompareTo"/>
        /// ReSharper disable once TailRecursiveCall - not tail recursive
        public int CompareTo(object? other) => other is Vector2 vector ? CompareTo(vector) : 0;

        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(in Vector2 other)
        {
            int comp = this.X.CompareTo(other.X);
            return comp is 0 ? this.Y.CompareTo(other.Y) : comp;
        }

        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"({this.X}, {this.Y})";
        
        /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)"/>
        public string ToString(string? format, IFormatProvider? provider) => $"({this.X.ToString(format, provider)}, {this.Y.ToString(format, provider)})";

        /// <summary>
        /// Creates a new vector resulting in the moving of this vector in the specified direction
        /// </summary>
        /// <param name="directions">Direction to move in</param>
        /// <returns>The new, moved vector</returns>
        public Vector2 Move(Directions directions) => this + directions.ToVector();

        /// <summary>
        /// Gets all the adjacent Vector2 to this one
        /// </summary>
        /// <returns>Adjacent vectors</returns>
        public IEnumerable<Vector2> Adjacent(bool includeDiagonals = true)
        {
            if (includeDiagonals)
            {
                for (int x = this.X - 1; x <= this.X + 1; x++)
                {
                    for (int y = this.Y - 1; y <= this.Y + 1; y++)
                    {
                        if (x == this.X && y == this.Y) continue;
                        
                        yield return new Vector2(x, y);
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
        public Vector2 Reduced()
        {
            int gcd = AoCUtils.GCD(this.X, this.Y);
            return this / gcd;
        }
        
        /// <inheritdoc cref="IEquatable{T}"/>
        bool IEquatable<Vector2>.Equals(Vector2 other) => Equals(other);
        
        /// <inheritdoc cref="IComparable{T}"/>
        int IComparable<Vector2>.CompareTo(Vector2 other) => CompareTo(other);
        #endregion
        
        #region Static methods
        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The distance between both vectors</returns>
        public static double Distance(in Vector2 a, in Vector2 b) => (a - b).Length;

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        public static int ManhattanDistance(in Vector2 a, in Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

        /// <summary>
        /// Calculates the signed angle, in degrees, between two vectors. The result is in the range [-180, 180]
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The angle between both vectors</returns>
        public static Angle Angle(in Vector2 a, in Vector2 b) => Vectors.Angle.FromRadians(Math.Atan2(a.X * b.Y - a.Y * b.X, a.X * b.X + a.Y * b.Y));

        /// <summary>
        /// Calculates the absolute angle, in degrees, between two vectors. The result is in the range [0, 180]
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The angle between both vectors</returns>
        public static Angle AbsoluteAngle(in Vector2 a, in Vector2 b) => Vectors.Angle.FromRadians(Math.Acos(Dot(a, b) / (a.Length * b.Length)));

        /// <summary>
        /// Calculates the dot product of both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The dot product of both vectors</returns>
        public static int Dot(in Vector2 a, in Vector2 b) => a.X * b.X + a.Y * b.Y;

        /// <summary>
        /// Rotates a vector by a specified angle, must be a multiple of 90 degrees
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <returns>The rotated vector</returns>
        /// <exception cref="InvalidOperationException">If the angle is not a multiple of 90 degrees</exception>
        public static Vector2 Rotate(in Vector2 vector, int angle)
        {
            if (angle % 90 is not 0) throw new InvalidOperationException($"Can only rotate integer vectors by 90 degrees, got {angle} instead");
            angle = ((angle % 360) + 360) % 360;
            return angle switch
            {
                90  => new Vector2(-vector.Y, vector.X),
                180 => -vector,
                270 => new Vector2(vector.Y, -vector.X),
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
        public static Vector2 ParseFromDirection(string value)
        {
            GroupCollection groups = directionMatch.Match(value).Groups;
            //Parse direction first
            Vector2 direction = groups[1].Value switch
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
        public static bool TryParseFromDirection(string value, out Vector2 direction)
        {
            //Check if it matches at all
            direction = Zero;
            Match match = directionMatch.Match(value);
            if (!match.Success) return false;

            
            GroupCollection groups = match.Groups;
            if (groups.Count is not 3) return false;
            if (!int.TryParse(groups[2].Value, out int distance)) return false;
            Vector2 dir;
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
        /// Cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="Vector2"/>
        /// </summary>
        /// <param name="tuple">Tuple to cast from</param>
        public static implicit operator Vector2((int x, int y) tuple) => new(tuple);

        /// <summary>
        /// Casts from <see cref="Vector2"/> to <see cref="ValueTuple{T1, T2}"/>
        /// </summary>
        /// <param name="vector">Vector to cast from</param>
        public static implicit operator (int x, int y)(in Vector2 vector) => (vector.X, vector.Y);

        /// <summary>
        /// Casts from <see cref="Vector2d"/> to <see cref="Vector2"/>
        /// </summary>
        /// <param name="vector">Vector to cast from</param>
        public static explicit operator Vector2(Vector2d vector) => new((int)vector.X, (int)vector.Y);

        /// <summary>
        /// Equality between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if both vectors are equal, false otherwise</returns>
        public static bool operator ==(in Vector2 a, in Vector2 b) => a.Equals(b);
        
        /// <summary>
        /// Inequality between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if both vectors are unequal, false otherwise</returns>
        public static bool operator !=(in Vector2 a, in Vector2 b) => !a.Equals(b);

        /// <summary>
        /// Less-than between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
        public static bool operator <(in Vector2 a, in Vector2 b) => a.CompareTo(b) < 0;

        /// <summary>
        /// Greater-than between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
        public static bool operator >(in Vector2 a, in Vector2 b) => a.CompareTo(b) > 0;
        
        /// <summary>
        /// Less-than-or-equals between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
        public static bool operator <=(in Vector2 a, in Vector2 b) => a.CompareTo(b) <= 0;

        /// <summary>
        /// Greater-than-or-equals between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
        public static bool operator >=(in Vector2 a, in Vector2 b) => a.CompareTo(b) >= 0;

        /// <summary>
        /// Negate operation on a vector
        /// </summary>
        /// <param name="a">Vector to negate</param>
        /// <returns>The vector with all it's components negated</returns>
        public static Vector2 operator -(in Vector2 a) => new(-a.X, -a.Y);

        /// <summary>
        /// Add operation between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>The result of the component-wise addition on both Vectors</returns>
        public static Vector2 operator +(in Vector2 a, in Vector2 b) => new(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// Subtract operation between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>The result of the component-wise subtraction on both Vectors</returns>
        public static Vector2 operator -(in Vector2 a, in Vector2 b) => new(a.X - b.X, a.Y - b.Y);

        /// <summary>
        /// Scalar integer multiplication on a Vector
        /// </summary>
        /// <param name="a">Vector to scale</param>
        /// <param name="b">Scalar to multiply by</param>
        /// <returns>The scaled vector</returns>
        public static Vector2 operator *(in Vector2 a, int b) => new(a.X * b, a.Y * b);

        /// <summary>
        /// Scalar integer division on a Vector
        /// </summary>
        /// <param name="a">Vector to scale</param>
        /// <param name="b">Scalar to divide by</param>
        /// <returns>The scaled vector</returns>
        public static Vector2 operator /(in Vector2 a, int b) => new(a.X / b, a.Y / b);

        /// <summary>
        /// Modulo scalar operation on a Vector
        /// </summary>
        /// <param name="a">Vector to use the Modulo onto</param>
        /// <param name="b">Scalar to modulo by</param>
        /// <returns>The vector with the results of the modulo operation component wise</returns>
        public static Vector2 operator %(in Vector2 a, int b) => new(a.X % b, a.Y % b);
        #endregion
    }
}