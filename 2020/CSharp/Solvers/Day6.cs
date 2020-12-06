using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;

namespace AdventOfCode.Solvers
{
    public class Day6 : Solver<HashSet<char>[][]>
    {
        /// <summary>
        /// Creates a new <see cref="Day6"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="HashSet{T}"/>[] fails</exception>
        public Day6(FileInfo file) : base(file, options: StringSplitOptions.TrimEntries) { }

        /// <summary>
        /// Runs the solver on the problem input
        /// </summary>
        public override void Run()
        {
            int anyTotal = 0;
            int allTotal = 0;
            foreach (HashSet<char>[] group in this.Input)
            {
                HashSet<char> anyAnswered = group[0];
                HashSet<char> allAnswered = new(anyAnswered);
                foreach (HashSet<char> answers in group[1..])
                {
                    anyAnswered.UnionWith(answers);
                    allAnswered.IntersectWith(answers);
                }

                anyTotal += anyAnswered.Count;
                allTotal += allAnswered.Count;
            }
            
            Trace.WriteLine(anyTotal);
            Trace.WriteLine(allTotal);
        }

        /// <summary>
        /// Input conversion function<br/>
        /// <b>NOTE</b>: This method <b>must</b> be pure as it initializes the base class
        /// </summary>
        /// <param name="rawInput">Input value</param>
        /// <returns>Target converted value</returns>
        public override HashSet<char>[][] Convert(string[] rawInput)
        {
            List<HashSet<char>[]> input = new();
            List<HashSet<char>> group = new();
            foreach (string line in rawInput)
            {
                if (string.IsNullOrEmpty(line))
                {
                    input.Add(group.ToArray());
                    group.Clear();
                }
                else
                {
                    group.Add(new HashSet<char>(line));
                }
            }

            if (group.Count is not 0)
            {
                input.Add(group.ToArray());
            }

            return input.ToArray();
        }
    }
}
