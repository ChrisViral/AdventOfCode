using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Grids.Vectors
{
    /// <summary>
    /// Integer two component vector
    /// </summary>
    public readonly struct Vector2 : IComparable, IComparable<Vector2>, IEquatable<Vector2>
    {
        #region Constants
        private const double RAD_TO_DEG = 180d / Math.PI;
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
        /// <summary>
        /// Checks for equality with the given object
        /// </summary>
        /// <param name="other">Other object to test</param>
        /// <returns>True if the object is a Vector2 with the same components, false otherwise</returns>
        public override bool Equals(object? other) => other is Vector2 vector && Equals(vector);

        /// <summary>
        /// Checks with equality with another Vector
        /// </summary>
        /// <param name="other">Other Vector to check</param>
        /// <returns>True if both vectors have the same components, false otherwise</returns>
        public bool Equals(in Vector2 other) => this.X == other.X && this.Y == other.Y;

        /// <summary>
        /// Calculates the hash of this vector
        /// </summary>
        /// <returns>HashCode of the Vector, calculated from both components</returns>
        public override int GetHashCode() => HashCode.Combine(this.X, this.Y);

        /// <summary>
        /// Compares this vector to another object
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>0 if the other object is not a vector, otherwise the result of <see cref="CompareTo(in Vector2)"/></returns>
        public int CompareTo(object? other) => other is Vector2 vector ? CompareTo(vector) : 0;

        /// <summary>
        /// Compares this vector to another vector. Comparison is done on the X component first, then Y if necessary.
        /// </summary>
        /// <param name="other">Vector to compare to</param>
        /// <returns>0 if both vectors are equal, 1 if this vector is greater, -1 otherwise</returns>
        public int CompareTo(in Vector2 other)
        {
            int comp = this.X.CompareTo(other.X);
            return comp is 0 ? this.Y.CompareTo(other.X) : comp;
        }

        /// <summary>
        /// Converts this vector to a readable string
        /// </summary>
        /// <returns>String representation of this vector</returns>
        public override string ToString() => $"({this.X}, {this.Y})";

        /// <summary>
        /// Creates a new vector resulting in the moving of this vector in the specified direction
        /// </summary>
        /// <param name="directions">Direction to move in</param>
        /// <returns>The new, moved vector</returns>
        public Vector2 Move(Directions directions) => this + directions.ToVector();
        
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
        /// Calculates the angle, in degrees, between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The angle between both vectors</returns>
        public static double Angle(in Vector2 a, in Vector2 b) => Math.Acos(Dot(a, b) / (a.Length * b.Length)) * RAD_TO_DEG;

        /// <summary>
        /// Calculates the dot product of both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The dot product of both vectors</returns>
        public static int Dot(in Vector2 a, in Vector2 b) => a.X * b.X + a.Y * b.Y;
        
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