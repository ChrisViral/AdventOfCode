using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors.BitVectors;

/// <summary>
/// 32 wide bit vector
/// </summary>
/// <param name="data">Initial data</param>
[PublicAPI]
public struct BitVector32(uint data) : IBitVector<uint, BitVector32>
{
    /// <inheritdoc />
    public static int Size => 32;

    /// <summary>
    /// Creates a new BitVector via copy
    /// </summary>
    /// <param name="other">Other bit vector to copy</param>
    public BitVector32(BitVector32 other) : this(other.Data) { }

    /// <inheritdoc />
    public uint Data { get; set; } = data;

    /// <inheritdoc />
    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector32)} range");

            return (this.Data & 1U.MaskBit(index)) is not 0U;
        }
        set
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector32)} range");

            if (value)
            {
                this.Data |= 1U.MaskBit(index);
            }
            else
            {
                this.Data &= 1U.InverseMaskBit(index);
            }
        }
    }

    /// <inheritdoc />
    public bool this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this[index.GetOffset(Size)];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this[index.GetOffset(Size)] = value;
    }

    /// <inheritdoc />
    public BitVector32 this[Range range]
    {
        get
        {
            // Get start and length
            (int start, int length) = range.GetOffsetAndLength(Size);
            int end = start + length;

            // Check range
            if (start < 0 || end >= Size) throw new ArgumentOutOfRangeException(nameof(range), range, $"Range outside of {nameof(BitVector32)} range");

            // Create mask over range
            uint mask = uint.MaxValue;
            int endCrop = Size - end;
            mask <<= endCrop;
            mask >>= endCrop + start - 1;
            mask <<= start;

            // Return masked value
            return this & mask;
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InvertBit(int index) => this[index] ^= true;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InvertBit(Index index) => this[index] ^= true;

    /// <inheritdoc />
    public static BitVector32 FromBitArray(ReadOnlySpan<bool> bits)
    {
        if (bits.Length > Size) throw new ArgumentException($"{nameof(BitVector32)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        uint data = 0U;
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= 1U;
            }
        }

        // Return result
        return data;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitVector32 other) => this == other;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector32 value && Equals(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this.Data.GetHashCode();

    /// <inheritdoc cref="BitVectorExtensions.ToBitString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToBitString();

    /// <summary>
    /// Bitwise or operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise or of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator |(BitVector32 a, BitVector32 b) => new() { Data = a.Data | b.Data };

    /// <summary>
    /// Bitwise and operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise and of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator &(BitVector32 a, BitVector32 b) => new() { Data = a.Data & b.Data };

    /// <summary>
    /// Bitwise xor operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise xor of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator ^(BitVector32 a, BitVector32 b) => new() { Data = a.Data ^ b.Data };

    /// <summary>
    /// Left shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the left-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator <<(BitVector32 vector, int shift) => new() { Data = vector.Data << shift };

    /// <summary>
    /// Right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the right-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator >>(BitVector32 vector, int shift) => new() { Data = vector.Data >> shift };

    /// <summary>
    /// Unsigned right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the unsigned right-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector32 operator >>>(BitVector32 vector, int shift) => new() { Data = vector.Data >>> shift };

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors have identical data, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitVector32 a, BitVector32 b) => a.Data == b.Data;

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors do not have identical data, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitVector32 a, BitVector32 b) => a.Data != b.Data;

    /// <summary>
    /// Truthiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a nonzero value, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(BitVector32 vector) => vector.Data != 0U;

    /// <summary>
    /// Falsiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a zero value, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(BitVector32 vector) => vector.Data == 0U;

    /// <summary>
    /// Implicit cast from data type
    /// </summary>
    /// <param name="value">Data value</param>
    /// <returns>Creates a new vector with the specified data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BitVector32(uint value) => new() { Data = value };

    /// <summary>
    /// Explicit cast from to type
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns>Extracts the data from this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator uint(BitVector32 vector) => vector.Data;
}
