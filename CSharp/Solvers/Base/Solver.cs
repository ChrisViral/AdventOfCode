using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solvers.Base
{
    /// <summary>
    /// Solver base class
    /// </summary>
    public abstract class Solver : ISolver
    {
        #region Constants
        /// <summary>
        /// Expected input file extension
        /// </summary>
        private const string EXTENSION = ".txt";

        /// <summary>
        /// Default split options
        /// </summary>
        private const StringSplitOptions DEFAULT_OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

        /// <summary>
        /// Default split characters
        /// </summary>
        private static readonly char[] defaultSplitters = { '\n' };
        #endregion

        #region Properties
        /// <summary>
        /// Input data
        /// </summary>
        protected string[] Input { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Solver"/> from the specified file
        /// </summary>
        /// <param name="file">File to load for puzzle input</param>
        /// <param name="splitters">Splitting characters, defaults to newline only</param>
        /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        protected Solver(FileInfo file, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS)
        {
            if (!file.Exists) throw new ArgumentException("Solver file does not exist", nameof(file));
            if (file.Extension is not EXTENSION) throw new ArgumentException($"File extension must be {EXTENSION}, got {file.Extension} instead.", nameof(file));

            try
            {
                using StreamReader reader = file.OpenText();
                this.Input = reader.ReadToEnd()
                                   .Split(splitters ?? defaultSplitters, options)
                                   .ToArray();
            }
            catch (Exception e)
            {
                throw new FileLoadException("Could not properly load the input file.", file.FullName, e);
            }
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Runs the solver on the problem input
        /// </summary>
        public abstract void Run();
        #endregion
    }

    /// <summary>
    /// Solver generic class
    /// </summary>
    /// <typeparam name="T">The fully parse input type</typeparam>
    public abstract class Solver<T> : Solver
    {
        #region Constants
        /// <summary>
        /// Default split options
        /// </summary>
        private const StringSplitOptions DEFAULT_OPTIONS = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        #endregion
        
        #region Properties
        /// <summary>
        /// Parsed input data
        /// </summary>
        protected new T Input { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new generic <see cref="Solver{T}"/> with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <param name="splitters">Splitting characters, defaults to newline only</param>
        /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <typeparamref name="T"/> fails</exception>
        protected Solver(FileInfo file, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(file, splitters, options)
        {
            try
            {
                //Convert is intended to be a Pure function, therefore it should be safe to call in the constructor
                //ReSharper disable once VirtualMemberCallInConstructor
                this.Input = Convert(base.Input);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not convert the string array input to the {typeof(T)} type using the {nameof(Convert)} method.", e);
            }
        }
        #endregion

        #region Abstract methods
        /// <summary>
        /// Input conversion function<br/>
        /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
        /// </summary>
        /// <param name="rawInput">Input value</param>
        /// <returns>Target converted value</returns>
        [Pure]
        public abstract T Convert(string[] rawInput);
        #endregion
    }
}
