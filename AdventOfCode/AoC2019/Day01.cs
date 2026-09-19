using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Microsoft.Extensions.Logging;

namespace AdventOfCode.AoC2019;

/// <summary>
/// Solver for 2019 Day 01
/// </summary>
[Solver(2019, 1)]
public sealed class Day01 : ArraySolver<int>
{
    /// <summary>
    /// Creates a new <see cref="Day01"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <param name="logger">Logger instance</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day01(string input, ILogger logger) : base(input, logger) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int fuelRequirement = 0;
        int compoundFuelRequirement = 0;
        foreach (int mass in this.Data)
        {
            int fuel = (mass / 3) - 2;
            fuelRequirement += fuel;
            while (fuel > 8)
            {
                fuel =  (fuel / 3) - 2;
                compoundFuelRequirement += fuel;
            }
        }
        LogAnswer(fuelRequirement);
        LogAnswer(fuelRequirement + compoundFuelRequirement);
    }

    /// <inheritdoc />
    protected override int ConvertLine(string line) => int.Parse(line);
}
