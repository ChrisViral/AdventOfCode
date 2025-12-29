using System.Runtime.CompilerServices;
using AdventOfCode.Utils.ValueEnumerators;
using JetBrains.Annotations;
using ZLinq;

// ReSharper disable once CheckNamespace
namespace AdventOfCode.Utils.Extensions.Ranges;

/// <summary>
/// Range extension methods
/// </summary>
/// ReSharper disable once UnusedMember.Global
[PublicAPI]
public static class RangeExtensions
{
    /// <summary>
    /// Optimized Range enumerator struct
    /// </summary>
    /// <param name="range">Range to create the iterator for</param>
    public ref struct RangeEnumerator(Range range)
    {
        private FromRange enumerator = new(range);

        /// <summary>
        /// Current iteration pointer
        /// </summary>
        public int Current { get; private set; }

        /// <summary>
        /// Moves the iteration forwards one step
        /// </summary>
        /// <returns>True if the iterator is still within bounds, otherwise false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (this.enumerator.TryGetNext(out int current))
            {
                this.Current = current;
                return true;
            }

            this.Current = -1;
            return false;
        }
    }

    extension(Range range)
    {
        /// <summary>
        /// Checks if a value is contained within the range<br/>
        /// If <see cref="Range.Start"/> is marked as <see cref="Index.IsFromEnd"/>, then the first value is excluded<br/>
        /// If <see cref="Range.End"/> is marked as <see cref="Index.IsFromEnd"/>, then the last value is included
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns>True if the value is within the range, false otherwise</returns>
        public bool IsInRange(int value)
        {
            int start = range.Start.IsFromEnd ? range.Start.Value + 1 : range.Start.Value;
            int end   = range.End.IsFromEnd   ? range.End.Value       : range.End.Value - 1;
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
        /// <returns>An enumerable over the specified range</returns>
        public ValueEnumerable<FromRange, int> AsValueEnumerable()
        {
            return new ValueEnumerable<FromRange, int>(new FromRange(range));
        }

        /// <summary>
        /// Range GetEnumerator method, allows foreach over a range as long as the indices are not from the end
        /// </summary>
        /// <returns>An enumerator over the entire range</returns>
        /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
        public RangeEnumerator GetEnumerator() => new(range);

        /// <summary>
        /// Deconstructs the range into a tuple containing the start and end values
        /// </summary>
        /// <param name="start">Start value output</param>
        /// <param name="end">End value output</param>
        public void Deconstruct(out int start, out int end)
        {
            start = range.Start.Value;
            end   = range.End.Value;
        }
    }
}
