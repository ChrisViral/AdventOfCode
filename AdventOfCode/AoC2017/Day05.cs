using Challenge.Utils;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2017;

/// <summary>
/// Solver for 2017 Day 05
/// </summary>
public sealed class Day05 : ArraySolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

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
        ChallengeUtils.LogPart1(steps);


        this.Data.CopyTo(jumpTable);
        steps = 0;
        for (int i = 0, jump; i >= 0 && i < jumpTable.Length; i += jump)
        {
            jump = jumpTable[i];
            jumpTable[i] += jump >= 3 ? -1 : 1;
            steps++;
        }
        ChallengeUtils.LogPart2(steps);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
