using Challenge.Collections;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Ranges;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 02
/// </summary>
[Solver(2018, 2)]
public sealed class Day02 : ArraySolver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day02(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int twos = 0;
        int threes = 0;
        foreach (string id in this.Data)
        {
            Counter<char> counter = new(id);
            if (counter.Counts.Any(v => v is 2))
            {
                twos++;
            }
            if (counter.Counts.Any(v => v is 3))
            {
                threes++;
            }
        }
        LogAnswer(twos * threes);

        string correctID = FindCorrectID();
        LogAnswer(correctID);
    }

    // ReSharper disable once CognitiveComplexity
    private string FindCorrectID()
    {
        foreach (int i in ..(this.Data.Length - 1))
        {
            string a = this.Data[i];
            foreach (int j in (i + 1)..this.Data.Length)
            {
                int diffIndex = -1;
                string b = this.Data[j];
                foreach (int n in ..a.Length)
                {
                    if (a[n] != b[n])
                    {
                        if (diffIndex is not -1)
                        {
                            diffIndex = -1;
                            break;
                        }

                        diffIndex = n;
                    }
                }

                if (diffIndex is not -1) return a.Remove(diffIndex, 1);
            }
        }

        return string.Empty;
    }

    /// <inheritdoc />
    protected override string ConvertLine(string line) => line;
}
