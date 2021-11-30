using System;
using System.Linq;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 16
/// </summary>
public class Day16 : Solver<string>
{
    #region Constants
    /// <summary>
    /// Iterations amount
    /// </summary>
    private const int ITERATIONS = 100;
    /// <summary>
    /// Input repeats for part 2
    /// </summary>
    private const int REPEATS = 10_000;
    #endregion
        
    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day16"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="string"/> fails</exception>
    public Day16(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        //Setup old/new arrays
        int length = this.Data.Length;
        char[] current = this.Data.ToCharArray();
        char[] updated = new char[length];
        foreach (int _ in ..ITERATIONS)
        {
            foreach (int i in ..length)
            {
                //Apply the filter
                int total = 0;
                int jump = 2 * (i + 1);
                for (int j = i; j < length; j += jump)
                {
                    for (int k = 0; k <= i && j + k < length; k++)
                    {
                        total += (current[j + k] - '0');
                    }

                    j += jump;
                    for (int k = 0; k <= i && j + k < length; k++)
                    {
                        total -= (current[j + k] - '0');
                    }
                }

                //Update the final value
                updated[i] = (char)((Math.Abs(total) % 10) + '0');
            }
            //Swap old/new
            (current, updated) = (updated, current);
        }
        //Return the start of the array
        AoCUtils.LogPart1(new string(current[..8]));

        //Get full input and cutoff at the offset
        int offset = int.Parse(this.Data[..7]);
        current = Enumerable.Repeat(this.Data, REPEATS).SelectMany(s => s).Skip(offset).ToArray();
            
        //Regenerate old/new array
        length = current.Length;
        updated = new char[length];
        //Last value is always the same
        updated[^1] = current[^1];
        foreach (int _ in ..ITERATIONS)
        {
            for (int i = length - 1; i > 0; /*i--*/)
            {
                //At this point the filter is all ones, so just sum backwards
                updated[i - 1] = (char)(((updated[i--] - '0' + current[i] - '0') % 10) + '0');
            }
            //Swap old/new
            (current, updated) = (updated, current);
        }
        //And return the start again
        AoCUtils.LogPart2(new string(current[..8]));
    }


    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override string Convert(string[] rawInput) => rawInput[0];
    #endregion
}