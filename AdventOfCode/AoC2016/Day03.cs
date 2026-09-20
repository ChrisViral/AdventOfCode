using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 3
/// </summary>
[Solver(2016, 3)]
public sealed partial class Day03 : RegexSolver<Day03.Triangle>
{
    [InlineArray(3)]
    public struct Triangle
    {
        private int element;

        public bool IsPossible => this[0] + this[1] > this[2]
                               && this[0] + this[2] > this[1]
                               && this[1] + this[2] > this[0];

        public Triangle(int a, int b, int c)
        {
            this[0] = a;
            this[1] = b;
            this[2] = c;
        }
    }

    /// <inheritdoc />
    [GeneratedRegex(@"(\d+)\s+(\d+)\s+(\d+)")]
    protected override partial Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="Day03"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day03(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int possible = this.Data.Count(t => t.IsPossible);
        LogAnswer(possible);

        possible = this.Data.Chunk(3)
                       .SelectMany(c => (..3).Select(i => new Triangle(c[0][i], c[1][i], c[2][i])))
                       .Count(t => t.IsPossible);
        LogAnswer(possible);
    }
}
