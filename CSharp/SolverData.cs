using System;

namespace AdventOfCode
{
    /// <summary>
    /// SolverData info
    /// </summary>
    public readonly struct SolverData
    {
        #region Constants
        /// <summary>
        /// Amount of expected arguments
        /// </summary>
        private const int ARGS = 2;
        /// <summary>
        /// Type qualifier for the solvers
        /// </summary>
        private const string QUALIFIER = "AdventOfCode.Solvers.AoC";
        #endregion
    
        #region Fields
        public readonly int year;
        public readonly int day;
        public readonly string input;
        public readonly string fullName;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new SolverData for the specified program arguments
        /// </summary>
        /// <param name="args">Program arguments</param>
        /// <exception cref="ArgumentException">If the <paramref name="args"/> are of the inappropriate length, or if the year cannot be parsed to an integer</exception>
        /// <exception cref="ArgumentNullException">If the day is null or empty</exception>
        public SolverData(string[] args)
        {
            if (args.Length is not ARGS) throw new ArgumentException($"Arguments have invalid data, {args.Length} arguments when expected {ARGS}.", nameof(args));
            if (!int.TryParse(args[0], out this.year)) throw new ArgumentException($"Year ({args[0]}) could not be parsed to integer.", $"{nameof(args)}[0]");
            if (!int.TryParse(args[1], out this.day)) throw new ArgumentException($"Day ({args[1]}) could not be parsed to integer.", $"{nameof(args)}[1]");
            
            this.input = InputFetcher.EnsureInput(this.year, this.day);
            this.fullName = $"{QUALIFIER}{this.year}.Day{this.day:D2}";
        }
        #endregion
        
        #region Methods
        /// <inheritdoc cref="object.ToString"/>
        public override string ToString() => $"{this.year} Day{this.day}";
        #endregion
    }
}