using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2024;

/// <summary>
/// Solver for 2024 Day 08
/// </summary>
public sealed class Day08 : GridSolver<char>
{
    /// <summary>
    /// Empty map value
    /// </summary>
    private const char EMPTY = '.';

    /// <summary>
    /// Creates a new <see cref="Day08"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day08(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        HashSet<Vector2<int>> harmonicAntinodes = new(100);
        Dictionary<char, List<Vector2<int>>> antennasByFrequency = new();
        foreach (Vector2<int> antennaPos in this.Grid.Dimensions.Enumerate())
        {
            // Check if we're on an antenna
            char frequency = this.Grid[antennaPos];
            if (frequency is EMPTY) continue;

            // Get antenna list
            if (!antennasByFrequency.TryGetValue(frequency, out List<Vector2<int>>? antennas))
            {
                antennas = new List<Vector2<int>>(10);
                antennasByFrequency.Add(frequency, antennas);
            }

            // Add antenna to list for current frequency
            antennas.Add(antennaPos);
            // Antennas are always harmonic antinodes
            harmonicAntinodes.Add(antennaPos);
        }

        HashSet<Vector2<int>> antinodes = new(100);
        foreach (List<Vector2<int>> antennas in antennasByFrequency.Values)
        {
            foreach ((Vector2<int> left, Vector2<int> right) in antennas.EnumeratePairs())
            {
                // Antennas are always harmonic antinodes
                harmonicAntinodes.Add(left);
                harmonicAntinodes.Add(right);

                // Get separation
                Vector2<int> separation = left - right;

                // Check antinodes of left antenna
                Vector2<int> antinode = left + separation;
                if (this.Grid.WithinGrid(antinode))
                {
                    antinodes.Add(antinode);
                    do
                    {
                        // Keep moving out and adding harmonic antinodes until outside of grid
                        harmonicAntinodes.Add(antinode);
                        antinode += separation;
                    }
                    while (this.Grid.WithinGrid(antinode));
                }

                // Check antinodes of right antenna
                antinode = right - separation;
                if (this.Grid.WithinGrid(antinode))
                {
                    antinodes.Add(antinode);
                    do
                    {
                        // Keep moving out and adding harmonic antinodes until outside of grid
                        harmonicAntinodes.Add(antinode);
                        antinode -= separation;
                    }
                    while (this.Grid.WithinGrid(antinode));
                }
            }
        }
        AoCUtils.LogPart1(antinodes.Count);
        AoCUtils.LogPart2(harmonicAntinodes.Count);
    }

    /// <inheritdoc />
    protected override char[] LineConverter(string line) => line.ToCharArray();
}
