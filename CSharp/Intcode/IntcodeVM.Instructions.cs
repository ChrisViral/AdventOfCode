using System.Runtime.CompilerServices;

namespace AdventOfCode.Intcode;

public partial class IntcodeVM
{
    /// <summary>
    /// Adds integers specified by the first two operands and stores them in the location of the third operand
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Add()
    {
        long a = ReadNextInt64();
        long b = ReadNextInt64();
        long c = ReadNextInt64();
        long result = ReadInt64(a) + ReadInt64(b);
        WriteInt64(c, result);
    }

    /// <summary>
    /// Multiplies integers specified by the first two operands and stores them in the location of the third operand
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Multiply()
    {
        long a = ReadNextInt64();
        long b = ReadNextInt64();
        long c = ReadNextInt64();
        long result = ReadInt64(a) * ReadInt64(b);
        WriteInt64(c, result);
    }
}