using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2019;

/// <summary>
/// Solver for 2019 Day 10
/// </summary>
public sealed class Day10 : Solver<Vector2<int>[]>
{
    /// <summary>
    /// Asteroid character
    /// </summary>
    private const char ASTEROID = '#';
    /// <summary>
    /// Vaporizations to execute
    /// </summary>
    private const int VAPORIZATIONS = 200;

    /// <summary>
    /// Creates a new <see cref="Day10"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Vector2{T}"/>[] fails</exception>
    public Day10(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        Vector2<int> stationPosition = (-1, -1);
        HashSet<Vector2<int>> bestStation    = new(this.Data.Length);
        HashSet<Vector2<int>> currentStation = new(this.Data.Length);
        foreach (Vector2<int> station in this.Data)
        {
            // Check all other asteroids
            foreach (Vector2<int> asteroid in this.Data)
            {
                if (asteroid == station) continue;

                currentStation.Add((asteroid - station).Reduced);
            }

            // If we have a better station, swap them
            if (currentStation.Count > bestStation.Count)
            {
                (bestStation, currentStation) = (currentStation, bestStation);
                stationPosition = station;
            }

            // Clear current
            currentStation.Clear();
        }
        AoCUtils.LogPart1(bestStation.Count);

        // Create a fake initial vaporization extremely far and ever so slightly to the up left
        Vector2<int> lastDirection = (-1, -999999999);
        Vector2<int> lastVaporized = stationPosition + lastDirection;
        Vector2<int> lastDirectionReduced = lastDirection;

        // Store all asteroids in set exception for station
        HashSet<Vector2<int>> asteroids   = [..this.Data];
        asteroids.Remove(stationPosition);

        // Execute specified number of vaporizations
        foreach (int _ in ..VAPORIZATIONS)
        {
            // Get data from first possible vaporization
            Vector2<int> toVaporize        = asteroids.First();
            Vector2<int> vaporizeDirection = toVaporize - stationPosition;
            Vector2<int> vaporizeDirectionReduced = vaporizeDirection.Reduced;
            int vaporizeDistance    = vaporizeDirection.ManhattanLength;
            Angle vaporizationAngle = Vector2<int>.Angle(lastDirection, vaporizeDirection).Circular;

            // Check all other vaporizations
            foreach (Vector2<int> asteroid in asteroids.Skip(1))
            {
                // Check to make sure we're not in the same direction as previous vaporization
                Vector2<int> direction        = asteroid - stationPosition;
                Vector2<int> directionReduced = direction.Reduced;
                if (directionReduced == lastDirectionReduced) continue;

                // Check if same angle but closer, or smaller angle
                int distance = direction.ManhattanLength;
                Angle angle = Vector2<int>.Angle(lastDirection, direction).Circular;
                if ((directionReduced == vaporizeDirectionReduced && distance < vaporizeDistance) || angle < vaporizationAngle)
                {
                    // Update data
                    toVaporize               = asteroid;
                    vaporizeDirection        = direction;
                    vaporizeDirectionReduced = directionReduced;
                    vaporizeDistance         = distance;
                    vaporizationAngle        = angle;
                }
            }

            // Store previous vaporization data and remove asteroid
            lastVaporized        = toVaporize;
            lastDirection        = vaporizeDirection;
            lastDirectionReduced = vaporizeDirectionReduced;
            asteroids.Remove(toVaporize);
        }
        AoCUtils.LogPart2((lastVaporized.X * 100) + lastVaporized.Y);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override Vector2<int>[] Convert(string[] rawInput)
    {
        int width = rawInput[0].Length;
        int height = rawInput.Length;
        List<Vector2<int>> asteroids = new(width * height / 2);
        foreach (int y in ..height)
        {
            ReadOnlySpan<char> line = rawInput[y];
            foreach (int x in ..width)
            {
                if (line[x] is ASTEROID)
                {
                    asteroids.Add((x, y));
                }
            }
        }
        return asteroids.ToArray();
    }
}
