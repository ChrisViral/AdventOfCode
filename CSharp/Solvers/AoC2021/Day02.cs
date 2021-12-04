using System;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 02
/// </summary>
public class Day02 : Solver<(string, int)[]>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver for 2021 - 02 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/>[] fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        Vector2<int> vector = new(3, 9);
        Vector2<int> reduced = vector.Reduced;


        Vector2<double> otherVector = new(3d, 9d);
        Vector2<double> normalized = otherVector.Normalized;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string, int)[] Convert(string[] rawInput)
    {
        return rawInput.Select(line => line.Split(' ')).Select(split => (split[0], int.Parse(split[1]))).ToArray();
    }
    #endregion
}
