using System.Linq;

namespace AdventOfCode.Challenges
{
    /// <summary>
    /// Day 1 Part 1 challenge
    /// </summary>
    public sealed class Day1_1 : Challenge
    {
        #region Properties
        /// <summary>
        /// Challenge ID
        /// </summary>
        public override string ID { get; } = "1-1";
        #endregion

        #region Methods
        /// <summary>
        /// Solver
        /// </summary>
        protected override void Solve()
        {
            int frequency = this.Console.EnumerateLines().Sum(int.Parse);
            this.Console.WriteLine("Final frequency: " + frequency);
        }
        #endregion
    }
}
