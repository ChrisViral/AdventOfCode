using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers
{
    public class Day2 : Solver<Day2.PasswordData[]>
    {
        public record PasswordData(int Min, int Max, char Target, string Password);

        #region Constants
        private const string PATTERN = @"(\d+)-(\d+) ([a-z]): ([a-z]+)";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new <see cref="Day2"/> Solver with the input data properly parsed
        /// </summary>
        /// <param name="file">Input file</param>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="file"/> does not exist or has an invalid extension</exception>
        /// <exception cref="FileLoadException">Thrown if the input <paramref name="file"/> could not be properly loaded</exception>
        /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="PasswordData"/> fails</exception>
        public Day2(FileInfo file) : base(file) { }
        #endregion

        #region Methods
        /// <inheritdoc cref="Solver"/>
        public override void Run()
        {
            int part1 = 0;
            int part2 = 0;
            foreach ((int min, int max, char target, string password) in this.Input)
            {
                //Part 1
                int occurrences = password.Count(c => c == target);
                if (occurrences >= min && occurrences <= max)
                {
                    part1++;
                }
                
                //Part 2
                if (password[min - 1] == target)
                {
                    if (password[max - 1] != target)
                    {
                        part2++;
                    }
                }
                else if (password[max - 1] == target)
                {
                    part2++;
                }
            }
            
            AoCUtils.LogPart1(part1);
            AoCUtils.LogPart2(part2);
        }

        /// <inheritdoc cref="Solver{T}"/>
        public override PasswordData[] Convert(string[] rawInput) => RegexUtils.ConstructObjects<PasswordData>(PATTERN, rawInput, RegexOptions.Compiled);
        #endregion
    }
}
