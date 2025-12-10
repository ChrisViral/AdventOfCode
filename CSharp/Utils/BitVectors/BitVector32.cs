using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.BitVectors;

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
        get => this[index.GetOffset(Size)];
        set => this[index.GetOffset(Size)] = value;
    }

    /// <inheritdoc />
    public void InvertBit(int index) => this[index] ^= true;

    /// <inheritdoc />
    public void InvertBit(Index index) => this[index] ^= true;

    /// <inheritdoc />
    public static BitVector32 FromBitArray(IReadOnlyList<bool> bits)
    {
        if (bits.Count > Size) throw new ArgumentException($"{nameof(BitVector32)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        uint data = 0U;
        for (int i = bits.Count - 1; i >= 0; i--)
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
    public bool Equals(BitVector32 other) => this.Data == other.Data;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector32 value && Equals(value);

    /// <inheritdoc />
    public override int GetHashCode() => this.Data.GetHashCode();
}
