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
    public FromRange(Range range)
    {
        this.sign = Math.Sign(range.End.Value - range.Start.Value);
        if (this.sign is 0) this.sign = 1;

        this.start = range.Start.IsFromEnd   ? range.Start.Value           : range.Start.Value - this.sign;
        this.end     = range.End.IsFromEnd   ? range.End.Value + this.sign : range.End.Value;
        this.lastValue = this.start;
    }

    /// <inheritdoc />
    public bool TryGetNext(out int current)
    {
        if (this.lastValue == this.end)
        {
            current = 0;
            return false;
        }

        current = this.lastValue += this.sign;
        return true;
    }

    /// <inheritdoc />
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = (this.end - this.start) * this.sign;
        return true;
    }

    /// <inheritdoc />
    public bool TryGetSpan(out ReadOnlySpan<int> span)
    {
        span = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryCopyTo(scoped Span<int> destination, Index offset) => false;

    /// <inheritdoc />
    public void Dispose() { }
}
