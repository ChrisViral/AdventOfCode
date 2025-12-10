using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AdventOfCode.Extensions.Numbers;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.BitVectors;

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

            return (this.Data & ((byte)1).MaskBit(index)) is not 0;
        }
        set
        {
            if (index < 0 || index >= Size) throw new ArgumentOutOfRangeException(nameof(index), index, $"Index outside of {nameof(BitVector8)} range");

            if (value)
            {
                this.Data |= ((byte)1).MaskBit(index);
            }
            else
            {
                this.Data &= ((byte)1).InverseMaskBit(index);
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
    public static BitVector8 FromBitArray(IReadOnlyList<bool> bits)
    {
        if (bits.Count > Size) throw new ArgumentException($"{nameof(BitVector8)} only supports up to {Size} bits", nameof(bits));

        // Mask out data
        byte data = 0;
        for (int i = bits.Count - 1; i >= 0; i--)
        {
            data <<= 1;
            if (bits[i])
            {
                data |= 1;
            }
        }

        // Return result
        return new BitVector8(data);
    }

    /// <inheritdoc />
    public bool Equals(BitVector8 other) => this.Data == other.Data;

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BitVector8 value && Equals(value);

    /// <inheritdoc />
    public override int GetHashCode() => this.Data.GetHashCode();
}
