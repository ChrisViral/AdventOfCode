using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 5
/// </summary>
[Solver(2017, 5)]
public sealed class Day05 : ArraySolver<int>
{

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Copy table
        Span<int> jumpTable = stackalloc int[this.Data.Length];
        this.Data.CopyTo(jumpTable);

        // Perform jumps
        int steps = 0;
        for (int i = 0, jump; i >= 0 && i < jumpTable.Length; i += jump)
        {
            jump = jumpTable[i];
            jumpTable[i]++;
            steps++;
        }
        LogAnswer(steps);

        this.Data.CopyTo(jumpTable);
        steps = 0;
        for (int i = 0, jump; i >= 0 && i < jumpTable.Length; i += jump)
        {
            jump = jumpTable[i];
            jumpTable[i] += jump >= 3 ? -1 : 1;
            steps++;
        }
        LogAnswer(steps);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
