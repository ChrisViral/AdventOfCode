using System.Collections.Generic;
using System.Diagnostics;

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
    }
}