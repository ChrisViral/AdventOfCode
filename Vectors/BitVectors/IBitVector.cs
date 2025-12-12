using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors.BitVectors;

/// <summary>
/// BitVector interface
/// </summary>
[PublicAPI]
public interface IBitVector
{
    /// <summary>
    /// Bit width of the vector
    /// </summary>
    static abstract int Size { get; }

    /// <summary>
    /// Gets/sets the bit state at the given index
    /// </summary>
    /// <param name="index">Bit index to get/set</param>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="index"/> is less than 0 or greater or equal to <see cref="Size"/></exception>
    /// <returns><see langword="true"/> when the bit at <paramref name="index"/> is set, otherwise <see langword="false"/></returns>
    bool this[int index] { get; set; }

    /// <inheritdoc cref="Item(int)"/>
    bool this[Index index] { get; set; }

    /// <summary>
    /// Inverts the bit at the given index
    /// </summary>
    /// <param name="index">Index to invert the bit at</param>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="index"/> is less than 0 or greater or equal to <see cref="Size"/></exception>
    void InvertBit(int index);

    /// <inheritdoc cref="InvertBit(int)"/>
    void InvertBit(Index index);
}

/// <summary>
/// BitVector interface
/// </summary>
/// <typeparam name="TData">Vector internal data</typeparam>
/// <typeparam name="TSelf">Vector self type</typeparam>
[PublicAPI]
public interface IBitVector<TData, TSelf> : IBitVector, IEquatable<TSelf>
    where TData : IBinaryInteger<TData>, IUnsignedNumber<TData>
    where TSelf : struct, IBitVector<TData, TSelf>
{
    /// <summary>
    /// Gets a masked version of the BitVector in the given range
    /// </summary>
    /// <param name="range">Range to mask</param>
    TSelf this[Range range] { get; }

    /// <summary>
    /// Vector data
    /// </summary>
    TData Data { get; set; }

    /// <summary>
    /// Creates a new BitVector from a given list of bits
    /// </summary>
    /// <param name="bits">Bits to create the vector from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">When the size of <paramref name="bits"/> is greater than <see cref="IBitVector.Size"/></exception>
    /// <returns>The created BitVector from the specified bits</returns>
    static abstract TSelf FromBitArray(ReadOnlySpan<bool> bits);

    /// <summary>
    /// Creates a new BitVector from a given list of bits
    /// </summary>
    /// <param name="bits">Bits to create the vector from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">When the size of <paramref name="bits"/> is greater than <see cref="IBitVector.Size"/></exception>
    /// <returns>The created BitVector from the specified bits</returns>
    static virtual TSelf FromBitArray(IReadOnlyList<bool> bits)
    {
        if (bits.Count > TSelf.Size) throw new ArgumentException($"{nameof(BitVector32)} only supports up to {TSelf.Size} bits", nameof(bits));

        // Mask out data
        TData data = TData.Zero;
        for (int i = bits.Count - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= TData.One;
            }
        }

        // Return result
        return data;
    }

    /// <summary>
    /// Bitwise or operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise or of both vectors</returns>
    static abstract TSelf operator |(TSelf a, TSelf b);

    /// <summary>
    /// Bitwise and operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise and of both vectors</returns>
    static abstract TSelf operator &(TSelf a, TSelf b);

    /// <summary>
    /// Bitwise xor operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise xor of both vectors</returns>
    static abstract TSelf operator ^(TSelf a, TSelf b);

    /// <summary>
    /// Left shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the left-shifted data of this vector</returns>
    static abstract TSelf operator <<(TSelf vector, int shift);

    /// <summary>
    /// Right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the right-shifted data of this vector</returns>
    static abstract TSelf operator >>(TSelf vector, int shift);

    /// <summary>
    /// Unsigned right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the unsigned right-shifted data of this vector</returns>
    static abstract TSelf operator >>>(TSelf vector, int shift);

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors have identical data, otherwise <see langword="false"/></returns>
    static abstract bool operator ==(TSelf a, TSelf b);

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors do not have identical data, otherwise <see langword="false"/></returns>
    static abstract bool operator !=(TSelf a, TSelf b);

    /// <summary>
    /// Truthiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a nonzero value, otherwise <see langword="false"/></returns>
    static abstract bool operator true(TSelf vector);

    /// <summary>
    /// Falsiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a zero value, otherwise <see langword="false"/></returns>
    static abstract bool operator false(TSelf vector);

    /// <summary>
    /// Implicit cast from data type
    /// </summary>
    /// <param name="value">Data value</param>
    /// <returns>Creates a new vector with the specified data</returns>
    static abstract implicit operator TSelf(TData value);

    /// <summary>
    /// Explicit cast from to type
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns>Extracts the data from this vector</returns>
    static abstract explicit operator TData(TSelf vector);
}

/// <summary>
/// BitVector extension methods
/// </summary>
public static class BitVectorExtensions
{
    /// <summary>
    /// Extension methods
    /// </summary>
    /// <param name="vector">Vector instance</param>
    /// <typeparam name="TVector">Vector type</typeparam>
    extension<TVector>(TVector vector) where TVector : struct, IBitVector
    {
        /// <summary>
        /// Creates a bit string from the given bit vector
        /// </summary>
        /// <returns>A string of 0 and 1's representing the vector</returns>
        public string ToBitString()
        {
            Span<char> data = stackalloc char[TVector.Size];
            for (int i = 0; i < data.Length; i++)
            {
                data[^(i + 1)] = vector[i] ? '1' : '0';
            }
            return new string(data);
        }
    }
}
