using System;
using System.Collections.Generic;

namespace AdventOfCode.Extensions;

/// <summary>
/// Range extension methods
/// </summary>
public static class RangeExtensions
{
    /// <summary>
    /// Optimized Range enumerator struct
    /// </summary>
    public ref struct RangeEnumerator
    {
        private readonly int end;
        private readonly int sign;

        /// <summary>
        /// Current iteration pointer
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// Creates a new range iterator from the given range
        /// </summary>
        /// <param name="range">Range to create the iterator for</param>
        public RangeEnumerator(Range range)
        {
            this.sign    = Math.Sign(range.End.Value - range.Start.Value);
            this.Current = range.Start.IsFromEnd ? range.Start.Value           : range.Start.Value - this.sign;
            this.end     = range.End.IsFromEnd   ? range.End.Value + this.sign : range.End.Value;
        }

        /// <summary>
        /// Moves the iteration forwards one step
        /// </summary>
        /// <returns>True if the iterator is still within bounds, otherwise false</returns>
        public bool MoveNext()
        {
            this.Current += this.sign;
            return this.Current == this.end;
        }
    }

    #region Extension methods
    /// <summary>
    /// Checks if a value is contained within the range<br/>
    /// If <see cref="Range.Start"/> is marked as <see cref="Index.IsFromEnd"/>, then the first value is excluded<br/>
    /// If <see cref="Range.End"/> is marked as <see cref="Index.IsFromEnd"/>, then the last value is included
    /// </summary>
    /// <param name="range">Range to check within</param>
    /// <param name="value">Value to check</param>
    /// <returns>True if the value is within the range, false otherwise</returns>
    public static bool IsInRange(this Range range, int value)
    {
        int start = range.Start.IsFromEnd ? range.Start.Value : range.Start.Value + 1;
        int end   = range.End.IsFromEnd   ? range.End.Value   : range.End.Value   + 1;
        if (start > end)
        {
            (start, end) = (end, start);
        }
        return value >= start && value <= end;
    }

    /// <summary>
    /// Transforms the range into an enumerable from <see cref="Range.Start"/> to <see cref="Range.End"/><br/>
    /// If <see cref="Range.Start"/> is marked as <see cref="Index.IsFromEnd"/>, then the first value is excluded<br/>
    /// If <see cref="Range.End"/> is marked as <see cref="Index.IsFromEnd"/>, then the last value is included
    /// </summary>
    /// <param name="range">Range to convert to enumerable</param>
    /// <returns>An enumerable over the specified range</returns>
    public static IEnumerable<int> AsEnumerable(this Range range)
    {
        int sign  = Math.Sign(range.End.Value - range.Start.Value);
        int start = range.Start.IsFromEnd ? range.Start.Value + sign : range.Start.Value;
        int end   = range.End.IsFromEnd   ? range.End.Value + sign   : range.End.Value;
        for (int i = start; i != end; i += sign)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Range GetEnumerator method, allows foreach over a range as long as the indices are not from the end
    /// </summary>
    /// <param name="range">Range to get the Enumerator for</param>
    /// <returns>An enumerator over the entire range</returns>
    /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
    public static RangeEnumerator GetEnumerator(this Range range) => new(range);
    #endregion
}
