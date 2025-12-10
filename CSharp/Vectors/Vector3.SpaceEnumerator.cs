using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

public readonly partial struct Vector3<T>
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    /// <param name="maxZ">Max space Z value (exclusive)</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator(T maxX, T maxY, T maxZ)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpaceEnumerator GetEnumerator() => this;
    }

    /// <summary>
    /// Two dimensional vector space enumerable
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    /// <param name="maxZ">Max space Z value (exclusive)</param>
    public class SpaceEnumerable(T maxX, T maxY, T maxZ) : IEnumerable<Vector3<T>>, IEnumerator<Vector3<T>>
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
}
