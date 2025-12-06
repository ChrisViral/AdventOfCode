using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors;

public readonly partial struct Vector2<T>
{
    /// <summary>
    /// Two dimensional vector space enumerator
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public ref struct SpaceEnumerator(T maxX, T maxY)
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;

        private T x = -T.One;
        private T y = T.Zero;

        /// <summary>
        /// Current enumerator value
        /// </summary>
        public readonly Vector2<T> Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(this.x, this.y);
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
                this.y++;
            }

            return this.y < this.maxY;
        }

        public SpaceEnumerator GetEnumerator() => this;
    }

    /// <summary>
    /// Two dimensional vector space enumerable
    /// </summary>
    /// <param name="maxX">Max space X value (exclusive)</param>
    /// <param name="maxY">Max space Y value (exclusive)</param>
    public class SpaceEnumerable(T maxX, T maxY) : IEnumerable<Vector2<T>>, IEnumerator<Vector2<T>>
    {
        private readonly T maxX = maxX;
        private readonly T maxY = maxY;

        private T x = -T.One;
        private T y = T.Zero;

        /// <inheritdoc />
        public Vector2<T> Current => new(this.x, this.y);

        /// <inheritdoc />
        object? IEnumerator.Current => this.Current;

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
        public void Reset()
        {
            this.x = -T.One;
            this.y = T.Zero;
        }

        /// <inheritdoc />
        public void Dispose() { }

        /// <inheritdoc />
        public IEnumerator<Vector2<T>> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}
