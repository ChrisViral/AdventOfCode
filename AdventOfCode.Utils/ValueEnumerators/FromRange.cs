using System.Runtime.CompilerServices;
using ZLinq;

namespace AdventOfCode.Utils.ValueEnumerators;

/// <summary>
/// Optimized Range enumerator struct
/// </summary>
public ref struct FromRange : IValueEnumerator<int>
{
    private readonly int start;
    private readonly int end;
    private readonly int sign;
    private int lastValue;

    /// <summary>
    /// Creates a new range iterator from the given range
    /// </summary>
    /// <param name="range">Range to create the iterator for</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FromRange(Range range)
    {
        this.sign = Math.Sign(range.End.Value - range.Start.Value);
        if (this.sign is 0) this.sign = 1;

        this.start = range.Start.IsFromEnd   ? range.Start.Value  + this.sign : range.Start.Value;
        this.end     = range.End.IsFromEnd   ? range.End.Value + this.sign    : range.End.Value;
        this.lastValue = this.start;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetNext(out int current)
    {
        if (this.lastValue == this.end)
        {
            current = 0;
            return false;
        }

        current = this.lastValue;
        this.lastValue += this.sign;
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = (this.end - this.start) * this.sign;
        return true;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSpan(out ReadOnlySpan<int> span)
    {
        span = default;
        return false;
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(scoped Span<int> destination, Index offset) => false;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() { }
}
