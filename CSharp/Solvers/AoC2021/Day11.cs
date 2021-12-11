using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 11
/// </summary>
public class Day11 : GridSolver<byte>
{
    private const int DAYS = 100;

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day11"/> Solver for 2021 - 11 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="Grid{T}"/> fails</exception>
    public Day11(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int flashes = 0;
        Queue<Vector2<int>> toFlash   = new();
        HashSet<Vector2<int>> flashed = new();
        foreach (int _ in ..DAYS)
        {
            flashes += SimulateFlashes(toFlash, flashed);
            flashed.Clear();
        }
        AoCUtils.LogPart1(flashes);

        int day = DAYS;
        do
        {
            day++;
            flashed.Clear();
            SimulateFlashes(toFlash, flashed);
        }
        while (flashed.Count != this.Grid.Size);
        AoCUtils.LogPart2(day);
    }

    private int SimulateFlashes(Queue<Vector2<int>> toFlash, ISet<Vector2<int>> flashed)
    {
        // Check all octopi that will flash
        int flashes = 0;
        Vector2<int>.Enumerate(this.Grid.Width, this.Grid.Height)
                    .Where(WillFlash)
                    .ForEach(toFlash.Enqueue);

        // Check all positions that are flashing
        while (toFlash.TryDequeue(out Vector2<int> position))
        {
            // Flash and add all adjacent
            flashes++;
            position.Adjacent(true)
                    .Where(p => this.Grid.WithinGrid(p) && WillFlash(p) && flashed.Add(p))
                    .ForEach(toFlash.Enqueue);
        }

        // Clear all flashed octopi
        flashed.ForEach(p => this.Grid[p] = 0);
        return flashes;
    }

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
    #endregion
}
