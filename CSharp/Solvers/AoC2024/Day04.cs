using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 04
/// </summary>
public class Day04 : GridSolver<char>
{
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int hits = 0;
        Vector2<int>[] directions = Vector2<int>.Zero.Adjacent(includeDiagonals: true).ToArray();
        foreach (Vector2<int> xPos in this.Data.Dimensions.EnumerateOver())
        {
            if (this.Data[xPos] is not 'X') continue;

            foreach (Vector2<int> direction in directions)
            {
                if (this.Data.TryMoveWithinGrid(xPos, direction, out Vector2<int> mPos) && this.Data[mPos] is 'M'
                 && this.Data.TryMoveWithinGrid(mPos, direction, out Vector2<int> aPos) && this.Data[aPos] is 'A'
                 && this.Data.TryMoveWithinGrid(aPos, direction, out Vector2<int> sPos) && this.Data[sPos] is 'S')
                {
                    hits++;
                }
            }
        }
        AoCUtils.LogPart1(hits);

        hits = 0;
        foreach (Vector2<int> startPos in this.Data.Dimensions.EnumerateOver())
        {
            if (this.Data[startPos] is not 'M') continue;

            // Horizontal Ms
            if (startPos.X < this.Data.Width - 2 && this.Data[startPos + (2, 0)] is 'M')
            {
                // S.S
                // .A.
                // M.M
                if (startPos.Y >= 2
                 && this.Data[startPos + (0, -2)] is 'S'
                 && this.Data[startPos + (2, -2)] is 'S'
                 && this.Data[startPos + (1, -1)] is 'A')
                {
                    hits++;
                }

                // M.M
                // .A.
                // S.S
                if (startPos.Y < this.Data.Height - 2
                 && this.Data[startPos + (0, 2)] is 'S'
                 && this.Data[startPos + (2, 2)] is 'S'
                 && this.Data[startPos + (1, 1)] is 'A')
                {
                    hits++;
                }
            }

            // Vertical Ms
            if (startPos.Y < this.Data.Height - 2 && this.Data[startPos + (0, 2)] is 'M')
            {
                // S.M
                // .A.
                // S.M
                if (startPos.X >= 2
                 && this.Data[startPos + (-2, 0)] is 'S'
                 && this.Data[startPos + (-2, 2)] is 'S'
                 && this.Data[startPos + (-1, 1)] is 'A')
                {
                    hits++;
                }

                // M.S
                // .A.
                // M.S
                if (startPos.X < this.Data.Width - 2
                 && this.Data[startPos + (2, 0)] is 'S'
                 && this.Data[startPos + (2, 2)] is 'S'
                 && this.Data[startPos + (1, 1)] is 'A')
                {
                    hits++;
                }
            }
        }
        AoCUtils.LogPart2(hits);
    }

    /// <inheritdoc cref="GridSolver{T}.LineConverter"/>
    protected override char[] LineConverter(string line) => line.ToCharArray();
    #endregion
}
