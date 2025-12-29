using System.Collections;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Internal;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Vector3 extensions
/// </summary>
[PublicAPI]
public static class Vector3Extensions
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator<T> : IValueEnumerator<Vector3<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX;
        private readonly T maxY;
        private readonly T maxZ;

        private T x = T.Zero;
        private T y = T.Zero;
        private T z = T.Zero;

        /// <summary>
        /// Two dimensional vector space enumerator
        /// </summary>
        /// <param name="maxX">Max space X value (exclusive)</param>
        /// <param name="maxY">Max space Y value (exclusive)</param>
        /// <param name="maxZ">Max space Z value (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
        public SpaceEnumerator(T maxX, T maxY, T maxZ)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");

            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
        }

        /// <inheritdoc />
        public bool TryGetNext(out Vector3<T> current)
        {
            if (this.z == this.maxZ)
            {
                current = default;
                return false;
            }

            current = new Vector3<T>(this.x, this.y, this.z);
            if (++this.x == this.maxX)
            {
                this.x = T.Zero;
                if (++this.y == this.maxY)
                {
                    this.y = T.Zero;
                    this.z++;
                }
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = int.CreateChecked(this.maxX * this.maxY * this.maxZ);
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSpan(out ReadOnlySpan<Vector3<T>> span)
        {
            span = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryCopyTo(scoped Span<Vector3<T>> destination, Index offset) => false;

        /// <inheritdoc />
        public void Dispose() { }
    }

    /// <summary>
    /// Adjacent vector enumerator
    /// </summary>
    /// <param name="vector">Vector to get the adjacent positions for</param>
    /// <param name="withDiagonals">If diagonal adjacents should be included</param>
    /// <param name="withSelf">If the self vector should be included</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct AdjacentEnumerator<T>(Vector3<T> vector, bool withDiagonals, bool withSelf) : IValueEnumerator<Vector3<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Orthogonal offsets
        /// </summary>
        public static readonly ImmutableArray<Vector3<T>> Offsets =
        [
            Vector3<T>.Up,
            Vector3<T>.Right,
            Vector3<T>.Down,
            Vector3<T>.Left,
            Vector3<T>.Backwards,
            Vector3<T>.Forwards
        ];

        /// <summary>
        /// Orthogonal and diagonal offsets
        /// </summary>
        public static readonly ImmutableArray<Vector3<T>> AllOffsets =
        [
            // Middle face
            Vector3<T>.Up,
            Vector3<T>.Up + Vector3<T>.Left,
            Vector3<T>.Left,
            Vector3<T>.Down + Vector3<T>.Left,
            Vector3<T>.Down,
            Vector3<T>.Down + Vector3<T>.Right,
            Vector3<T>.Right,
            Vector3<T>.Up + Vector3<T>.Right,

            // Back face
            Vector3<T>.Backwards,
            Vector3<T>.Backwards + Vector3<T>.Up,
            Vector3<T>.Backwards + Vector3<T>.Up + Vector3<T>.Left,
            Vector3<T>.Backwards + Vector3<T>.Left,
            Vector3<T>.Backwards + Vector3<T>.Down + Vector3<T>.Left,
            Vector3<T>.Backwards + Vector3<T>.Down,
            Vector3<T>.Backwards + Vector3<T>.Down + Vector3<T>.Right,
            Vector3<T>.Backwards + Vector3<T>.Right,
            Vector3<T>.Backwards + Vector3<T>.Up + Vector3<T>.Right,

            // Front face
            Vector3<T>.Forwards,
            Vector3<T>.Forwards + Vector3<T>.Up,
            Vector3<T>.Forwards + Vector3<T>.Up + Vector3<T>.Left,
            Vector3<T>.Forwards + Vector3<T>.Left,
            Vector3<T>.Forwards + Vector3<T>.Down + Vector3<T>.Left,
            Vector3<T>.Forwards + Vector3<T>.Down,
            Vector3<T>.Forwards + Vector3<T>.Down + Vector3<T>.Right,
            Vector3<T>.Forwards + Vector3<T>.Right,
            Vector3<T>.Forwards + Vector3<T>.Up + Vector3<T>.Right,
        ];

        private readonly bool withSelf = withSelf;
        private readonly Vector3<T> vector = vector;
        private readonly ReadOnlySpan<Vector3<T>> offsets = withDiagonals ? AllOffsets.AsSpan() : Offsets.AsSpan();
        private int index = 0;

        /// <inheritdoc />
        public bool TryGetNext(out Vector3<T> current)
        {
            if (this.index >= this.offsets.Length)
            {
                if (this.withSelf && this.index == this.offsets.Length)
                {
                    current = this.vector;
                    this.index++;
                    return true;
                }

                current = default;
                return false;
            }

            current = this.offsets[this.index++];
            return true;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = this.offsets.Length;
            if (this.withSelf) count++;
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSpan(out ReadOnlySpan<Vector3<T>> span)
        {
            if (this.withSelf)
            {
                span = default;
                return false;
            }

            span = this.offsets;
            return true;
        }

        /// <inheritdoc />
        public bool TryCopyTo(scoped Span<Vector3<T>> destination, Index offset)
        {
            if (!this.withSelf)
            {
                if (EnumeratorHelper.TryGetSlice(this.offsets, offset, destination.Length, out ReadOnlySpan<Vector3<T>> slice))
                {
                    slice.CopyTo(destination);
                    return true;
                }
                return false;
            }

            Span<Vector3<T>> temp = stackalloc Vector3<T>[this.offsets.Length + 1];
            this.offsets.CopyTo(temp);
            temp[^1] = this.vector;

            if (EnumeratorHelper.TryGetSlice(temp, offset, destination.Length, out ReadOnlySpan<Vector3<T>> tempSlice))
            {
                tempSlice.CopyTo(destination);
                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public void Dispose() { }
    }

    /// <summary>
    /// Adjacent vector enumerator
    /// </summary>
    /// <param name="vector">Vector to get the adjacent positions for</param>
    /// <param name="withDiagonals">If diagonal adjacents should be included</param>
    /// <param name="withSelf">If the self vector should be included</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class AdjacentEnumerable<T>(Vector3<T> vector, bool withDiagonals, bool withSelf) : IEnumerable<Vector3<T>>, IEnumerator<Vector3<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly bool withSelf = withSelf;
        private readonly Vector3<T> vector = vector;
        private readonly ImmutableArray<Vector3<T>> offsets = withDiagonals ? AdjacentEnumerator<T>.AllOffsets : AdjacentEnumerator<T>.Offsets;
        private int index = -1;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public Vector3<T> Current
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
        public IEnumerator<Vector3<T>> GetEnumerator() => this;

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

    extension<T>(in Vector3<T> value) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Returns a span of the values of the vector
        /// </summary>
        /// <returns>Span over the values of the vector</returns>
        public ReadOnlySpan<T> AsSpan() => value.data;
    }

    extension<T>(Vector3<T> value) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates an irreducible version of this vector<br/>
        /// </summary>
        /// <returns>The fully reduced version of this vector</returns>
        public Vector3<T> Reduced
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / MathUtils.GCD(MathUtils.GCD(value.X, value.Y), value.Z);
        }

        /// <summary>
        /// Absolute length of both vector components summed
        /// </summary>
        public T ManhattanLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => T.Abs(value.X) + T.Abs(value.Y) + T.Abs(value.Z);
        }

        /// <summary>
        /// Gets all the adjacent Vector3 to this one
        /// </summary>
        /// <param name="withDiagonals">If diagonal vectors should be included</param>
        /// <param name="withSelf">If self vector should be included</param>
        /// <returns>Adjacent vectors</returns>
        /// ReSharper disable once CognitiveComplexity
        public ValueEnumerable<AdjacentEnumerator<T>, Vector3<T>> Adjacent(bool withDiagonals = false, bool withSelf = false)
        {
            return new ValueEnumerable<AdjacentEnumerator<T>, Vector3<T>>(new AdjacentEnumerator<T>(value, withDiagonals, withSelf));
        }

        /// <summary>
        /// Gets all the adjacent Vector3 to this one
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
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="Vector3{T}.X"/>, <see cref="Vector3{T}.Y"/>, or <see cref="Vector3{T}.Z"/> are smaller or equal to zero</exception>
        public ValueEnumerable<SpaceEnumerator<T>, Vector3<T>> Enumerate()
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector3<T>>(new SpaceEnumerator<T>(value.X, value.Y, value.Y));
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
        /// </summary>
        /// <param name="maxX">Max value for the x component, exclusive</param>
        /// <param name="maxY">Max value for the y component, exclusive</param>
        /// <param name="maxZ">Max value for the z component, exclusive</param>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
        public static ValueEnumerable<SpaceEnumerator<T>, Vector3<T>> EnumerateOver(T maxX, T maxY, T maxZ)
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector3<T>>(new SpaceEnumerator<T>(maxX, maxY, maxZ));
        }

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ManhattanDistance(Vector3<T> a, Vector3<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y) + T.Abs(a.Z - b.Z);

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult ManhattanDistance<TResult>(Vector3<T> a, Vector3<T> b) where TResult : unmanaged, IBinaryInteger<TResult>, IMinMaxValue<TResult>
        {
            return Vector3<TResult>.ManhattanDistance(Vector3<TResult>.CreateChecked(a), Vector3<TResult>.CreateChecked(b));
        }
    }

    extension<T>(Vector3<T> value) where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates a normalized version of this vector<br/>
        /// </summary>
        /// <returns>The vector normalized</returns>
        public Vector3<T> Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / T.CreateChecked(value.Length);
        }
    }
}
