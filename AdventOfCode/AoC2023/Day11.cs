using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2023;

/// <summary>
/// Solver for 2023 Day 11
/// </summary>
public sealed class Day11 : GridSolver<bool>
{
    private const char GALAXY = '#';
    private const int OLD_EXPANSION = 1_000_000;

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int>[] galaxies = Vector2<int>.MakeEnumerable(this.Data.Width, this.Data.Height)
                                              .Where(p => this.Data[p])
                                              .ToArray();

        bool[] buffer = new bool[this.Data.Height];
        HashSet<int> emptyColumns = [];
        foreach (int x in ..this.Data.Width)
        {
            this.Data.GetColumn(x, buffer);
            if (buffer.Exists(b => b)) continue;

            emptyColumns.Add(x);
        }

        buffer = new bool[this.Data.Width];
        HashSet<int> emptyRows = [];
        foreach (int y in ..this.Data.Height)
        {
            this.Data.GetRow(y, buffer);
            if (buffer.Exists(b => b)) continue;

            emptyRows.Add(y);
        }

        long total = GetTotalDistances(galaxies, emptyRows, emptyColumns);
        AoCUtils.LogPart1(total);

        total = GetTotalDistances(galaxies, emptyRows, emptyColumns, OLD_EXPANSION);
        AoCUtils.LogPart2(total);
    }

    private long GetTotalDistances(Vector2<int>[] galaxies, HashSet<int> emptyRows, HashSet<int> emptyColumns, int emptyExpansion = 2)
    {
        long total = 0L;

        foreach (int i in ..(galaxies.Length - 1))
        {
            Vector2<int> first = galaxies[i];
            foreach (int j in ^i..galaxies.Length)
            {
                Vector2<int> second = galaxies[j];
                int distance = CalculateDistance(first.X, second.X, emptyColumns, emptyExpansion);
                distance += CalculateDistance(first.Y, second.Y, emptyRows, emptyExpansion);
                total += distance;
            }
        }

        return total;
    }

    private static int CalculateDistance(int x1, int x2, HashSet<int> empty, int emptyExpansion)
    {
        if (x1 == x2) return 0;

        if (x1 > x2)
        {
            AoCUtils.Swap(ref x1, ref x2);
        }

        int distance = 0;
        for (int x = x1 + 1; x <= x2; x++)
        {
            distance += empty.Contains(x) ? emptyExpansion : 1;
        }

        return distance;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.Select(c => c is GALAXY).ToArray();
}
