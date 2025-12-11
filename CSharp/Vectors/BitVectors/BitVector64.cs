using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors.BitVectors;

/// <summary>
/// 64 wide bit vector
/// </summary>
/// <param name="data">Initial data</param>
[PublicAPI]
public struct BitVector64(ulong data) : IBitVector<ulong, BitVector64>
{
    /// <inheritdoc />
    public static int Size => 64;

    /// <summary>
    /// Creates a new BitVector via copy
    /// </summary>
    /// <param name="other">Other bit vector to copy</param>
    public BitVector64(BitVector64 other) : this(other.Data) { }

    /// <inheritdoc />
    public ulong Data { get; set; } = data;

    /// <inheritdoc />
    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector64)} range");

            return (this.Data & 1UL.MaskBit(index)) is not 0UL;
        }
        set
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector64)} range");

            if (value)
            {
                this.Data |= 1UL.MaskBit(index);
            }
            else
            {
                this.Data &= 1UL.InverseMaskBit(index);
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InvertBit(int index) => this[index] ^= true;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InvertBit(Index index) => this[index] ^= true;

    /// <inheritdoc />
    public static BitVector64 FromBitArray(ReadOnlySpan<bool> bits)
    {
        if (bits.Length > Size) throw new ArgumentException($"{nameof(BitVector64)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        ulong data = 0UL;
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= 1UL;
            }
        }

        // Return result
        return new BitVector64(data);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitVector64 other) => this.Data == other.Data;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector64 value && Equals(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this.Data.GetHashCode();

    /// <inheritdoc cref="BitVectorExtensions.ToBitString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToBitString<ulong, BitVector64>();

    /// <summary>
    /// Checks if the given BitVectors are equal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are equal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitVector64 a, BitVector64 b) => a.Data == b.Data;

    /// <summary>
    /// Checks if the given BitVectors are unequal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are unequal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitVector64 a, BitVector64 b) => a.Data != b.Data;
}
