using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Arrays;
using Challenge.Utils.Extensions.Enumerables;

namespace AdventOfCode.AoC2024;

/// <summary>
/// Solver for 2024 Day 2
/// </summary>
[Solver(2024, 2)]
public sealed partial class Day02 : ArraySolver<int[]>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int[][] unsafeReports = this.Data.WhereNot(IsSafe).ToArray();
        int safe = this.Data.Length - unsafeReports.Length;
        LogAnswer(safe);

        int safeDampened = unsafeReports.Count(IsSafeDampened);
        LogAnswer(safe + safeDampened);
    }

    private static bool IsSafe(ICollection<int> report)
    {
        Span<int> signs = stackalloc int[report.Count - 1];

        int i = 0;
        int previous = report.First();
        foreach (int current in report.Skip(1))
        {
            int diff = current - previous;
            if (Math.Abs(diff) is < 1 or > 3) return false;

            signs[i++] = Math.Sign(diff);
            previous   = current;
        }

        int sign = signs[0];
        return signs[1..].All(d => Math.Sign(d) == sign);
    }

    private static bool IsSafeDampened(int[] report)
    {
        LinkedList<int> linkedReport = new(report);
        for (LinkedListNode<int>? removed = linkedReport.First!, current = removed.Next; current is not null; removed = current, current = current.Next)
        {
            linkedReport.Remove(removed);
            if (IsSafe(linkedReport)) return true;

            linkedReport.AddBefore(current, removed);
        }

        linkedReport.RemoveLast();
        return IsSafe(linkedReport);
    }

    /// <inheritdoc cref="ArraySolver{T}.ConvertLine"/>
    protected override int[] ConvertLine(string line) => line.Split(' ').ConvertAll(int.Parse);
}
