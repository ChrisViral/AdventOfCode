using AdventOfCode.Extensions.Enumerables;
using AdventOfCode.Extensions.Ranges;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Vectors;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 11
/// </summary>
public sealed class Day11 : GridSolver<byte>
{
    /// <summary>Simulation days</summary>
    private const int DAYS = 100;
    /// <summary>Queue to store the octopi that mush flash</summary>
    private static readonly Queue<Vector2<int>> ToFlash   = new();
    /// <summary>Set containing all octopi that have flashed</summary>
    private static readonly HashSet<Vector2<int>> Flashed = [];

    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver for 2021 - 11 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day11(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int flashes = 0;
        foreach (int _ in ..DAYS)
        {
            // Simulate flashes for each day
            flashes += SimulateFlashes();
        }
        AoCUtils.LogPart1(flashes);

        int day = DAYS;
        do
        {
            // Simulate until everything has flashed
            day++;
            flashes = SimulateFlashes();
        }
        while (flashes != this.Grid.Size);
        AoCUtils.LogPart2(day);
    }

    /// <summary>
    /// Simulates flashes on the entire board
    /// </summary>
    /// <returns>The amount of flashes that occurred</returns>
    private int SimulateFlashes()
    {
        // Check all octopi that will flash
        int flashes = 0;
        Vector2<int>.MakeEnumerable(this.Grid.Width, this.Grid.Height)
                    .Where(WillFlash)
                    .ForEach(ToFlash.Enqueue);
        Flashed.AddRange(ToFlash);

        // Check all positions that are flashing
        while (ToFlash.TryDequeue(out Vector2<int> position))
        {
            // Flash and add all adjacent
            flashes++;
            position.AsAdjacentEnumerable(withDiagonals: true)
                    .Where(p => this.Grid.WithinGrid(p) && WillFlash(p) && Flashed.Add(p))
                    .ForEach(ToFlash.Enqueue);
        }

        // Clear all flashed octopi
        Flashed.ForEach(p => this.Grid[p] = 0);
        Flashed.Clear();
        return flashes;
    }

    /// <summary>
    /// Check if a given octopi will flash on the given turn
    /// </summary>
    /// <param name="position">Position to check</param>
    /// <returns><see langword="true"/> if the octopi will flash, <see langword="false"/> otherwise</returns>
    private bool WillFlash(Vector2<int> position)
    {
        // If at nine, will be flashing
        if (this.Grid[position] is 9)
        {
            return true;
        }

        // Otherwise will not flash, increment
        this.Grid[position]++;
        return false;
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override byte[] LineConverter(string line) => line.Select(c => (byte)(c - '0')).ToArray();
}
