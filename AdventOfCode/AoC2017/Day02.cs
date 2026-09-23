using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 2
/// </summary>
[Solver(2017, 2)]
public sealed partial class Day02 : ArraySolver<int[]>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int checksum = this.Data.Sum(GetRowDiff);
        LogAnswer(checksum);

        checksum = this.Data.Sum(GetRowQuotient);
        LogAnswer(checksum);
    }

    private static int GetRowDiff(int[] row)
    {
        int min = row[0];
        int max = min;
        for (int i = 1; i < row.Length; i++)
        {
            int value = row[i];
            if (value < min)
            {
                min = value;
            }
            else if (value > max)
            {
                max = value;
            }
        }
        return max - min;
    }

    private static int GetRowQuotient(int[] row)
    {
        for (int i = 0; i < row.Length - 1; i++)
        {
            int a = row[i];
            for (int j = i + 1; j < row.Length; j++)
            {
                int b = row[j];
                (int q, int r) = a > b ? Math.DivRem(a, b) : Math.DivRem(b, a);
                if (r is 0) return q;
            }
        }
        throw new InvalidOperationException("Invalid row");
    }

    /// <inheritdoc />
    protected override int[] ConvertLine(string line)
    {
        ReadOnlySpan<char> input = line;
        int count = input.Count('\t') + 1;
        Span<Range> splits = stackalloc Range[count];
        count = input.Split(splits, '\t', DEFAULT_OPTIONS);
        int[] row = new int[count];
        for (int i = 0; i < count; i++)
        {
            row[i] = int.Parse(input[splits[i]]);
        }

        return row;
    }
}
