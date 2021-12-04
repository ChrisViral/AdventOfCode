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
    /// Transforms the range into an enumerable over it's start and end
    /// </summary>
    /// <param name="range">Range to convert to enumerable</param>
    /// <returns>An enumerable over the entire Range</returns>
    /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
    public static IEnumerable<int> AsEnumerable(this Range range)
    {
        //Make sure the indices aren't from the end
        if (range.Start.IsFromEnd || range.End.IsFromEnd) throw new ArgumentException("Range indices cannot be from end for enumeration", nameof(range));

        return range.End.Value > range.Start.Value ? Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value) : Enumerable.Range(0, 0);
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
