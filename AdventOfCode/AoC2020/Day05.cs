using AdventOfCode.Extensions.Arrays;
using AdventOfCode.Solvers;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2020;

/// <summary>
/// Solver for 2020 Day 05
/// </summary>
public sealed class Day05 : Solver<Day05.BoardingPass[]>
{
    /// <summary>
    /// BoardingPass info
    /// </summary>
    public sealed class BoardingPass
    {
        public const int MAX_ROW = 127;
        public const int MAX_COLUMN = 7;

        public int Row { get; }

        public int Column { get; }

        public int Id { get; }

        public BoardingPass(string pattern)
        {
            this.Row = BinarySearch(pattern[..7], 'F', MAX_ROW);
            this.Column = BinarySearch(pattern[7..], 'L', MAX_COLUMN);
            this.Id = (this.Row * 8) + this.Column;
        }

        private static int BinarySearch(string pattern, char trueChar, int max)
        {
            int l = 0, r = max;
            int middle = max / 2;

            foreach (char c in pattern)
            {
                if (c == trueChar)
                {
                    r = middle;
                }
                else
                {
                    l = middle + 1;
                }

                middle = l + ((r - l) / 2);
            }

            return middle;
        }
    }

    /// <summary>
    /// Creates a new <see cref="Day05"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the data type fails</exception>
    public Day05(string input) : base(input) { }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        //Part 1
        int max = 0;
        HashSet<int> existing = [];
        bool[,] seats = new bool[BoardingPass.MAX_ROW + 1, BoardingPass.MAX_COLUMN + 1];
        foreach (BoardingPass pass in this.Data)
        {
            max = Math.Max(max, pass.Id);
            existing.Add(pass.Id);
            seats[pass.Row, pass.Column] = true;
        }
        AoCUtils.LogPart1(max);

        //Part 2
        for (int row = 0; row <= BoardingPass.MAX_ROW; row++)
        {
            int rowId = row * 8;
            for (int col = 0; col <= BoardingPass.MAX_COLUMN; col++)
            {
                if (!seats[row, col])
                {
                    int id = rowId + col;
                    if (existing.Contains(id + 1) && existing.Contains(id - 1))
                    {
                        AoCUtils.LogPart2(id);
                        return;
                    }
                }
            }
        }
    }

    /// <inheritdoc />
    protected override BoardingPass[] Convert(string[] rawInput) => rawInput.ConvertAll(s => new BoardingPass(s));
}
