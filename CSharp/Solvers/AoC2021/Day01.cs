using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 01
/// </summary>
public class Day01 : Solver<int[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver for 2021 - 01 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
    public Day01(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Part 1
        int total = 0;
        foreach (int i in 1..Data.Length)
        {
            if (Data[i] > Data[i - 1])
            {
                total++;
            }
        }

        AoCUtils.LogPart1(total);

        // Part 2
        total = 0;
        int previous = Data[..3].Sum();
        foreach (int i in 3..Data.Length)
        {
            int current = previous + Data[i] - Data[i - 3];
            if (current > previous)
            {
                total++;
            }

            previous = current;
        }

        AoCUtils.LogPart2(total);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override int[] Convert(string[] rawInput) => rawInput.ConvertAll(int.Parse);
    #endregion
}
