using AdventOfCode.Tools;

//It is intended for the base class to dictate which file will be opened
//ReSharper disable VirtualMemberCallInConstructor

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
        public abstract string ID { get; }

        /// <summary>
        /// ConsoleHelper generated to assist with read/writes
        /// </summary>
        protected ConsoleHelper Console { get; private set; }
        #endregion

        #region Methods
        public void Run()
        {
            //Setup Console helper
            using (this.Console = new ConsoleHelper($"Input/input{this.ID}.txt"))
            {
                Solve();
            }
        }

        /// <summary>
        /// Solving method of the challenge
        /// </summary>
        protected abstract void Solve();
        #endregion
    }
}
