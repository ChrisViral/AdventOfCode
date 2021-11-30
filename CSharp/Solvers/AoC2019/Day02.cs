using System;
using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 02
/// </summary>
public class Day02 : IntcodeSolver
{
    #region Constants
    /// <summary>
    /// Max value for the noun or verb
    /// </summary>
    private const int MAX = 100;
    /// <summary>
    /// Target to find using the Intcode VM
    /// </summary>
    private const long TARGET = 19_690_720L;
    #endregion
        
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="IntcodeVM"/> fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        (this.VM[1], this.VM[2]) = (12L, 2L);
        this.VM.Run();
        AoCUtils.LogPart1(this.VM[0]);

        foreach (int noun in ..MAX)
        {
            foreach (int verb in ..MAX)
            {
                this.VM.Reset();
                (this.VM[1], this.VM[2]) = (noun, verb);
                this.VM.Run();
                if (this.VM[0] is TARGET)
                {
                    AoCUtils.LogPart2((100 * noun) + verb);
                    return;
                }
            }
        }
    }
    #endregion
}