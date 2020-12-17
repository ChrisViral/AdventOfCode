using System;

namespace AdventOfCode.Grids.Vectors
{
    /// <summary>
    /// Integer three component vector
    /// </summary>
    public readonly struct Vector3 : IComparable, IComparable<Vector3>, IEquatable<Vector3>, IFormattable
    {
        #region Constants
        /// <summary>Zero vector</summary>
        public static readonly Vector3 Zero  = new(0, 0, 0);
        /// <summary>One vector</summary>
        public static readonly Vector3 One   = new(1, 1, 1);
        /// <summary>Up vector</summary>
        public static readonly Vector3 Up    = new(0, 1, 0);
        /// <summary>Down vector</summary>
        public static readonly Vector3 Down  = new(0, -1, 0);
        /// <summary>Left vector</summary>
        public static readonly Vector3 Left  = new(-1, 0, 0);
        /// <summary>Right vector</summary>
        public static readonly Vector3 Right = new(1, 0, 0);
        /// <summary>Forward vector</summary>
        public static readonly Vector3 Forwards = new(0, 0, 1);
        /// <summary>Backward vector</summary>
        public static readonly Vector3 Backwards = new(0, 0, -1);
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
        /// X component of the Vector
        /// </summary>
        public int Z { get; }

        /// <summary>
        /// Length of the Vector
        /// </summary>
        public double Length { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Vector3"/> with the specified components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Length = Math.Sqrt(x * x + y * y + z * z);
        }
        
        /// <summary>
        /// Creates a new <see cref="Vector3"/> from a given three component tuple
        /// </summary>
        /// <param name="tuple">Tuple to create the Vector from</param>
        public Vector3((int x, int y, int z) tuple) : this(tuple.x, tuple.y, tuple.z) { }

        /// <summary>
        /// Vector copy constructor
        /// </summary>
        /// <param name="copy">Vector to copy</param>
        public Vector3(in Vector3 copy)
        {
            this.X = copy.X;
            this.Y = copy.Y;
            this.Z = copy.Z;
            this.Length = copy.Length;
        }
        #endregion
        
        #region Methods
        /// <inheritdoc cref="object.Equals(object)"/>
        public override bool Equals(object? other) => other is Vector3 vector && Equals(vector);

        /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
        public bool Equals(in Vector3 other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;

        /// <inheritdoc cref="object.GetHashCode"/>
        public override int GetHashCode() => HashCode.Combine(this.X, this.Y, this.Z);

        /// <inheritdoc cref="IComparable.CompareTo"/>
        /// ReSharper disable once TailRecursiveCall - not tail recursive
        public int CompareTo(object? other) => other is Vector3 vector ? CompareTo(vector) : 0;

        /// <inheritdoc cref="IComparable{T}.CompareTo"/>
        public int CompareTo(in Vector3 other) => this.Length.CompareTo(other.Length);

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
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = this.X;
            y = this.Y;
            z = this.Z;
        }

        /// <inheritdoc cref="IEquatable{T}"/>
        bool IEquatable<Vector3>.Equals(Vector3 other) => Equals(other);
        
        /// <inheritdoc cref="IComparable{T}"/>
        int IComparable<Vector3>.CompareTo(Vector3 other) => CompareTo(other);
        #endregion
        
        #region Static methods
        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The distance between both vectors</returns>
        public static double Distance(in Vector3 a, in Vector3 b) => (a - b).Length;

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        public static int ManhattanDistance(in Vector3 a, in Vector3 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);

        /// <summary>
        /// Calculates the dot product of both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>The dot product of both vectors</returns>
        public static int Dot(in Vector3 a, in Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>
        /// Gives the absolute value of a given vector
        /// </summary>
        /// <param name="vector">Vector to get the absolute value of</param>
        /// <returns>Absolute value of the vector</returns>
        public static Vector3 Abs(in Vector3 vector) => new(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        #endregion

        #region Operators
        /// <summary>
        /// Cast from <see cref="ValueTuple{T1, T2, T3}"/> to <see cref="Vector3"/>
        /// </summary>
        /// <param name="tuple">Tuple to cast from</param>
        public static implicit operator Vector3((int x, int y, int z) tuple) => new(tuple);

        /// <summary>
        /// Casts from <see cref="Vector3"/> to <see cref="ValueTuple{T1, T2, T3}"/>
        /// </summary>
        /// <param name="vector">Vector to cast from</param>
        public static implicit operator (int x, int y, int z)(in Vector3 vector) => (vector.X, vector.Y, vector.Z);

        /// <summary>
        /// Casts from <see cref="Vector3"/> to <see cref="Vector3"/>
        /// </summary>
        /// <param name="vector">Vector to cast from</param>
        public static implicit operator Vector3(in Vector2 vector) => new(vector.X, vector.Y, 0);

        /// <summary>
        /// Equality between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if both vectors are equal, false otherwise</returns>
        public static bool operator ==(in Vector3 a, in Vector3 b) => a.Equals(b);
        
        /// <summary>
        /// Inequality between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if both vectors are unequal, false otherwise</returns>
        public static bool operator !=(in Vector3 a, in Vector3 b) => !a.Equals(b);

        /// <summary>
        /// Less-than between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is less than the second vector, false otherwise</returns>
        public static bool operator <(in Vector3 a, in Vector3 b) => a.CompareTo(b) < 0;

        /// <summary>
        /// Greater-than between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is greater than the second vector, false otherwise</returns>
        public static bool operator >(in Vector3 a, in Vector3 b) => a.CompareTo(b) > 0;
        
        /// <summary>
        /// Less-than-or-equals between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is less than or equal to the second vector, false otherwise</returns>
        public static bool operator <=(in Vector3 a, in Vector3 b) => a.CompareTo(b) <= 0;

        /// <summary>
        /// Greater-than-or-equals between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>True if the first vector is greater than or equal to the second vector, false otherwise</returns>
        public static bool operator >=(in Vector3 a, in Vector3 b) => a.CompareTo(b) >= 0;

        /// <summary>
        /// Negate operation on a vector
        /// </summary>
        /// <param name="a">Vector to negate</param>
        /// <returns>The vector with all it's components negated</returns>
        public static Vector3 operator -(in Vector3 a) => new(-a.X, -a.Y, -a.Z);

        /// <summary>
        /// Add operation between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>The result of the component-wise addition on both Vectors</returns>
        public static Vector3 operator +(in Vector3 a, in Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        /// <summary>
        /// Subtract operation between two vectors
        /// </summary>
        /// <param name="a">First Vector</param>
        /// <param name="b">Second Vector</param>
        /// <returns>The result of the component-wise subtraction on both Vectors</returns>
        public static Vector3 operator -(in Vector3 a, in Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        /// <summary>
        /// Scalar integer multiplication on a Vector
        /// </summary>
        /// <param name="a">Vector to scale</param>
        /// <param name="b">Scalar to multiply by</param>
        /// <returns>The scaled vector</returns>
        public static Vector3 operator *(in Vector3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);

        /// <summary>
        /// Scalar integer division on a Vector
        /// </summary>
        /// <param name="a">Vector to scale</param>
        /// <param name="b">Scalar to divide by</param>
        /// <returns>The scaled vector</returns>
        public static Vector3 operator /(in Vector3 a, int b) => new(a.X / b, a.Y / b, a.Z / b);

        /// <summary>
        /// Modulo scalar operation on a Vector
        /// </summary>
        /// <param name="a">Vector to use the Modulo onto</param>
        /// <param name="b">Scalar to modulo by</param>
        /// <returns>The vector with the results of the modulo operation component wise</returns>
        public static Vector3 operator %(in Vector3 a, int b) => new(a.X % b, a.Y % b, a.Z % b);
        #endregion
    }
}