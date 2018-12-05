
using System.Collections.Generic;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Day 1 Part 2 challenge
    /// </summary>
    public sealed class Day1_2 : Challenge
    {
        #region Properties
        /// <summary>
        /// Challenge ID
        /// </summary>
        public override string ID { get; } = "1-2";
        #endregion

        #region Methods
        /// <summary>
        /// Solver
        /// </summary>
        protected override void Solve()
        {
            int frequency = 0;
            HashSet<int> frequencies = new HashSet<int>();
            List<int> jumps = new List<int>();

            foreach (string line in this.Console.EnumerateLines())
            {
                int jump = int.Parse(line);
                jumps.Add(jump);
                frequency += jump;
                frequencies.Add(frequency);
            }

            int index = 0;
            do
            {
                if (index >= jumps.Count) { index = 0; }
                frequency += jumps[index++];
            }
            while (frequencies.Add(frequency));
            
            this.Console.WriteLine("Looping frequency: " + frequency);
        }
        #endregion
    }
}
