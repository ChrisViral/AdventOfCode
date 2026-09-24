using Challenge.Collections;
using Challenge.Maths.Vectors;
using Challenge.Solvers;
using Challenge.Solvers.Specialized;
using Challenge.Utils.Extensions.Enumerables;
using Challenge.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2015;

/// <summary>
/// Solver for 2015 Day 18
/// </summary>
[Solver(2015, 18)]
public sealed class Day18 : GridSolver<bool>
{
    private const char ON = '#';
    private const int STEPS = 100;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        DelayedGrid<bool> lights = new(this.Grid);
        foreach (int _ in ..STEPS)
        {
            UpdateLights(lights);
        }
        LogAnswer(lights.Count(true));

        lights = new DelayedGrid<bool>(this.Grid);
        SetFixedLights(lights);
        lights.Apply();

        foreach (int _ in ..STEPS)
        {
            UpdateLights(lights, true);
        }
        LogAnswer(lights.Count(true));
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
