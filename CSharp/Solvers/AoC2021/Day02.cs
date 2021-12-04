using System;
using System.Linq;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using Vector2 = AdventOfCode.Grids.Vectors.Vector2<int>;

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
        Vector3 position = Vector3.Zero;
        foreach ((string direction, int amount) in Data)
        {
            switch (direction)
            {
                case "forward":
                    position += new Vector2(amount, 0);
                    break;
                case "down":
                    position += new Vector2(0, amount);
                    break;
                case "up":
                    position -= new Vector2(0, amount);
                    break;
            }
        }

        AoCUtils.LogPart2(position.X * position.Y);

        position = Vector3.Zero;
        foreach ((string direction, int amount) in Data)
        {
            switch (direction)
            {
                case "forward":
                    position += new Vector3(amount, position.Z * amount, 0);
                    break;
                case "down":
                    position += new Vector3(0, 0, amount);
                    break;
                case "up":
                    position -= new Vector3(0, 0, amount);
                    break;
            }
        }

        AoCUtils.LogPart2((long)position.X * position.Y);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string, int)[] Convert(string[] rawInput)
    {
        return rawInput.Select(line => line.Split(' ')).Select(split => (split[0], int.Parse(split[1]))).ToArray();
    }
    #endregion
}
