using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace AdventOfCode.Utils.BitVectors;

/// <summary>
///
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TSelf"></typeparam>
[PublicAPI]
public interface IBitVector<TData, TSelf> : IEquatable<TSelf>
    where TData : IBinaryInteger<TData>, IUnsignedNumber<TData>
    where TSelf : struct, IBitVector<TData, TSelf>
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
    /// Vector data
    /// </summary>
    TData Data { get; set; }

    /// <summary>
    /// Inverts the bit at the given index
    /// </summary>
    /// <param name="index">Index to invert the bit at</param>
    /// <exception cref="ArgumentOutOfRangeException">When <paramref name="index"/> is less than 0 or greater or equal to <see cref="Size"/></exception>
    void InvertBit(int index);

    /// <inheritdoc cref="InvertBit(int)"/>
    void InvertBit(Index index);

    /// <summary>
    /// Creates a new BitVector from a given list of bits
    /// </summary>
    /// <param name="bits">Bits to create the vector from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">When the size of <paramref name="bits"/> is greater than <see cref="Size"/></exception>
    /// <returns>The created BitVector from the specified bits</returns>
    static abstract TSelf FromBitArray(IReadOnlyList<bool> bits);
}
