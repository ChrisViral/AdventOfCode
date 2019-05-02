using System;
using System.Text.RegularExpressions;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Day 5 challenge
    /// </summary>
    public class Day05 : Challenge
    {
        #region Properties
        /// <summary>
        /// Day ID
        /// </summary>
        public override int ID { get; } = 5;
        #endregion

        #region Methods
        /// <summary>
        /// Challenge solver
        /// </summary>
        public override void Solve()
        {
            string original = GetLine();

            string polymer = React(original);
            int length = polymer.Length;
            Print("Part one final length: " + length);
            
            for (char c = 'a'; c <= 'z'; c++)
            {
                length = Math.Min(length, React(Regex.Replace(original, c.ToString(), string.Empty, RegexOptions.IgnoreCase)).Length);
            }

            Print("Part two final length: " + length);
        }

        /// <summary>
        /// Fully reacts a polymer chain
        /// </summary>
        /// <param name="polymer">Polymer chain to react</param>
        /// <returns>The reacted version of the polymer chain</returns>
        private static string React(string polymer)
        {
            if (polymer.Length < 2) { return polymer; }

            bool react;
            do
            {
                react = false;
                char prev = polymer[0];
                for (int i = 1; i < polymer.Length; i++)
                {
                    char curr = polymer[i];
                    if (char.ToLowerInvariant(prev) == char.ToLowerInvariant(curr) && char.IsLower(prev) != char.IsLower(curr))
                    {
                        polymer = polymer.Replace(prev.ToString() + curr, string.Empty);
                        react = true;
                        break;
                    }

                    prev = curr;
                }
            }
            while (react);

            return polymer;
        }
        #endregion
    }
}
