using System.Numerics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ZLinq;

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
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator<T>: IValueEnumerator<Vector4<T>>
        where T: unmanaged, IBinaryInteger<T>, IMinMaxValue<T>
    {
        private readonly T maxX;
        private readonly T maxY;
        private readonly T maxZ;
        private readonly T maxW;

        private T x = T.Zero;
        private T y = T.Zero;
        private T z = T.Zero;
        private T w = T.Zero;

        /// <summary>
        /// Two dimensional vector space enumerator
        /// </summary>
        /// <param name="maxX">Max space X value (exclusive)</param>
        /// <param name="maxY">Max space Y value (exclusive)</param>
        /// <param name="maxZ">Max space Z value (exclusive)</param>
        /// <param name="maxW">Max space W value (exclusive)</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="maxX"/>, <paramref name="maxY"/>, <paramref name="maxZ"/>, or <paramref name="maxW"/> are smaller or equal to zero</exception>
        public SpaceEnumerator(T maxX, T maxY, T maxZ, T maxW)
        {
            if (maxX <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxX), maxX, "X boundary value must be greater than zero");
            if (maxY <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxY), maxY, "Y boundary value must be greater than zero");
            if (maxZ <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxZ), maxZ, "Z boundary value must be greater than zero");
            if (maxW <= T.Zero) throw new ArgumentOutOfRangeException(nameof(maxW), maxW, "W boundary value must be greater than zero");

            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
            this.maxW = maxW;
        }

        /// <inheritdoc />
        public bool TryGetNext(out Vector4<T> current)
        {
            if (this.w == this.maxW)
            {
                current = default;
                return false;
            }

            current = new Vector4<T>(this.x, this.y, this.z, this.w);
            if (++this.x == this.maxX)
            {
                this.x = T.Zero;
                if (++this.y == this.maxY)
                {
                    this.y = T.Zero;
                    if (++this.z == this.maxZ)
                    {
                        this.z = T.Zero;
                        this.w++;
                    }
                }
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = int.CreateChecked(this.maxX * this.maxY * this.maxZ * this.maxW);
            return true;
        }

        /// <inheritdoc />
        public bool TryGetSpan(out ReadOnlySpan<Vector4<T>> span)
        {
            span = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryCopyTo(scoped Span<Vector4<T>> destination, Index offset) => false;

        /// <inheritdoc />
        public void Dispose() { }
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
        public ValueEnumerable<SpaceEnumerator<T>, Vector4<T>> Enumerate()
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector4<T>>(new SpaceEnumerator<T>(value.X, value.Y, value.Y, value.W));
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
        public static ValueEnumerable<SpaceEnumerator<T>, Vector4<T>> EnumerateOver(T maxX, T maxY, T maxZ, T maxW)
        {
            return new ValueEnumerable<SpaceEnumerator<T>, Vector4<T>>(new SpaceEnumerator<T>(maxX, maxY, maxZ, maxW));
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
