using System.Collections;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils.Extensions.Numbers;
using JetBrains.Annotations;
using ZLinq;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Vector2 Extensions
/// </summary>
[PublicAPI]
public static class Vector2Extensions
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator<T> : IValueEnumerator<Vector2<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX;
        private readonly T maxY;

        private T x = T.Zero;
        private T y = T.Zero;

        /// <summary>
        /// Two dimensional vector space enumerator
        /// </summary>
        /// <param name="maxX">Max space X value (exclusive)</param>
        /// <param name="maxY">Max space Y value (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/> or <paramref name="maxY"/> are smaller or equal to zero</exception>
        public SpaceEnumerator(T maxX, T maxY)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");

            this.maxX = maxX;
            this.maxY = maxY;
        }

        /// <inheritdoc />
        public bool TryGetNext(out Vector2<T> current)
        {
            if (this.y == this.maxY)
            {
                current = default;
                return false;
            }

            current = new Vector2<T>(this.x, this.y);
            if (++this.x == this.maxX)
            {
                this.x = T.Zero;
                this.y++;
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = int.CreateChecked(this.maxX * this.maxY);
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSpan(out ReadOnlySpan<Vector2<T>> span)
        {
            span = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryCopyTo(scoped Span<Vector2<T>> destination, Index offset) => false;

        /// <inheritdoc />
        public void Dispose() { }
    }

    /// <summary>
    /// Two dimensional vector space enumerable
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    public sealed class SpaceEnumerable<T>(T maxX, T maxY) : IEnumerable<Vector2<T>>, IEnumerator<Vector2<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;

        private T x = -T.One;
        private T y = T.Zero;

        /// <inheritdoc />
        public Vector2<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y);
        }

        /// <inheritdoc />
        object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Current;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            this.x = -T.One;
            this.y = T.Zero;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Vector2<T>> GetEnumerator() => this;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => this;
    }

    /// <summary>
    /// Adjacent vector enumerator
    /// </summary>
    /// <param name="vector">Vector to get the adjacent positions for</param>
    /// <param name="withDiagonals">If diagonal adjacents should be included</param>
    /// <param name="withSelf">If the self vector should be included</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct AdjacentEnumerator<T>(Vector2<T> vector, bool withDiagonals, bool withSelf) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Orthogonal offsets
        /// </summary>
        public static readonly ImmutableArray<Vector2<T>> Offsets =
        [
            Vector2<T>.Up,
            Vector2<T>.Right,
            Vector2<T>.Down,
            Vector2<T>.Left
        ];

        /// <summary>
        /// Orthogonal and diagonal offsets
        /// </summary>
        public static readonly ImmutableArray<Vector2<T>> AllOffsets =
        [
            Vector2<T>.Up,
            Vector2<T>.Up + Vector2<T>.Left,
            Vector2<T>.Left,
            Vector2<T>.Down + Vector2<T>.Left,
            Vector2<T>.Down,
            Vector2<T>.Down + Vector2<T>.Right,
            Vector2<T>.Right,
            Vector2<T>.Up + Vector2<T>.Right
        ];

        private readonly bool withSelf = withSelf;
        private readonly Vector2<T> vector = vector;
        private readonly ReadOnlySpan<Vector2<T>> offsets = withDiagonals ? AllOffsets.AsSpan() : Offsets.AsSpan();
        private int index = -1;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public readonly Vector2<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.withSelf && this.index == this.offsets.Length ? this.vector : this.vector + this.offsets[this.index];
        }

        /// <summary>
        /// Move to the next enumerator value
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            this.index++;
            return this.withSelf ? this.index <= this.offsets.Length : this.index < this.offsets.Length;
        }

        /// <summary>
        /// Enumerator instance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly AdjacentEnumerator<T> GetEnumerator() => this;
    }

    /// <summary>
    /// Adjacent vector enumerator
    /// </summary>
    /// <param name="vector">Vector to get the adjacent positions for</param>
    /// <param name="withDiagonals">If diagonal adjacents should be included</param>
    /// <param name="withSelf">If the self vector should be included</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class AdjacentEnumerable<T>(Vector2<T> vector, bool withDiagonals, bool withSelf) : IEnumerable<Vector2<T>>, IEnumerator<Vector2<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly bool withSelf = withSelf;
        private readonly Vector2<T> vector = vector;
        private readonly ImmutableArray<Vector2<T>> offsets = withDiagonals ? AdjacentEnumerator<T>.AllOffsets : AdjacentEnumerator<T>.Offsets;
        private int index = -1;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public Vector2<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.withSelf && this.index == this.offsets.Length ? this.vector : this.vector + this.offsets[this.index];
        }

        /// <inheritdoc />
        object IEnumerator.Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Current;
        }

        /// <summary>
        /// Move to the next enumerator value
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            this.index++;
            return this.withSelf ? this.index <= this.offsets.Length : this.index < this.offsets.Length;
        }

        /// <summary>
        /// Enumerator instance
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Vector2<T>> GetEnumerator() => this;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => this;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
    }

    extension<T>(in Vector2<T> value) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Returns a span of the values of the vector
        /// </summary>
        /// <returns>Span over the values of the vector</returns>
        public ReadOnlySpan<T> AsSpan() => value.data;
    }

    /// <summary>
    /// Integer vector extensions
    /// </summary>
    /// <param name="value">Vector value</param>
    /// <typeparam name="T">Integer type</typeparam>
    extension<T>(Vector2<T> value) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates an irreducible version of this vector
        /// </summary>
        /// <returns>The fully reduced version of this vector</returns>
        public Vector2<T> Reduced
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / MathUtils.GCD(value.X, value.Y);
        }

        /// <summary>
        /// Absolute length of both vector components summed
        /// </summary>
        public T ManhattanLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => T.Abs(value.X) + T.Abs(value.Y);
        }

        /// <summary>
        /// Gets all the adjacent Vector2 to this one
        /// </summary>
        /// <param name="withDiagonals">If diagonal vectors should be included</param>
        /// <param name="withSelf">If self vector should be included</param>
        /// <returns>Adjacent vectors</returns>
        /// ReSharper disable once CognitiveComplexity
        public AdjacentEnumerator<T> Adjacent(bool withDiagonals = false, bool withSelf = false) => new(value, withDiagonals, withSelf);

        /// <summary>
        /// Gets all the adjacent Vector2 to this one
        /// </summary>
        /// <param name="withDiagonals">If diagonal vectors should be included</param>
        /// <param name="withSelf">If self vector should be included</param>
        /// <returns>Adjacent vectors</returns>
        /// ReSharper disable once CognitiveComplexity
        public AdjacentEnumerable<T> AsAdjacentEnumerable(bool withDiagonals = false, bool withSelf = false) => new(value, withDiagonals, withSelf);

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
        /// </summary>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="Vector2{T}.X"/> or <see cref="Vector2{T}.Y"/> are smaller or equal to zero</exception>
        public ValueEnumerable<SpaceEnumerator<T>, Vector2<T>> Enumerate()
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector2<T>>(new SpaceEnumerator<T>(value.X, value.Y));
        }

        /// <summary>
        /// Rotates a vector by a specified angle, must be a multiple of 90 degrees
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <returns>The rotated vector</returns>
        public static Vector2<T> Rotate(Vector2<T> vector, int angle)
        {
            if (!angle.IsMultiple(90)) throw new InvalidOperationException($"Can only rotate integer vectors by 90 degrees, got {angle} instead");

            angle = angle.Mod(360);
            return angle switch
            {
                90  => new Vector2<T>(-vector.Y, vector.X),
                180 => -vector,
                270 => new Vector2<T>(vector.Y, -vector.X),
                _   => vector
            };
        }

        /// <summary>
    /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
    /// </summary>
    /// <param name="maxX">Max value for the x component, exclusive</param>
    /// <param name="maxY">Max value for the y component, exclusive</param>
    /// <returns>An enumerator of all the vectors in the given range</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/> or <paramref name="maxY"/> are smaller or equal to zero</exception>
        public static ValueEnumerable<SpaceEnumerator<T>, Vector2<T>> EnumerateOver(T maxX, T maxY)
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector2<T>>(new SpaceEnumerator<T>(maxX, maxY));
        }

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ManhattanDistance(Vector2<T> a, Vector2<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y);

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult ManhattanDistance<TResult>(Vector2<T> a, Vector2<T> b) where TResult : unmanaged, IBinaryInteger<TResult>, IMinMaxValue<TResult>
        {
            return Vector2<TResult>.ManhattanDistance(Vector2<TResult>.CreateChecked(a), Vector2<TResult>.CreateChecked(b));
        }
    }

    /// <summary>
    /// Floating point vector extensions
    /// </summary>
    /// <param name="value">Vector value</param>
    /// <typeparam name="T">Floating point type</typeparam>
    extension<T>(Vector2<T> value) where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates a normalized version of this vector
        /// </summary>
        /// <returns>The vector normalized</returns>
        public Vector2<T> Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / T.CreateChecked(value.Length);
        }

        /// <summary>
        /// Rotates a vector by a specified angle
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <returns>The rotated vector</returns>
        public static Vector2<T> Rotate(Vector2<T> vector, double angle)
        {
            (double x, double y) = Vector2<double>.CreateChecked(vector);
            double radians = angle * Angle.DEG2RAD;
            Vector2<double> result = new(x * Math.Cos(radians) - y * Math.Sin(radians),
                                         x * Math.Sin(radians) + y * Math.Cos(radians));
            return Vector2<T>.CreateChecked(result);
        }

        /// <summary>
        /// Rotates a vector by a specified angle
        /// </summary>
        /// <param name="vector">Vector to rotate</param>
        /// <param name="angle">Angle to rotate by</param>
        /// <returns>The rotated vector</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2<T> Rotate(Vector2<T> vector, Angle angle) => Vector2<T>.Rotate(vector, angle.Radians);
    }
}
