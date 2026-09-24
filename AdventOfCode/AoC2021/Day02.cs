using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 2
/// </summary>
[Solver(2021, 2)]
public sealed class Day02 : ArraySolver<(string command, int value)>
{
    /// <summary>Forward command</summary>
    private const string FORWARD = "forward";
    /// <summary>Down command</summary>
    private const string DOWN    = "down";
    /// <summary>Up command</summary>
    private const string UP      = "up";

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Handle cardinal movement
        Vector3<long> position = Vector3<long>.Zero;
        foreach ((string command, int value) in this.Data)
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

        LogAnswer(position.X * position.Y);

        // Handle heading based movement
        position = Vector3<long>.Zero;
        foreach ((string direction, int value) in this.Data)
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

        LogAnswer(position.X * position.Y);
    }

    /// <inheritdoc />
    protected override (string command, int value) ConvertLine(string line)
    {
        string[] splits = line.Split(' ');
        return (splits[0], int.Parse(splits[1]));
    }
}
