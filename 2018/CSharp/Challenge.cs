using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode
{
    /// <summary>
    /// Challenge base class
    /// </summary>
    public abstract class Challenge
    {
        #region Properties
        /// <summary>
        /// ID of the challenge
        /// </summary>
        public abstract int ID { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the first line of the input for this challenge
        /// </summary>
        /// <returns>First line of the input</returns>
        protected string GetLine()
        {
            using (StreamReader fs = File.OpenText($"Input/day{this.ID}.txt"))
            {
                return fs.ReadLine()?.Trim();
            }
        }

        /// <summary>
        /// Gets all the input lines for this given challenge
        /// </summary>
        /// <returns>Enumerable of all lines</returns>
        protected IEnumerable<string> GetLines()
        {
            using (StreamReader fs = File.OpenText($"Input/day{this.ID}.txt"))
            {
                for (string line = fs.ReadLine(); line != null; line = fs.ReadLine())
                {
                    yield return line.Trim();
                }
            }
        }

        /// <summary>
        /// Prints the given message to the Console output
        /// </summary>
        /// <param name="message"></param>
        protected void Print(string message) => Console.WriteLine(message);

        /// <summary>
        /// Solving method of the challenge
        /// </summary>
        public abstract void Solve();
        #endregion
    }
}
