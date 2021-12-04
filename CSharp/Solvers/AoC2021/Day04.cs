using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Grids;
using AdventOfCode.Grids.Vectors;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;
using AdventOfCode.Utils.Extensions;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 04
/// </summary>
public class Day04 : Solver<Day04.BingoData>
{
    /// <summary>
    /// Bingo system data
    /// </summary>
    public class BingoData
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
        public List<Grid<int>> Boards { get; } = new();

        /// <summary>
        /// Creates a new bingo board from the input
        /// </summary>
        /// <param name="input">input data</param>
        public BingoData(string[] input)
        {
            DrawnNumbers = Array.ConvertAll(input[0].Split(',', DEFAULT_OPTIONS), int.Parse);
            for (int i = 1; i < input.Length; i += SIZE)
            {
                Boards.Add(new(SIZE, SIZE, input[i..(i + SIZE)], line => Array.ConvertAll(line.Split(' ', DEFAULT_OPTIONS), int.Parse)));
            }
        }
    }

    #region Constants
    /// <summary>Marked bingo location value</summary>
    public const int MARKED = -1;
    private static readonly int[] buffer = new int[BingoData.SIZE];
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day04"/> Solver for 2021 - 04 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="int"/>[] fails</exception>
    public Day04(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int? winner = null;
        int? loser  = null;
        foreach (int drawn in Data.DrawnNumbers)
        {
            for (int i = 0; i < Data.Boards.Count; i++)
            {
                Grid<int> board = Data.Boards[i];
                Vector2<int>? position = Vector2<int>.Enumerate(BingoData.SIZE, BingoData.SIZE)
                                                     .Cast<Vector2<int>?>()
                                                     .FirstOrDefault(pos => board[pos!.Value] == drawn);
                if (!position.HasValue) continue;

                board[position.Value] = MARKED;
                if (!CheckBoard(board, position.Value)) continue;

                winner ??= board.Where(n => n is not MARKED).Sum() * drawn;
                Data.Boards.RemoveAt(i--);
                if (!this.Data.Boards.IsEmpty()) continue;

                loser = board.Where(n => n is not MARKED).Sum() * drawn;
                break;
            }

            if (Data.Boards.IsEmpty()) break;
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
        board.GetColumnNoAlloc(x, buffer);
        if (Array.TrueForAll(buffer, n => n is MARKED))
        {
            return true;
        }

        board.GetRowNoAlloc(y, buffer);
        return Array.TrueForAll(buffer, n => n is MARKED);
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override BingoData Convert(string[] rawInput) => new(rawInput);
    #endregion
}
