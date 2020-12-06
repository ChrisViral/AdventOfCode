using System;
using System.IO;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    public class Day6 : Solver<string[]>
    {
        /// <summary>
        /// Creates a new <see cref="Day6"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <param name="splitters">Splitting characters, defaults to newline only</param>
        /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
        public Day6(FileInfo file, char[]? splitters = null, StringSplitOptions options = StringSplitOptions.None | StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) : base(file, splitters, options) { }

        /// <summary>
        /// Runs the solver on the problem input
        /// </summary>
        public override void Run()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Input conversion function<br/>
        /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
        /// </summary>
        /// <param name="rawInput">Input value</param>
        /// <returns>Target converted value</returns>
        public override string[] Convert(string[] rawInput) => throw new NotImplementedException();
    }
}
