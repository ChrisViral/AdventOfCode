using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Enumerables;
using AdventOfCode.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 18
/// </summary>
public sealed class Day18 : GridSolver<bool>
{
    private const char ON = '#';
    private const int STEPS = 100;

    /// <summary>
    /// Creates a new <see cref="Day18"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day18(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        DelayedGrid<bool> lights = new(this.Grid);
        foreach (int _ in ..STEPS)
        {
            UpdateLights(lights);
        }
        AoCUtils.LogPart1(lights.Count(true));

        lights = new DelayedGrid<bool>(this.Grid);
        SetFixedLights(lights);
        lights.Apply();

        foreach (int _ in ..STEPS)
        {
            UpdateLights(lights, true);
        }
        AoCUtils.LogPart2(lights.Count(true));
    }

    // ReSharper disable once CognitiveComplexity
    private static void UpdateLights(DelayedGrid<bool> lights, bool fixCorners = false)
    {
        foreach (Vector2<int> position in lights.Dimensions.Enumerate())
        {
            int onNeighbours = 0;
            foreach (Vector2<int> adjacent in position.Adjacent(withDiagonals: true))
            {
                if (lights.WithinGrid(adjacent))
                {
                    if (lights[adjacent])
                    {
                        onNeighbours++;
                    }
                }
            }
            lights[position] = lights[position] ? onNeighbours is 2 or 3 : onNeighbours is 3;
        }

        if (fixCorners)
        {
            SetFixedLights(lights);
        }
        lights.Apply();
    }

    private static void SetFixedLights(DelayedGrid<bool> lights)
    {
        lights[0, 0] = true;
        lights[0, lights.Height - 1] = true;
        lights[lights.Width - 1, 0] = true;
        lights[lights.Width - 1, lights.Height - 1] = true;
    }

    /// <inheritdoc />
    protected override bool[] LineConverter(string line) => line.AsValueEnumerable()
                                                                .Select(c => c is ON)
                                                                .ToArray();

    /// <inheritdoc />
    protected override string StringConversion(bool obj) => obj ? "#" : ".";
}
