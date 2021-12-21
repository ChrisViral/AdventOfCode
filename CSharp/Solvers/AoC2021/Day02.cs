using System;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 02
/// </summary>
public class Day02 : ArraySolver<(string command, int value)>
{
    #region Constants
    /// <summary>Forward command</summary>
    private const string FORWARD = "forward";
    /// <summary>Down command</summary>
    private const string DOWN    = "down";
    /// <summary>Up command</summary>
    private const string UP      = "up";
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver for 2021 - 02 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day02(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        // Handle cardinal movement
        Vector3<long> position = Vector3<long>.Zero;
        foreach ((string command, int value) in Data)
        {
            switch (command)
            {
                case FORWARD:
                    position += new Vector2<long>(value, 0L);
                    break;
                case DOWN:
                    position += new Vector2<long>(0L, value);
                    break;
                case UP:
                    position -= new Vector2<long>(0L, value);
                    break;
            }
        }

        AoCUtils.LogPart2(position.X * position.Y);

        // Handle heading based movement
        position = Vector3<long>.Zero;
        foreach ((string direction, int value) in Data)
        {
            switch (direction)
            {
                case FORWARD:
                    position += new Vector3<long>(value, position.Z * value, 0L);
                    break;
                case DOWN:
                    position += new Vector3<long>(0L, 0L, value);
                    break;
                case UP:
                    position -= new Vector3<long>(0L, 0L, value);
                    break;
            }
        }

        AoCUtils.LogPart2(position.X * position.Y);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (string command, int value) ConvertLine(string line)
    {
        string[] splits = line.Split(' ');
        return (splits[0], int.Parse(splits[1]));
    }
    #endregion
}
