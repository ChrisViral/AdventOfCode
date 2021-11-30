using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Solvers.Base;
using AdventOfCode.Utils;

namespace AdventOfCode.Solvers.AoC2020;

/// <summary>
/// Solver for 2020 Day 22
/// </summary>
public class Day22 : Solver<(int[] p1, int[] p2)>
{
    /// <summary>
    /// Player enum
    /// </summary>
    public enum Player
    {
        P1,
        P2
    }

    #region Constants
    /// <summary>
    /// State creation StringBuilder
    /// </summary>
    private static readonly StringBuilder stateBuilder = new();
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new <see cref="Day22"/> Solver with the input data properly parsed
    /// </summary>
    /// <param name="input">Puzzle input</param>
    /// <exception cref="InvalidOperationException">Thrown if the conversion to <see cref="ValueTuple{T1, T2}"/> fails</exception>
    public Day22(string input) : base(input) { }
    #endregion

    #region Methods
    /// <inheritdoc cref="Solver.Run"/>
    public override void Run()
    {
        //Play a single game
        Queue<int> winner = PlayCombat(this.Data.p1, this.Data.p2);
        int count = winner.Count;
        AoCUtils.LogPart1(winner.Sum(c => c * count--));

        //Play a recursive game
        winner = PlayRecursiveCombat(this.Data.p1, this.Data.p2).deck;
        count = winner.Count;
        AoCUtils.LogPart2(winner.Sum(c => c * count--));
    }

    /// <summary>
    /// Plays a game of Combat and determines the winning deck
    /// </summary>
    /// <param name="p1Deck">Starting deck of the first player</param>
    /// <param name="p2Deck">Starting deck of the second player</param>
    /// <returns>The final deck of the winning player</returns>
    public static Queue<int> PlayCombat(IEnumerable<int> p1Deck, IEnumerable<int> p2Deck)
    {
        //Create decks
        Queue<int> p1 = new(p1Deck);
        Queue<int> p2 = new(p2Deck);
        //Keep running until a player has lost
        while (p1.Count is not 0 && p2.Count is not 0)
        {
            //Draw a card
            int c1 = p1.Dequeue();
            int c2 = p2.Dequeue();
                
            //Check round winner
            if (c1 > c2)
            {
                p1.Enqueue(c1);
                p1.Enqueue(c2);
            }
            else
            {
                p2.Enqueue(c2);
                p2.Enqueue(c1);
            }
        }
        //Return winning deck
        return p1.Count is not 0 ? p1 : p2;
    }

    /// <summary>
    /// Plays a game of Combat recursively and determines the winning deck
    /// </summary>
    /// <param name="p1Deck">Starting deck of the first player</param>
    /// <param name="p2Deck">Starting deck of the second player</param>
    /// <returns>A tuple containing the wining player and the final deck of the winning player</returns>
    public static (Player winner, Queue<int> deck) PlayRecursiveCombat(IEnumerable<int> p1Deck, IEnumerable<int> p2Deck)
    {
        //Create decks
        Queue<int> p1 = new(p1Deck);
        Queue<int> p2 = new(p2Deck);
        //Create states memory and loop until an old state is repeated
        HashSet<string> states = new();
        while (states.Add(GetState(p1, p2)))
        {
            //Draw cards
            int c1 = p1.Dequeue();
            int c2 = p2.Dequeue();
            //Get round winner
            Player winner = p1.Count >= c1 && p2.Count >= c2 ? PlayRecursiveCombat(p1.Take(c1), p2.Take(c2)).winner : (c1 > c2 ? Player.P1 : Player.P2);
            //Adjust deck of winner
            switch (winner)
            {
                case Player.P1:
                    p1.Enqueue(c1);
                    p1.Enqueue(c2);
                    break;
                        
                case Player.P2:
                    p2.Enqueue(c2);
                    p2.Enqueue(c1);
                    break;
            }
                
            //Check for a winner
            if (p1.Count is 0) return (Player.P2, p2);
            if (p2.Count is 0) return (Player.P1, p1);
        }
            
        //If an old state was reached, player 1 wins
        return (Player.P1, p1);
    }

    /// <summary>
    /// Gets a string state for the current decks
    /// </summary>
    /// <param name="p1">Deck of the first player</param>
    /// <param name="p2">Deck of the second player</param>
    /// <returns>A string representation of the game state</returns>
    private static string GetState(IEnumerable<int> p1, IEnumerable<int> p2) => stateBuilder.Clear().AppendJoin(',', p1).Append('-').AppendJoin(',', p2).ToString();

    /// <inheritdoc cref="Solver{T}.Convert"/>
    protected override (int[], int[]) Convert(string[] rawInput)
    {
        int end = Enumerable.Range(1, rawInput.Length).First(i => rawInput[i][0] is 'P');
        return (Array.ConvertAll(rawInput[1..end++], int.Parse), Array.ConvertAll(rawInput[end..], int.Parse));
    }
    #endregion
}