using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors;

/// <summary>
/// Vector4 extensions
/// </summary>
[PublicAPI]
public static class Vector4Extensions
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    /// <param name="maxZ">Max space Z value (exclusive)</param>
    /// <param name="maxW">Max space W value (exclusive)</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator<T>(T maxX, T maxY, T maxZ, T maxW) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;
        private readonly T maxZ = maxZ;
        private readonly T maxW = maxW;

        private T x = -T.One;
        private T y = T.Zero;
        private T z = T.Zero;
        private T w = T.Zero;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public readonly Vector4<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y, this.z, this.w);
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
                    if (++this.z == this.maxZ)
                    {
                        this.z = T.Zero;
                        w++;
                    }
                }
            }

            return this.w < this.maxW;
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
    /// <param name="maxW">Max space W value (exclusive)</param>
    public sealed class SpaceEnumerable<T>(T maxX, T maxY, T maxZ, T maxW) : IEnumerable<Vector4<T>>, IEnumerator<Vector4<T>>
        where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;
        private readonly T maxZ = maxZ;
        private readonly T maxW = maxW;

        private T x = -T.One;
        private T y = T.Zero;
        private T z = T.Zero;
        private T w = T.Zero;

        /// <inheritdoc />
        public Vector4<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y, this.z, this.w);
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
                    if (++this.z == this.maxZ)
                    {
                        this.z = T.Zero;
                        w++;
                    }
                }
            }

            return this.w < this.maxW;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            this.x = -T.One;
            this.y = T.Zero;
            this.z = T.Zero;
            this.w = T.Zero;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IDisposable.Dispose() { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<Vector4<T>> GetEnumerator() => this;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() => this;
    }

    extension<T>(in Vector4<T> value) where T : unmanaged, IBinaryNumber<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Returns a span of the values of the vector
        /// </summary>
        /// <returns>Span over the values of the vector</returns>
        public ReadOnlySpan<T> AsSpan() => value.data;
    }

    extension<T>(Vector4<T> value) where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates an irreducible version of this vector<br/>
        /// </summary>
        /// <returns>The fully reduced version of this vector</returns>
        public Vector4<T> Reduced
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / MathUtils.GCD(MathUtils.GCD(MathUtils.GCD(value.X, value.Y), value.Z), value.W);
        }

        /// <summary>
        /// Absolute length of both vector components summed
        /// </summary>
        public T ManhattanLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => T.Abs(value.X) + T.Abs(value.Y) + T.Abs(value.Z) + T.Abs(value.W);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
        /// </summary>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="Vector4{T}.X"/>, <see cref="Vector4{T}.Y"/>, <see cref="Vector4{T}.Z"/>, or <see cref="Vector4{T}.W"/> are smaller or equal to zero</exception>
        public SpaceEnumerator<T> Enumerate()
        {
            if (value.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.X), value.X, "X boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Y), value.Y, "Y boundary value must be greater than zero");
            if (value.Z <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Z), value.Z, "Z boundary value must be greater than zero");
            if (value.W <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.W), value.W, "W boundary value must be greater than zero");

            return new SpaceEnumerator<T>(value.X, value.Y, value.Y, value.W);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension, using this vector's values as the maximums
        /// </summary>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <see cref="Vector4{T}.X"/>, <see cref="Vector4{T}.Y"/>, <see cref="Vector4{T}.Z"/>,  or <see cref="Vector4{T}.W"/> are smaller or equal to zero</exception>
        public SpaceEnumerable<T> AsEnumerable()
        {
            if (value.X <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.X), value.X, "X boundary value must be greater than zero");
            if (value.Y <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Y), value.Y, "Y boundary value must be greater than zero");
            if (value.Z <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.Z), value.Z, "Z boundary value must be greater than zero");
            if (value.W <= T.Zero) throw new ArgumentOutOfRangeException(nameof(value.W), value.W, "W boundary value must be greater than zero");

            return new SpaceEnumerable<T>(value.X, value.Y, value.Z, value.W);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
        /// </summary>
        /// <param name="maxX">Max value for the x component, exclusive</param>
        /// <param name="maxY">Max value for the y component, exclusive</param>
        /// <param name="maxZ">Max value for the z component, exclusive</param>
        /// <param name="maxW">Max value for the w component, exclusive</param>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, <paramref name="maxZ"/>, or <paramref name="maxW"/> are smaller or equal to zero</exception>
        public static SpaceEnumerator<T> EnumerateOver(T maxX, T maxY, T maxZ, T maxW)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");
            if (maxW <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxW), maxW, "W boundary value must be greater than zero");

            return new SpaceEnumerator<T>(maxX, maxY, maxZ, maxW);
        }

        /// <summary>
        /// Enumerates in row order all the vectors which have components in the range [0,max[ for each dimension
        /// </summary>
        /// <param name="maxX">Max value for the x component, exclusive</param>
        /// <param name="maxY">Max value for the y component, exclusive</param>
        /// <param name="maxZ">Max value for the z component, exclusive</param>
        /// <param name="maxW">Max value for the w component, exclusive</param>
        /// <returns>An enumerator of all the vectors in the given range</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, <paramref name="maxZ"/>, or <paramref name="maxW"/> are smaller or equal to zero</exception>
        public static SpaceEnumerable<T> MakeEnumerable(T maxX, T maxY, T maxZ, T maxW)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");
            if (maxW <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxW), maxW, "W boundary value must be greater than zero");

            return new SpaceEnumerable<T>(maxX, maxY, maxZ, maxW);
        }

        /// <summary>
        /// Gets all the adjacent Vector4 to this one
        /// </summary>
        /// <param name="withDiagonals">If diagonal vectors should be included</param>
        /// <param name="withSelf">If self vector should be included</param>
        /// <returns>Adjacent vectors</returns>
        /// ReSharper disable once CognitiveComplexity
        public IEnumerable<Vector4<T>> Adjacent(bool withDiagonals = false, bool withSelf = false)
        {
            if (withDiagonals)
            {
                for (T x = -T.One; x <= T.One; x++)
                {
                    for (T y = -T.One; y <= T.One; y++)
                    {
                        for (T z = -T.One; z <= T.One; z++)
                        {
                            for (T w = -T.One; w <= T.One; w++)
                            {
                                if (!withSelf && x == T.Zero && y == T.Zero && z == T.Zero && w == T.Zero) continue;

                                yield return new Vector4<T>(value.X + x, value.Y + y, value.Z + z, value.W + w);
                            }
                        }
                    }
                }
            }
            else
            {
                if (withSelf) yield return value;
                yield return value + Vector4<T>.Up;
                yield return value + Vector4<T>.Down;
                yield return value + Vector4<T>.Left;
                yield return value + Vector4<T>.Right;
                yield return value + Vector4<T>.Forwards;
                yield return value + Vector4<T>.Backwards;
                yield return value + Vector4<T>.Inwards;
                yield return value + Vector4<T>.Outwards;
            }
        }

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ManhattanDistance(Vector4<T> a, Vector4<T> b) => T.Abs(a.X - b.X) + T.Abs(a.Y - b.Y) + T.Abs(a.Z - b.Z) + T.Abs(a.W - b.W);

        /// <summary>
        /// The Manhattan distance between both vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Tge straight line distance between both vectors</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResult ManhattanDistance<TResult>(Vector4<T> a, Vector4<T> b) where TResult : unmanaged, IBinaryInteger<TResult>, IMinMaxValue<TResult>
        {
            return Vector4<TResult>.ManhattanDistance(Vector4<TResult>.CreateChecked(a), Vector4<TResult>.CreateChecked(b));
        }
    }

    extension<T>(Vector4<T> value) where T : unmanaged, IBinaryFloatingPointIeee754<T>, IMinMaxValue<T>
    {
        /// <summary>
        /// Creates a normalized version of this vector<br/>
        /// </summary>
        /// <returns>The vector normalized</returns>
        public Vector4<T> Normalized
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value / T.CreateChecked(value.Length);
        }
    }
}
