using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.Utils
{
    /// <summary>
    /// General Advent of Code utility methods
    /// </summary>
    public static class AoCUtils
    {
        #region Static methods
        /// <summary>
        /// Combines input lines into sequences, separated by empty lines
        /// </summary>
        /// <param name="input">Input lines</param>
        /// <returns>An enumerable of the packed input</returns>
        public static IEnumerable<List<string>> CombineLines(string[] input)
        {
            List<string> pack = new();
            foreach (string line in input)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (pack.Count is not 0)
                    {
                        yield return pack;
                        pack = new List<string>();
                    }
                }
                else
                {
                    pack.Add(line);
                }
            }

            if (pack.Count is not 0)
            {
                yield return pack;
            }
        }

        /// <summary>
        /// Logs the answer to Part 1 to the console and results file
        /// </summary>
        /// <param name="answer">Answer to log</param>
        public static void LogPart1(object answer) => Trace.WriteLine($"Part 1: {answer}");
        
        /// <summary>
        /// Logs the answer to Part 3 to the console and results file
        /// </summary>
        /// <param name="answer">Answer to log</param>
        public static void LogPart2(object answer) => Trace.WriteLine($"Part 2: {answer}");
        #endregion
        
        #region Extension methods
        /// <summary>
        /// Transforms the range into an enumerable over it's start and end
        /// </summary>
        /// <returns>An enumerable over the entire Range</returns>
        /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
        public static IEnumerable<int> AsEnumerable(this Range range)
        {
            //Make sure the indices aren't from the end
            if (range.Start.IsFromEnd || range.End.IsFromEnd) throw new ArgumentException("Range indices cannot be from end for enumeration", nameof(range));
            
            return range.End.Value > range.Start.Value ? Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value - 1) : Enumerable.Range(0, 0);
        }

        /// <summary>
        /// Range GetEnumerator method, allows foreach over a range as long as the indices are not from the end
        /// </summary>
        /// <returns>An enumerator over the entire range</returns>
        /// <exception cref="ArgumentException">If any of the indices are marked as from the end</exception>
        public static IEnumerator<int> GetEnumerator(this Range range) => range.AsEnumerable().GetEnumerator();
        #endregion
    }
}