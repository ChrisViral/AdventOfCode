using AdventOfCode.Collections;
using AdventOfCode.Maths.Vectors;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions.Arrays;
using AdventOfCode.Utils.Extensions.Collections;
using ZLinq;

namespace AdventOfCode.AoC2021;

/// <summary>
/// Solver for 2021 Day 04
/// </summary>
public sealed class Day04 : Solver<Day04.BingoData>
{
    /// <summary>
    /// Bingo system data
    /// </summary>
    public sealed class BingoData
    {
        /// <summary>Bingo board size</summary>
        public const int SIZE   = 5;

        /// <summary>
        /// Drawn bingo numbers, in order
        /// </summary>
        public int[] DrawnNumbers { get; }

        /// <summary>
        /// All available bingo boards
        /// </summary>
        public List<Grid<int>> Boards { get; } = [];

        /// <summary>
        /// Creates a new bingo board from the input
        /// </summary>
        /// <param name="input">input data</param>
        public BingoData(string[] input)
        {
            this.DrawnNumbers = Array.ConvertAll(input[0].Split(',', DEFAULT_OPTIONS), int.Parse);
            for (int i = 1; i < input.Length; i += SIZE)
            {
                this.Boards.Add(new Grid<int>(SIZE, SIZE, input[i..(i + SIZE)], line => line.Split(' ', DEFAULT_OPTIONS).ConvertAll(int.Parse)));
            }
        }
    }

    /// <summary>Marked bingo location value</summary>
    private const int MARKED = -1;

    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver for 2021 - 04 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day04(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int? winner = null;
        int? loser  = null;
        foreach (int drawn in this.Data.DrawnNumbers)
        {
            for (int i = 0; i < this.Data.Boards.Count; i++)
            {
                Grid<int> board = this.Data.Boards[i];
                Vector2<int>? position = Vector2<int>.EnumerateOver(BingoData.SIZE, BingoData.SIZE)
                                                     .Cast<Vector2<int>?>()
                                                     .FirstOrDefault(pos => board[pos!.Value] == drawn);
                if (!position.HasValue) continue;

                board[position.Value] = MARKED;
                if (!CheckBoard(board, position.Value)) continue;

                winner ??= board.Where(n => n is not MARKED).Sum() * drawn;
                this.Data.Boards.RemoveAt(i--);
                if (!this.Data.Boards.IsEmpty) continue;

                loser = board.Where(n => n is not MARKED).Sum() * drawn;
                break;
            }

            if (this.Data.Boards.IsEmpty) break;
        }

        AoCUtils.LogPart1(winner!.Value);
        AoCUtils.LogPart2(loser!.Value);
    }

    /// <summary>
    /// Checks the board column and row at the specified position
    /// </summary>
    /// <param name="board">Board to check</param>
    /// <param name="position">Board position</param>
    /// <returns>True if the board has a completed row or column at the specified location</returns>
    private static bool CheckBoard(Grid<int> board, Vector2<int> position)
    {
        (int x, int y) = position;
        Span<int> column = stackalloc int[BingoData.SIZE];
        board.GetColumn(x, column);
        return column.All(n => n is MARKED) || board[y].All(n => n is MARKED);
    }

    /// <inheritdoc />
    protected override BingoData Convert(string[] rawInput) => new(rawInput);
}
