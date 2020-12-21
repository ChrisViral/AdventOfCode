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
        public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count is 0;

        /// <summary>
        /// Returns the sequence where every element is repeated a given amount of times
        /// </summary>
        /// <typeparam name="T">Type of element in the sequence</typeparam>
        /// <param name="e">Enumerable from which to repeat the elements</param>
        /// <param name="count">Amount of times to repeat each element</param>
        /// <returns>An enumerable where all the elements of the original sequence are repeated the specified amount of times</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="count"/> is less than or equal to zero</exception>
        public static IEnumerable<T> RepeatElements<T>(this IEnumerable<T> e, int count)
        {
            switch (count)
            {
                case <= 0:
                    throw new ArgumentOutOfRangeException(nameof(count), count, "Count must be greater than zero");
                
                case 1:
                    //For one simply loop through the elements
                    foreach (T t in e)
                    {
                        yield return t;
                    }
                    break;

                default:
                    //Otherwise repeat each element the right amount of times
                    foreach (T t in e)
                    {
                        foreach (int _ in ..count)
                        {
                            yield return t;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Loops an enumerator over itself until the total length is reached
        /// </summary>
        /// <typeparam name="T">Type of element within the enumerator</typeparam>
        /// <param name="e">Enumerable to loop over</param>
        /// <param name="length">Total length to reach</param>
        /// <param name="copy">If a local copy of the enumerator should be cached, defaults to true</param>
        /// <returns>A sequence looping over the input sequence and of the specified length</returns>
        public static IEnumerable<T> Loop<T>(this IEnumerable<T> e, int length, bool copy = true)
        {
            //Make sure the length is valid
            if (length < 1) yield break;
            
            //Caching
            if (copy)
            {
                //Create cache over first iteration
                List<T> cache = new();
                foreach (T t in e)
                {
                    yield return t;
                    if (--length is 0) yield break;
                    cache.Add(t);
                }

                while (true)
                {
                    //Loop forever
                    foreach (T t in cache)
                    {
                        yield return t;
                        if (--length is 0) yield break;
                    }
                }
            }
            
            //Not caching
            while (true)
            {
                //ReSharper disable once PossibleMultipleEnumeration - intended
                foreach (T t in e)
                {
                    yield return t;
                    if (--length is 0) yield break;
                }
            }
        }
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
        public static IEnumerable<Group> GetCapturedGroups(this Match match) => match.Groups
                                                                                     .Cast<Group>()
                                                                                     .Skip(1)
                                                                                     .Where(g => !string.IsNullOrEmpty(g.Value));
        #endregion
    }
}
