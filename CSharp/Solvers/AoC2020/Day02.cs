using System;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 02
/// </summary>
public class Day02 : Solver<Day02.PasswordData[]>
{
    /// <summary>
    /// Password info data
    /// </summary>
    public record PasswordData(int Min, int Max, char Target, string Password);

    #region Constants
    private const string PATTERN = @"(\d+)-(\d+) ([a-z]): ([a-z]+)";
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="PasswordData"/> fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int part1 = 0;
        int part2 = 0;
        foreach ((int min, int max, char target, string password) in this.Data)
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

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override PasswordData[] Convert(string[] rawInput) => RegexFactory<PasswordData>.ConstructObjects(PATTERN, rawInput, RegexOptions.Compiled);
    #endregion
}
