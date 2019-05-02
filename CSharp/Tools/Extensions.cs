using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Tools
{
    /// <summary>
    /// Extension methods container
    /// </summary>
    public static class Extensions
    {
        #region StringBuilder Extensions
        /// <summary>
        /// Appends multiple objects to a StringBuilder, separated by the given string
        /// </summary>
        /// <typeparam name="T">Type of elements in the Enumerable</typeparam>
        /// <param name="sb">StringBuilder to append to</param>
        /// <param name="source">Enumerable to loop through</param>
        /// <param name="separator">Separator string, defaults to the empty string</param>
        /// <returns>The StringBuilder instance after the appending is done</returns>
        public static StringBuilder AppendJoin<T>(this StringBuilder sb, IEnumerable<T> source, string separator = "")
        {
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    sb.Append(e.Current);
                    while (e.MoveNext())
                    {
                        sb.Append(separator).Append(e.Current);
                    }
                }
            }
 
            return sb;
        }
 
        /// <summary>
        /// Appends multiple objects to a StringBuilder, separated by the given string
        /// </summary>
        /// <typeparam name="T">Type of elements in the Enumerable</typeparam>
        /// <param name="sb">StringBuilder to append to</param>
        /// <param name="source">Enumerable to loop through</param>
        /// <param name="separator">Separator character</param>
        /// <returns>The StringBuilder instance after the appending is done</returns>
        public static StringBuilder AppendJoin<T>(this StringBuilder sb, IEnumerable<T> source, char separator)
        {
            using (IEnumerator<T> e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    sb.Append(e.Current);
                    while (e.MoveNext())
                    {
                        sb.Append(separator).Append(e.Current);
                    }
                }
            }
 
            return sb;
        }

        /// <summary>
        /// Copies the contents of a StringBuilder instance to an array
        /// </summary>
        /// <param name="sb">StringBuilder to copy</param>
        /// <returns>Contents array of the StringBuilder</returns>
        public static char[] ToCharArray(this StringBuilder sb)
        {
            char[] buffer = new char[sb.Length];
            sb.CopyTo(0, buffer, 0, sb.Length);
            return buffer;
        }
        #endregion

        #region Regex Extensions
        /// <summary>
        /// Uses a regex pattern and extracts its capture groups, excluding the entire match
        /// </summary>
        /// <param name="pattern">Pattern used to get the groups</param>
        /// <param name="line">Line to get the groups from</param>
        /// <returns>An enumerable of the match groups</returns>
        public static IEnumerable<Group> GetGroups(this Regex pattern, string line) => pattern.Match(line).Groups.Cast<Group>().Skip(1);

        /// <summary>
        /// Uses the capture groups of a given regex pattern to extract data from a string
        /// </summary>
        /// <param name="pattern">Pattern used to extract the data</param>
        /// <param name="line">Line to extract the data from</param>
        /// <returns>An enumerable of all the extracted data</returns>
        public static string[] ParseData(this Regex pattern, string line) => pattern.GetGroups(line).Select(g => g.Value).ToArray();

        /// <summary>
        /// Uses the capture groups of a given regex pattern to parse and extract data from a string
        /// </summary>
        /// <param name="pattern">Pattern used to extract the data</param>
        /// <param name="line">Line to extract the data from</param>
        /// <param name="parse">Parsing function to apply to each group</param>
        /// <typeparam name="T">Type to parse the data to</typeparam>
        /// <returns>An enumerable of all the extracted parsed data</returns>
        public static T[] ParseData<T>(this Regex pattern, string line, Func<string, T> parse) => pattern.GetGroups(line).Select(g => parse(g.Value)).ToArray();
        #endregion

        #region Enumerable Extensions
        /// <summary>
        /// Finds the object with the maximum value in the enumerable
        /// </summary>
        /// <param name="sequence">Enumerable to loop through</param>
        /// <param name="selector">Function calculating the value that we want the max from</param>
        /// <typeparam name="T">Type of objects in the Enumerable</typeparam>
        /// <typeparam name="TU">Comparing type, must implement <see cref="IComparable{T}"/></typeparam>
        /// <returns>The object with the maximum value in the enumerable</returns>
        public static T MaxValue<T, TU>(this IEnumerable<T> sequence, Func<T, TU> selector) where TU : IComparable<TU>
        {
            using (IEnumerator<T> e = sequence.GetEnumerator())
            {
                if (!e.MoveNext()) { throw new InvalidOperationException("No elements in sequence"); }
 
                T max = e.Current;
                TU value = selector(max);
                while (e.MoveNext())
                {
                    TU v = selector(e.Current);
                    if (value.CompareTo(v) < 0)
                    {
                        max = e.Current;
                        value = v;
                    }
                }
 
                return max;
            }
        }
 
        /// <summary>
        /// Finds the object with the minimum value in the enumerable
        /// </summary>
        /// <param name="sequence">Enumerable to loop through</param>
        /// <param name="selector">Function calculating the value that we want the min from</param>
        /// <typeparam name="T">Type of objects in the Enumerable</typeparam>
        /// <typeparam name="TU">Comparing type, must implement <see cref="IComparable{T}"/>/></typeparam>
        /// <returns>The object with the minimum value in the enumerable</returns>
        public static T MinValue<T, TU>(this IEnumerable<T> sequence, Func<T, TU> selector) where TU : IComparable<TU>
        {
            using (IEnumerator<T> e = sequence.GetEnumerator())
            {
                if (!e.MoveNext()) { throw new InvalidOperationException("No elements in sequence"); }
 
                T min = e.Current;
                TU value = selector(min);
                while (e.MoveNext())
                {
                    TU v = selector(e.Current);
                    if (value.CompareTo(v) > 0)
                    {
                        min = e.Current;
                        value = v;
                    }
                }
 
                return min;
            }
        }

        /// <summary>
        /// Gets the index of the maximum value in this enumerable
        /// </summary>
        /// <param name="sequence">Enumerable implementation to look through</param>
        /// <typeparam name="T">Type of element in the Enumerable</typeparam>
        /// <returns>The index of the maximum element</returns>
        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            using (IEnumerator<T> e = sequence.GetEnumerator())
            {
                if (!e.MoveNext()) { return -1; }

                int index = 0, i = 0;
                T max = e.Current;
                while (e.MoveNext())
                {
                    i++;
                    if (e.Current?.CompareTo(max) > 0)
                    {
                        max = e.Current;
                        index = i;
                    }
                }

                return index;
            }
        }

        /// <summary>
        /// Gets the index of the minimum value in this enumerable
        /// </summary>
        /// <param name="sequence">Enumerable implementation to look through</param>
        /// <typeparam name="T">Type of element in the Enumerable</typeparam>
        /// <returns>The index of the minimum element</returns>
        public static int MinIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            using (IEnumerator<T> e = sequence.GetEnumerator())
            {
                if (!e.MoveNext()) { return -1; }

                int index = 0, i = 0;
                T max = e.Current;
                while (e.MoveNext())
                {
                    i++;
                    if (e.Current?.CompareTo(max) < 0)
                    {
                        max = e.Current;
                        index = i;
                    }
                }

                return index;
            }
        }
        #endregion
    }
}
