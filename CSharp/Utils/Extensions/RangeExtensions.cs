using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Utils.Extensions;

/// <summary>
/// Range extension methods
/// </summary>
public static class RangeExtensions
{
    #region Extension methods
    /// <summary>
    /// Transforms the range into an enumerable from <see cref="Range.Start"/> to <see cref="Range.End"/><br/>
    /// If <see cref="Range.End"/> is less than <see cref="Range.Start"/>, then it enumerates from <see cref="Range.End"/> to <see cref="Range.Start"/> instead<br/>
    /// If <see cref="Range.Start"/> is marked as <see cref="Index.IsFromEnd"/>, then the first value is excluded<br/>
    /// If <see cref="Range.End"/> is marked as <see cref="Index.IsFromEnd"/>, then the last value is included
    /// </summary>
    /// <param name="range">Range to convert to enumerable</param>
    /// <returns>An enumerable over the specified range</returns>
    public static IEnumerable<int> AsEnumerable(this Range range)
    {
        (Index start, Index end) = (range.Start, range.End);
        if (start.Value > end.Value)
        {
            (start, end) = (end, start);
        }

        int startValue = start.Value;
        int length = end.Value - start.Value;
        if (range.Start.IsFromEnd) startValue++;
        if (range.End.IsFromEnd)   length++;

        return Enumerable.Range(startValue, length);
    }

    /// <summary>
    /// Range GetEnumerator method, allows foreach over a range as long as the indices are not from the end
    /// </summary>
    /// <param name="range">Range to get the Enumerator for</param>
    /// <returns>An enumerator over the entire range</returns>
    /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
    public static IEnumerator<int> GetEnumerator(this Range range) => range.AsEnumerable().GetEnumerator();
    #endregion
}
