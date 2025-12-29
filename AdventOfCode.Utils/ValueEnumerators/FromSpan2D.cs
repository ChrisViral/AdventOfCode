using CommunityToolkit.HighPerformance;
using JetBrains.Annotations;
using ZLinq;
using ZLinq.Internal;

namespace AdventOfCode.Utils.ValueEnumerators;

/// <summary>
/// Span2D bridge for ZLinq
/// </summary>
/// <param name="span">Span instance</param>
/// <typeparam name="T">Value contained within the span</typeparam>
[PublicAPI]
public ref struct FromSpan2D<T>(ReadOnlySpan2D<T> span) : IValueEnumerator<T>
{
    private readonly ReadOnlySpan2D<T> spanRef = span;
    private int currentColumn = 0;
    private int currentRow = span.IsEmpty ? -1 : 0;

    /// <inheritdoc />
    public bool TryGetNext(out T current)
    {
        if (this.currentRow is -1)
        {
            current = default!;
            return false;
        }

        current = this.spanRef[this.currentRow, this.currentColumn];
        this.currentColumn++;
        if (this.currentColumn == this.spanRef.Width)
        {
            this.currentColumn = 0;
            this.currentRow++;
            if (this.currentRow == this.spanRef.Height)
            {
                this.currentRow = -1;
            }
        }
        return true;
    }

    /// <inheritdoc />
    public bool TryGetNonEnumeratedCount(out int count)
    {
        count = (int)this.spanRef.Length;
        return true;
    }

    /// <inheritdoc />
    public bool TryGetSpan(out ReadOnlySpan<T> span)
    {
        if (this.spanRef.TryGetSpan(out span))
        {
            return true;
        }

        span = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryCopyTo(scoped Span<T> destination, Index offset)
    {
        // Try and get the underlying span
        if (this.spanRef.TryGetSpan(out ReadOnlySpan<T> underlying))
        {
            if (EnumeratorHelper.TryGetSlice(underlying, offset, destination.Length, out ReadOnlySpan<T> slice))
            {
                slice.CopyTo(destination);
                return true;
            }
            return false;
        }

        // Check the length in the span we're dealing with
        int length = (int)this.spanRef.Length;
        int start = offset.GetOffset(length);
        int remaining = length - start;
        if (remaining <= 0) return false;

        // Get how much we have to put into the destination
        remaining = Math.Min(destination.Length, remaining);
        // And where we're starting in the grid
        (int row, int column) = Math.DivRem(start, this.spanRef.Width);
        int currentColumnWidth = this.spanRef.Width - column;

        // If we have less to copy than the starting column width, copy that only
        if (remaining <= currentColumnWidth)
        {
            this.spanRef
                .GetRowSpan(row)
                .Slice(column, remaining)
                .CopyTo(destination);
            return true;
        }

        // Else copy the segment of the first column
        this.spanRef
            .GetRowSpan(row++)[column..]
            .CopyTo(destination);
        Span<T> remainingDestination = destination[currentColumnWidth..];
        remaining -= currentColumnWidth;

        // Then while we can copy full columns, do so
        while (remaining >= this.spanRef.Width)
        {
            this.spanRef
                .GetRowSpan(row++)
                .CopyTo(remainingDestination);
            remaining -= this.spanRef.Width;
            remainingDestination = remainingDestination[this.spanRef.Width..];
        }

        // Finally, check if we need to copy the segment of a column at the end
        if (remaining > 0)
        {
            this.spanRef
                .GetRowSpan(row)[..remaining]
                .CopyTo(remainingDestination);
        }
        return true;
    }

    /// <inheritdoc />
    public void Dispose() { }
}
