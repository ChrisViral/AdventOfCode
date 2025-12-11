using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Vectors.BitVectors;

/// <summary>
/// 16 wide bit vector
/// </summary>
/// <param name="data">Initial data</param>
[PublicAPI]
public struct BitVector16(ushort data) : IBitVector<ushort, BitVector16>
{
    /// <inheritdoc />
    public static int Size => 16;

    /// <summary>
    /// Creates a new BitVector via copy
    /// </summary>
    /// <param name="other">Other bit vector to copy</param>
    public BitVector16(BitVector16 other) : this(other.Data) { }

    /// <inheritdoc />
    public ushort Data { get; set; } = data;

    /// <inheritdoc />
    public bool this[int index]
    {
        get
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector32)} range");

            return (this.Data & ((ushort)1U).MaskBit(index)) is not 0;
        }
        set
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector32)} range");

            if (value)
            {
                this.Data |= ((ushort)1U).MaskBit(index);
            }
            else
            {
                this.Data &= ((ushort)1U).InverseMaskBit(index);
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
    public static BitVector16 FromBitArray(ReadOnlySpan<bool> bits)
    {
        if (bits.Length > Size) throw new ArgumentException($"{nameof(BitVector16)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        ushort data = 0;
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= 1;
            }
        }

        // Return result
        return new BitVector16(data);
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitVector16 other) => this.Data == other.Data;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector16 value && Equals(value);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => this.Data.GetHashCode();

    /// <inheritdoc cref="BitVectorExtensions.ToBitString"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.ToBitString<ushort, BitVector16>();

    /// <summary>
    /// Checks if the given BitVectors are equal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are equal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitVector16 a, BitVector16 b) => a.Data == b.Data;

    /// <summary>
    /// Checks if the given BitVectors are unequal
    /// </summary>
    /// <param name="a">First vector</param>
    /// <param name="b">Second vector</param>
    /// <returns><see langword="true"/> if both vectors are unequal, otherwise <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitVector16 a, BitVector16 b) => a.Data != b.Data;
}
