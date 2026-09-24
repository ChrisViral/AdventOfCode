using Challenge.Solvers;
using Challenge.Utils;
using Challenge.Utils.Extensions.Ranges;
using ZLinq;

namespace AdventOfCode.AoC2016;

/// <summary>
/// Solver for 2016 Day 18
/// </summary>
[Solver(2016, 18)]
public sealed class Day18 : Solver<Day18.Tile[]>
{
    public enum Tile
    {
        SAFE = '.',
        TRAP = '^'
    }

    private const int PART1_ROWS = 40;
    private const int PART2_ROWS = 400_000;

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        // Setup row buffers
        Span<Tile> currentRow = stackalloc Tile[this.Data.Length + 2];
        Span<Tile> nextRow    = stackalloc Tile[currentRow.Length];
        currentRow.Fill(Tile.SAFE);
        nextRow.Fill(Tile.SAFE);

        // Setup initial data
        this.Data.CopyTo(currentRow[1..^1]);
        int safe = this.Data.AsSpan().Count(Tile.SAFE);

        // Generate rows
        foreach (int _ in 1..PART1_ROWS)
        {
            GenerateNextRow(currentRow, nextRow, ref safe);
            SwapUtils.Swap(ref currentRow, ref nextRow);
        }
        LogAnswer(safe);

        // Generate rows
        foreach (int _ in PART1_ROWS..PART2_ROWS)
        {
            GenerateNextRow(currentRow, nextRow, ref safe);
            SwapUtils.Swap(ref currentRow, ref nextRow);
        }
        LogAnswer(safe);
    }

    private void GenerateNextRow(Span<Tile> currentRow, Span<Tile> nextRow, ref int safe)
    {
        // Setup initial tiles
        Tile left   = currentRow[0];
        Tile center = currentRow[1];
        foreach (int i in 1..^this.Data.Length)
        {
            // Setup next tiles
            Tile right = currentRow[i + 1];
            switch (left, center, right)
            {
                // Trap patterns
                case (Tile.TRAP, Tile.TRAP, Tile.SAFE):
                case (Tile.SAFE, Tile.TRAP, Tile.TRAP):
                case (Tile.TRAP, Tile.SAFE, Tile.SAFE):
                case (Tile.SAFE, Tile.SAFE, Tile.TRAP):
                    nextRow[i] = Tile.TRAP;
                    break;

                // Rest is safe
                default:
                    safe++;
                    nextRow[i] = Tile.SAFE;
                    break;
            }

            // Slide window
            left = center;
            center = right;
        }
    }

    /// <inheritdoc />
    protected override Tile[] Convert(string[] rawInput) => rawInput[0].AsValueEnumerable()
                                                                       .Select(c => (Tile)c)
                                                                       .ToArray();
}
