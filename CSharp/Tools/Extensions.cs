using System.Collections.Generic;
using System.Text;

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
    }
}
