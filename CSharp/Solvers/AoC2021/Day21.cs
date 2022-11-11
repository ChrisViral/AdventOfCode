using System;
using System.Collections.Generic;
using AdventOfCode.Extensions;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2021;

/// <summary>
/// Solver for 2021 Day 21
/// </summary>
public class Day21 : Solver<(Day21.Player p1, Day21.Player p2)>
{
    /// <summary>
    /// Player structure
    /// </summary>
    /// <param name="Position">Player board position</param>
    /// <param name="Score">Player score</param>
    public record struct Player(int Position, int Score);

    #region Constants
    /// <summary>Board size</summary>
    private const int BOARD = 10;
    /// <summary>Convolutions for 3d3</summary>
    private static readonly Dictionary<int, int> diceConvolutions = new(6)
    {
        [3] = 1,
        [4] = 3,
        [5] = 6,
        [6] = 7,
        [7] = 6,
        [8] = 3,
        [9] = 1
    };
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day21"/> Solver for 2021 - 21 with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to the target type fails</exception>
    public Day21(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        int rolls = 0;
        int die   = 1;
        Player next    = this.Data.p2;
        Player current = this.Data.p1;
        do
        {
            // Calculate move
            int move = (3 * die) + 3;
            die      = (die + 3) % 100;
            rolls   += 3;
            // Calculate new player position and score
            int newPosition = (current.Position + move) % BOARD;
            int newScore    = current.Score + newPosition + 1;
            (current, next) = (next, current with { Position = newPosition, Score = newScore });
        }
        while (next.Score < 1000);

        AoCUtils.LogPart1(current.Score * rolls);

        // Simulate all possible games
        long p1Wins = 0L, p2Wins = 0L;
        SimulateGame(this.Data.p1, this.Data.p2, ref p1Wins, ref p2Wins);
        AoCUtils.LogPart2(Math.Max(p1Wins, p2Wins));
    }

    /// <summary>
    /// Simulates the players moves in all possible permutations, recursively
    /// </summary>
    /// <param name="current">Current active player</param>
    /// <param name="next">Next active player</param>
    /// <param name="totalCurrent">Total wins for the current player</param>
    /// <param name="totalNext">Total wins for the next player</param>
    /// <param name="permutations">Current permutation total to reach this state, defaults to 1</param>
    private static void SimulateGame(Player current, Player next, ref long totalCurrent, ref long totalNext, long permutations = 1L)
    {
        // Simulate all 3d3 possible results
        foreach (int roll in 3..^9)
        {
            // Calculate permutations, and player position and score
            long newPermutations = permutations * diceConvolutions[roll];
            int newPosition      = (current.Position + roll) % BOARD;
            int newScore         = current.Score + newPosition + 1;

            if (newScore >= 21)
            {
                // If won, add possible permutations to reach this state to the player's win
                totalCurrent += newPermutations;
            }
            else
            {
                // Keep simulating and switch players
                SimulateGame(next, current with { Position = newPosition, Score = newScore }, ref totalNext, ref totalCurrent, newPermutations);
            }
        }
    }

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (Player, Player) Convert(string[] rawInput)
    {
        Player p1 = new(int.Parse(rawInput[0][28..]) - 1, 0);
        Player p2 = new(int.Parse(rawInput[1][28..]) - 1, 0);
        return (p1, p2);
    }
    #endregion
}
