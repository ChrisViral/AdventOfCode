#nullable enable
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Base
{
    public interface ISolver
    {
        void Run();
    }

    /// <summary>
    /// Solver base class
    /// </summary>
    public abstract class Solver<T> : ISolver
    {
        #region Constants
        /// <summary>
        /// Split options
        /// </summary>
        private const StringSplitOptions OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        /// <summary>
        /// Split characters
        /// </summary>
        private static readonly char[] splitters = { '\n' };
        #endregion

        #region Properties
        /// <summary>
        /// Input data
        /// </summary>
        protected T[] Input { get; }
        #endregion

        #region Constructors
        protected Solver(FileInfo file)
        {
            using StreamReader reader = file.OpenText();
            this.Input = reader.ReadToEnd()
                               .Split(splitters, OPTIONS)
                               .Select(Convert)
                               .ToArray();
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Input conversion function
        /// </summary>
        /// <param name="s">Input value</param>
        /// <returns>Target converted value</returns>
        public abstract T Convert(string s);

        /// <summary>
        /// Solver implementation should be in this method
        /// </summary>
        public abstract void Run();
        #endregion
    }
}
