using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utils
{
    /// <summary>
    /// Various extension methods
    /// </summary>
    public static class Extensions
    {
        #region Enumerable extensions
        /// <summary>
        /// Applies an action to every member of the enumerable
        /// </summary>
        /// <param name="e">Enumerable to iterate over</param>
        /// <param name="action">Action to apply</param>
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (T t in e)
            {
                action(t);
            }
        }

        /// <summary>
        /// Checks if a collection is empty
        /// </summary>
        /// <param name="collection">Collection to check</param>
        /// <returns>True if the collection is empty, false otherwise</returns>
        public static bool IsEmpty(this ICollection collection) => collection.Count is 0;
        #endregion
        
        #region Range extensions
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
        
        #region Regex extensions
        /// <summary>
        /// Gets all the captured groups of the match
        /// </summary>
        /// <param name="match">Match to get the groups from</param>
        /// <returns>Enumerable of the captured groups</returns>
        public static IEnumerable<Group> GetGroups(this Match match) => match.Groups
                                                                             .Cast<Group>()
                                                                             .Skip(1);
        #endregion
    }
}
