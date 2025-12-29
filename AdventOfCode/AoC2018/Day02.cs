using AdventOfCode.Collections;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Ranges;

namespace AdventOfCode.AoC2018;

/// <summary>
/// Solver for 2018 Day 02
/// </summary>
public sealed class Day02 : ArraySolver<string>
{
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day02(string input) : base(input) { }

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
        AoCUtils.LogPart1(twos * threes);

        string correctID = FindCorrectID();
        AoCUtils.LogPart2(correctID);
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
