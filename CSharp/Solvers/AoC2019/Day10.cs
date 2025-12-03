using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2020 Day 10
/// </summary>
public class Day10 : GridSolver<bool>
{
    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="GridSolver{T}"/> fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector2<int>> asteroids = [];
        foreach (int j in ..this.Grid.Height)
        {
            foreach (int i in ..this.Grid.Width)
            {
                if (this.Grid[i, j])
                {
                    asteroids.Add(new Vector2<int>(i, j));
                }
            }
        }

        Vector2<int> station = Vector2<int>.Zero;
        Dictionary<Vector2<int>, int> visible = new();
        foreach (Vector2<int> asteroid in asteroids)
        {
            Dictionary<Vector2<int>, int> targets = new();
            foreach (Vector2<int> direction in asteroids.Where(t => t != asteroid).Select(t => (t - asteroid).Reduced))
            {
                if (targets.ContainsKey(direction))
                {
                    targets[direction]++;
                }
                else
                {
                    targets.Add(direction, 1);
                }
            }

            if (targets.Count <= visible.Count) continue;

            station = asteroid;
            visible = targets;
        }
        AoCUtils.LogPart1(visible.Count);

        Vector2<int> lastDirection = Vector2<int>.Up;
        Vector2<int> lastPosition = Vaporize(visible, station, lastDirection);
        int totalVaporized = 1;

        while (totalVaporized is not 200)
        {
            Angle bestAngle = Angle.FullCircle;
            Vector2<int> closestDirection = Vector2<int>.Zero;
            foreach (Vector2<int> direction in visible.Keys.Where(d => d != lastDirection))
            {
                Angle angle = Vector2<int>.Angle(lastDirection, direction).Circular;
                if (angle >= bestAngle) continue;

                bestAngle = angle;
                closestDirection = direction;
            }
            lastDirection = closestDirection;
            lastPosition = Vaporize(visible, station, closestDirection);
            totalVaporized++;
        }
        AoCUtils.LogPart2((lastPosition.X * 100) + lastPosition.Y);
    }

    /// <summary>
    /// Vaporizes the next asteroid in the specified direction
    /// </summary>
    /// <param name="visible">Dictionary of visible asteroids in the angles at which they are found</param>
    /// <param name="station">Position of the station</param>
    /// <param name="direction">Direction in which to vaporize</param>
    /// <returns>The position of the vaporized asteroid</returns>
    private Vector2<int> Vaporize(IDictionary<Vector2<int>, int> visible, in Vector2<int> station, in Vector2<int> direction)
    {
        if (visible[direction] is 1)
        {
            visible.Remove(direction);
        }
        else
        {
            visible[direction]--;
        }
        Vector2<int> position = station + direction;
        while (!this.Grid[position])
        {
            position += direction;
        }

        this.Grid[position] = false;
        return position;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override bool[] LineConverter(string line) => line.Select(c => c is '#').ToArray();
}
