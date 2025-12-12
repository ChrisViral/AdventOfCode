using AdventOfCode.Intcode;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 19
/// </summary>
public sealed class Day19 : IntcodeSolver
{
    private const int MAP_SIZE = 50;
    private const int REQUIRED_SIZE = 100;

    private static readonly Dictionary<Vector2<int>, bool> BeamMap      = new(100);
    private static readonly Dictionary<Vector2<int>, int> BeamWidthMap  = new(100);
    private static readonly Dictionary<Vector2<int>, int> BeamHeightMap = new(100);

    /// <summary>
    /// Creates a new <see cref="Day19"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day19(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int affected = 0;
        foreach (Vector2<int> position in Vector2<int>.EnumerateOver(MAP_SIZE, MAP_SIZE))
        {
            if (IsAffected(position))
            {
                affected++;
            }
        }
        AoCUtils.LogPart1(affected);

        int beamHeight = 0;
        Vector2<int> startPosition = new(-1, 0);
        do
        {
            startPosition += Vector2<int>.Right;
            switch (GetBeamWidth(startPosition))
            {
                case 0:
                    continue;

                case < REQUIRED_SIZE:
                    startPosition += Vector2<int>.Down;
                    continue;
            }

            beamHeight = GetBeamHeight(startPosition);
        }
        while (beamHeight < REQUIRED_SIZE);

        AoCUtils.LogPart2(startPosition.X * 10000 + startPosition.Y);
    }

    private bool IsAffected(Vector2<int> position)
    {
        // Try get cached value
        if (BeamMap.TryGetValue(position, out bool isAffected)) return isAffected;

        // Run VM
        this.VM.Input.AddValue(position.X);
        this.VM.Input.AddValue(position.Y);
        this.VM.Run();

        // Get and cache result
        isAffected = this.VM.Output.GetValue() is IntcodeVM.TRUE;
        this.VM.Reset();
        BeamMap.Add(position, isAffected);
        return isAffected;
    }

    private int GetBeamWidth(Vector2<int> startPosition)
    {
        if (BeamWidthMap.TryGetValue(startPosition + Vector2<int>.Left, out int width))
        {
            BeamWidthMap.Add(startPosition, --width);
            return width;
        }

        width = 0;
        Vector2<int> position = startPosition;
        while (IsAffected(position))
        {
            width++;
            position += Vector2<int>.Right;
        }

        BeamWidthMap.Add(startPosition, width);
        return width;
    }

    private int GetBeamHeight(Vector2<int> startPosition)
    {
        if (BeamHeightMap.TryGetValue(startPosition + Vector2<int>.Up, out int height))
        {
            BeamHeightMap.Add(startPosition, --height);
            return height;
        }

        height = 0;
        Vector2<int> position = startPosition;
        while (IsAffected(position))
        {
            height++;
            position += Vector2<int>.Down;
        }

        BeamHeightMap.Add(startPosition, height);
        return height;
    }
}
