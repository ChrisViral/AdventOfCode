using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers
{
    public class Day2 : Solver<Day2.PasswordData[]>
    {
        public record PasswordData(int Min, int Max, char Target, string Password);

        public const string PATTERN = @"(\d+)-(\d+) ([a-z]): ([a-z]+)";

        /// <summary>
        /// Creates a new generic <see cref="Solver{T}"/> with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="PasswordData"/> fails</exception>
        public Day2(FileInfo file) : base(file) { }

        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            int count = 0;
            foreach ((int min, int max, char target, var password) in this.Input)
            {
                int occurrences = password.Count(c => c == target);
                if (occurrences >= min && occurrences <= max)
                {
                    count++;
                }
            }
            
            Trace.WriteLine(count);
            
            count = 0;
            foreach ((int min, int max, char target, var password) in this.Input)
            {
                if (password[min - 1] == target)
                {
                    if (password[max - 1] != target)
                    {
                        count++;
                    }
                }
                else if (password[max - 1] == target)
                {
                    count++;
                }
            }
            Trace.WriteLine(count);
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override PasswordData[] Convert(string[] rawInput) => RegexUtils.CreateObjects<PasswordData>(PATTERN, rawInput, true);
    }
}
