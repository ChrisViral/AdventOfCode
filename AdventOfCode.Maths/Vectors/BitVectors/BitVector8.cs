using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Maths.Vectors.BitVectors;

/// <summary>
/// 8 wide bit vector
/// </summary>
/// <param name="data">Initial data</param>
[PublicAPI]
public struct BitVector8(byte data) : IBitVector<byte, BitVector8>
{
    /// <inheritdoc />
    public static int Size => 8;

    /// <summary>
    /// Creates a new BitVector via copy
    /// </summary>
    /// <param name="other">Other bit vector to copy</param>
    public BitVector8(BitVector8 other) : this(other.Data) { }

    /// <inheritdoc />
    public byte Data { get; set; } = data;

    /// <inheritdoc />
    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector8)} range");

            return (this.Data & ((byte)1U).MaskBit(index)) is not 0;
        }
        set
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector8)} range");

            if (value)
            {
                this.Data |= ((byte)1U).MaskBit(index);
            }
            else
            {
                this.Data &= ((byte)1U).InverseMaskBit(index);
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
    public BitVector8 this[Range range]
    {
        get
        {
            // Get start and length
            (int start, int length) = range.GetOffsetAndLength(Size);
            int end = start + length;

            // Check range
            if (start < 0 || end >= Size) throw new ArgumentOutOfRangeException(nameof(range), range, $"Range outside of {nameof(BitVector8)} range");

            // Create mask over range
            byte mask = byte.MaxValue;
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
    public static BitVector8 FromBitArray(ReadOnlySpan<bool> bits)
    {
        if (bits.Length > Size) throw new ArgumentException($"{nameof(BitVector8)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        byte data = 0;
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= 1;
            }
        }

        // Return result
        return data;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitVector8 other) => this == other;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector8 value && Equals(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this.Data.GetHashCode();

    /// <inheritdoc cref="BitVectorExtensions.ToBitString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToBitString();

    /// <summary>
    /// Bitwise complement operator
    /// </summary>
    /// <param name="vector">Vector to complement</param>
    /// <returns>A new vector containing the bitwise complement of this instance</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator ~(BitVector8 vector) => new() { Data = (byte)~vector.Data };

    /// <summary>
    /// Bitwise or operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise or of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator |(BitVector8 a, BitVector8 b) => new() { Data = (byte)(a.Data | b.Data) };

    /// <summary>
    /// Bitwise and operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise and of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator &(BitVector8 a, BitVector8 b) => new() { Data = (byte)(a.Data & b.Data) };

    /// <summary>
    /// Bitwise xor operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns>A new vector made of the bitwise xor of both vectors</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator ^(BitVector8 a, BitVector8 b) => new() { Data = (byte)(a.Data ^ b.Data) };

    /// <summary>
    /// Left shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the left-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator <<(BitVector8 vector, int shift) => new() { Data = (byte)(vector.Data << shift) };

    /// <summary>
    /// Right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the right-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator >>(BitVector8 vector, int shift) => new() { Data = (byte)(vector.Data >> shift) };

    /// <summary>
    /// Unsigned right shift operator
    /// </summary>
    /// <param name="vector">Vector to shift</param>
    /// <param name="shift">Shift amount</param>
    /// <returns>A new vector made of the unsigned right-shifted data of this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitVector8 operator >>>(BitVector8 vector, int shift) => new() { Data = (byte)(vector.Data >>> shift) };

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors have identical data, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitVector8 a, BitVector8 b) => a.Data == b.Data;

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors do not have identical data, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitVector8 a, BitVector8 b) => a.Data != b.Data;

    /// <summary>
    /// Truthiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a nonzero value, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(BitVector8 vector) => vector.Data != 0U;

    /// <summary>
    /// Falsiness
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns><see langword="true"/> if the vector has a zero value, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(BitVector8 vector) => vector.Data == 0U;

    /// <summary>
    /// Implicit cast from data type
    /// </summary>
    /// <param name="value">Data value</param>
    /// <returns>Creates a new vector with the specified data</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BitVector8(byte value) => new() { Data = value };

    /// <summary>
    /// Explicit cast from to type
    /// </summary>
    /// <param name="vector">Vector</param>
    /// <returns>Extracts the data from this vector</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator byte(BitVector8 vector) => vector.Data;
}
