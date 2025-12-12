using AdventOfCode.Extensions.Numbers;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Solvers.Specialized;
using AdventOfCode.Utils;

namespace AdventOfCode.AoC2022;

/// <summary>
/// Solver for 2022 Day 02
/// </summary>
public sealed class Day02 : ArraySolver<(Day02.Move opponent, Day02.Move self)>
{
    public readonly struct Move(int value)
    {
        private const int LOSE = 0;
        private const int TIE  = 1;
        private const int WIN  = 2;

        private const int TIE_SCORE = 3;
        private const int WIN_SCORE = 6;

        private readonly int value = value;

        /// <summary>
        /// Move value
        /// </summary>
        private int Value => this.value + 1;

        /// <summary>
        /// Gets the resulting score from a match against the opponent
        /// </summary>
        /// <param name="opponent">Opponent move</param>
        /// <returns>Resulting score</returns>
        public int GetResultFromScore(Move opponent)
        {
            if (this.value == opponent.value)
            {
                // Tie
                return this.Value + TIE_SCORE;
            }
            if (this.value == (opponent.value + 1).Mod(3))
            {
                // Win
                return this.Value + WIN_SCORE;
            }
            // Loss
            return this.Value;
        }

        /// <summary>
        /// Gets the resulting score from a match against the opponent, assuming a preset result
        /// </summary>
        /// <param name="opponent">Opponent move</param>
        /// <returns>Resulting score</returns>
        public int GetScoreFromResult(Move opponent) => this.value switch
        {
            LOSE => (opponent.value - 1).Mod(3) + 1,
            TIE  => opponent.Value + TIE_SCORE,
            WIN  => (opponent.value + 1).Mod(3) + WIN_SCORE + 1,
            _    => throw new InvalidOperationException("Invalid game type")
        };
    }

    /// <summary>
    /// Creates a new <see cref="Day02"/> Solver for 2022 - 02 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day02(string input) : base(input) { }

    /// <inheritdoc cref="Solver.Run"/>
    /// ReSharper disable once CognitiveComplexity
    public override void Run()
    {
        int score = this.Data.Sum(moves => moves.self.GetResultFromScore(moves.opponent));
        AoCUtils.LogPart1(score);

        score = this.Data.Sum(moves => moves.self.GetScoreFromResult(moves.opponent));
        AoCUtils.LogPart2(score);
    }

    /// <inheritdoc cref="ArraySolver{T}.ConvertLine"/>
    protected override (Move, Move) ConvertLine(string line)
    {
        return (new Move(line[0] - 'A'), new Move(line[2] - 'X'));
    }
}
