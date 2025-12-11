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
        return new BitVector32(data);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitVector32 other) => this.Data == other.Data;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector32 value && Equals(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this.Data.GetHashCode();

    /// <inheritdoc cref="BitVectorExtensions.ToBitString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToBitString<uint, BitVector32>();

    /// <summary>
    /// Checks if the given BitVectors are equal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are equal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitVector32 a, BitVector32 b) => a.Data == b.Data;

    /// <summary>
    /// Checks if the given BitVectors are unequal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are unequal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitVector32 a, BitVector32 b) => a.Data != b.Data;
}
