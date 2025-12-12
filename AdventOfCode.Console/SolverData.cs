using JetBrains.Annotations;

namespace AdventOfCode.Console;

/// <summary>
/// SolverData info
/// </summary>
[PublicAPI]
public readonly struct SolverData
{
    /// <summary>
    /// Amount of expected arguments
    /// </summary>
    private const int ARGS = 2;
    /// <summary>
    /// Type qualifier for the solvers
    /// </summary>
    private const string QUALIFIER = $"{nameof(AdventOfCode)}.{nameof(Solvers)}.AoC";

    public readonly int year;
    public readonly int day;
    public readonly string input;
    public readonly string fullName;

    /// <summary>
    /// Creates a new SolveData with the specified values
    /// </summary>
    /// <param name="year">Problem year</param>
    /// <param name="day">Problem day</param>
    /// <param name="input">Problem input</param>
    private SolverData(int year, int day, string input)
    {
        this.year     = year;
        this.day      = day;
        this.input    = input;
        this.fullName = $"{QUALIFIER}{this.year}.Day{this.day:D2}";
    }

    /// <inheritdoc cref="object.ToString"/>
    public override string ToString() => $"{this.year} Day{this.day:D2}";

    /// <summary>
    /// Creates a new SolverData for the specified program arguments
    /// </summary>
    /// <param name="args">Program arguments</param>
    /// <exception cref="ArgumentException">If the <paramref name="args"/> are of the inappropriate length, or if the year cannot be parsed to an integer</exception>
    /// <exception cref="ArgumentNullException">If the day is null or empty</exception>
    public static async Task<SolverData> CreateData(string[] args)
    {
        if (args.Length is not ARGS) throw new ArgumentException($"Arguments have invalid data, {args.Length} arguments when expected {ARGS}.", nameof(args));
        if (!int.TryParse(args[0], out int year)) throw new ArgumentException($"Year ({args[0]}) could not be parsed to integer.", $"{nameof(args)}[0]");
        if (!int.TryParse(args[1], out int day))  throw new ArgumentException($"Day ({args[1]}) could not be parsed to integer.",  $"{nameof(args)}[1]");

        string input = await InputFetcher.EnsureInput(year, day).ConfigureAwait(false);
        return new SolverData(year, day, input);
    }
}
