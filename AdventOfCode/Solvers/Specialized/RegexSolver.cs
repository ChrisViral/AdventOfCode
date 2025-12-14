using System.Text.RegularExpressions;
using AdventOfCode.Utils;
using JetBrains.Annotations;

namespace AdventOfCode.Solvers.Specialized;

/// <summary>
/// Regex-parsed array solver
/// </summary>
/// <typeparam name="T">Array element</typeparam>
public abstract class RegexSolver<[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)] T> : Solver<T[]> where T : notnull
{
    /// <summary>
    /// Object line matcher
    /// </summary>
    protected abstract Regex Matcher { get; }

    /// <summary>
    /// Creates a new <see cref="RegexSolver{T}"/> parsing the input lines using <see cref="Matcher"/>
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="splitters">Splitting characters, defaults to newline only</param>
    /// <param name="options">Input parsing options, defaults to removing empty entries and trimming entries</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion the regex parsing fails</exception>
    protected RegexSolver(string input, char[]? splitters = null, StringSplitOptions options = DEFAULT_OPTIONS) : base(input, splitters, options) { }

    /// <inheritdoc />
    public override void Run() { }

    /// <inheritdoc />
    protected override T[] Convert(string[] rawInput) => RegexFactory<T>.ConstructObjects(this.Matcher, rawInput);
}
