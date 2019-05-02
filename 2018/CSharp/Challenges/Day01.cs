using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Day 1 challenge
    /// </summary>
    public sealed class Day01 : Challenge
    {
        #region Properties
        /// <summary>
        /// Challenge ID
        /// </summary>
        public override int ID { get; } = 1;
        #endregion

        #region Methods
        /// <summary>
        /// Solver
        /// </summary>
        public override void Solve()
        {
            List<int> jumps = new List<int>();
            HashSet<int> frequencies = new HashSet<int>();
            int frequency = 0;
            foreach (int jump in GetLines().Select(int.Parse))
            {
                jumps.Add(jump);
                frequency += jump;
                frequencies.Add(frequency);
            }

            Print("Part one frequency: " + frequency);

            bool found = false;
            while (!found)
            {
                foreach (int jump in jumps)
                {
                    frequency += jump;
                    if (!frequencies.Add(frequency))
                    {
                        found = true;
                        break;
                    }
                }
            }

            Print("Part two repeated: " + frequency);
        }
        #endregion
    }
}
