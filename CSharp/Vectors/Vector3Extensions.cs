using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

/// <summary>
/// Vector3 enumerable
/// </summary>
[PublicAPI]
public static class Vector3Extensions
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    /// <param name="maxZ">Max space Z value (exclusive)</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator<T>(T maxX, T maxY, T maxZ) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;
        private readonly T maxZ = maxZ;

        private T x = -T.One;
        private T y = T.Zero;
        private T z = T.Zero;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public readonly Vector3<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y, this.z);
        }

        /// <summary>
        /// Move to the next enumerator value
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (++this.x == this.maxX)
            {
                this.x = T.Zero;
                if (++this.y == this.maxY)
                {
                    this.y = T.Zero;
                    this.z++;
                }
            }

            return this.z < this.maxZ;
        }

        /// <summary>
        /// Enumerator instance
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly SpaceEnumerator<T> GetEnumerator() => this;
    }

    /// <summary>
    /// Two dimensional vector space enumerable
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    /// <param name="maxZ">Max space Z value (exclusive)</param>
    public sealed class SpaceEnumerable<T>(T maxX, T maxY, T maxZ) : IEnumerable<Vector3<T>>, IEnumerator<Vector3<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;
        private readonly T maxZ = maxZ;

        private T x = -T.One;
        private T y = T.Zero;
        private T z = T.Zero;

        /// <inheritdoc />
        public Vector3<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y, this.z);
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
                if (++this.y == this.maxY)
                {
                    this.y = T.Zero;
                    this.z++;
                }
            }

            return this.z < this.maxZ;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            this.x = -T.One;
            this.y = T.Zero;
            this.z = T.Zero;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Vector3<T>> GetEnumerator() => this;

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
    public ref struct AdjacentEnumerator<T>(Vector3<T> vector, bool withDiagonals, bool withSelf) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
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
        private int index = -1;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public readonly Vector3<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.withSelf && index == this.offsets.Length ? this.vector : this.offsets[index];
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
            get => this.withSelf && index == this.offsets.Length ? this.vector : this.offsets[index];
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
        /// Gets all the adjacent Vector3 to this one
        /// </summary>
        /// <param name="withDiagonals">If diagonal vectors should be included</param>
        /// <param name="withSelf">If self vector should be included</param>
        /// <returns>Adjacent vectors</returns>
        /// ReSharper disable once CognitiveComplexity
        public AdjacentEnumerator<T> Adjacent(bool withDiagonals = false, bool withSelf = false) => new(value, withDiagonals, withSelf);

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
        public SpaceEnumerator<T> Enumerate()
        {
            if (value.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.X), value.X, "X boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Y), value.Y, "Y boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Z), value.Z, "Z boundary value must be greater than zero");

            return new SpaceEnumerator<T>(value.X, value.Y, value.Y);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
        /// </summary>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="Vector3{T}.X"/>, <see cref="Vector3{T}.Y"/>, or <see cref="Vector3{T}.Z"/> are smaller or equal to zero</exception>
        public SpaceEnumerable<T> AsEnumerable()
        {
            if (value.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.X), value.X, "X boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Y), value.Y, "Y boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Z), value.Z, "Z boundary value must be greater than zero");

            return new SpaceEnumerable<T>(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
        /// </summary>
        /// <param name="maxX">Max value for the x component, exclusive</param>
        /// <param name="maxY">Max value for the y component, exclusive</param>
        /// <param name="maxZ">Max value for the z component, exclusive</param>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
        public static SpaceEnumerator<T> EnumerateOver(T maxX, T maxY, T maxZ)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");

            return new SpaceEnumerator<T>(maxX, maxY, maxZ);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
        /// </summary>
        /// <param name="maxX">Max value for the x component, exclusive</param>
        /// <param name="maxY">Max value for the y component, exclusive</param>
        /// <param name="maxZ">Max value for the z component, exclusive</param>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, or <paramref name="maxZ"/> are smaller or equal to zero</exception>
        public static SpaceEnumerable<T> MakeEnumerable(T maxX, T maxY, T maxZ)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");

            return new SpaceEnumerable<T>(maxX, maxY, maxZ);
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
